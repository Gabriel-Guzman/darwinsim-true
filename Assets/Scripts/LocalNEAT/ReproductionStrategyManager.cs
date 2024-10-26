using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace LocalNEAT
{
    public enum ReproStrategyManagerState
    {
        Running,
        Switching,
        Stopped
    }
    public class ReproductionStrategyManager
    {
        private readonly Dictionary<ReproductionStrategy, IReproStrategy> _strategies = new();
        public ReproStrategyManagerState State { get; private set; } = ReproStrategyManagerState.Stopped;

        public ReproductionStrategyManager(List<IReproStrategy> strategies)
        {
            foreach (var strategy in strategies) {
                _strategies[strategy.Type] = strategy;
            }
        }
        public ReproductionStrategy Current { get; private set; }

        public IEnumerator StartStrategyCoroutine(ReproductionStrategy strategy)
        {
            if (State == ReproStrategyManagerState.Running)
            {
                if (strategy == Current) yield break;
                yield return SwitchStrategyCoroutine(strategy);
                yield break;
            }
            Current = strategy;
            var newStrategy = _strategies[strategy];
            
            State = ReproStrategyManagerState.Running;
            yield return newStrategy.Coroutine();
        }
    
        private IEnumerator SwitchStrategyCoroutine(ReproductionStrategy strategy)
        {
            State = ReproStrategyManagerState.Switching;
            Assert.AreNotEqual(strategy, Current);

            // get the strategy 
            var newStrategy = _strategies[strategy];

            // stop the strategy if it's running
            var currentStrategy = _strategies[Current];
            currentStrategy.StopImmediately();
            
            yield return new WaitUntil(() => currentStrategy.State == ReproStrategyState.Stopped); 
            
            Current = strategy;  // update current strategy 
            State = ReproStrategyManagerState.Running;  // update state 
            yield return newStrategy.Coroutine();  // start the new strategy 
        }
    }
}