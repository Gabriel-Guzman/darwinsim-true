using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Entities;
using UnityEngine;
using UnityEngine.Assertions;
using UnitySharpNEAT.SharpNEAT.Core;
using UnitySharpNEAT.SharpNEAT.Genomes.Neat;
using Util;

namespace LocalNEAT
{
    public class AllAtOnceRepro : IReproStrategy
    {
        public ReproStrategyState State { get; private set; } = ReproStrategyState.Stopped;
        public ReproductionStrategy Type => ReproductionStrategy.AllAtOnce;
        public string Details => "All at once reproduction";
        public ITeam Team { get; }
        private readonly uint _generationLength;
        private readonly uint _targetPopulationSize;
        public float SecondsStartedAt { get; private set; }
        private bool _stopFlag;
        private uint _generationCount;
        private readonly uint _matingPoolSize;

        public AllAtOnceReproductionStatistics AtoStats { get; }
        public IReproductionStatistics Statistics => AtoStats;

        public AllAtOnceRepro(Team team, uint maxGenerationLength, uint matingPoolSize, uint targetPopulationSize)
        {
            Assert.AreNotEqual(0, maxGenerationLength, "Generation length must be greater than zero.");

            Team = team;
            _generationLength = maxGenerationLength;
            AtoStats = new AllAtOnceReproductionStatistics();
            _matingPoolSize = matingPoolSize;
            _targetPopulationSize = targetPopulationSize;
        }

        /// <summary>
        /// Coroutine that manages the reproduction process for the team.
        /// It initializes the population, handles reproduction cycles, and tracks generation statistics.
        /// </summary>
        /// <returns>An IEnumerator for coroutine execution.</returns>
        public IEnumerator Coroutine()
        {
            State = ReproStrategyState.Running;
            
            // Prepare team for algorithm
            if (Team.Entities.Count == 0)
            {
                _generationCount = 0;
                Team.InitializePopulation(_targetPopulationSize, _generationCount);
            }
            else if (Team.Entities.Count < _targetPopulationSize)
            {
                // add random units to reach target population
                while (Team.Entities.Count < _targetPopulationSize)
                {
                    Team.Fill(_targetPopulationSize, _generationCount);
                }
            } else if (Team.Entities.Count > _targetPopulationSize)
            {
                // remove the weakest units until target population is reached
                var sorted = Team.Entities.Values.OrderByDescending(x => x.GetScore()).ToList();
                while (Team.Entities.Count > _targetPopulationSize)
                {
                    Team.RemoveEntity(sorted[Team.Entities.Count - 1]);
                }
            }
            

            _generationCount++;
            while (true)
            {
                SecondsStartedAt = Time.time;

                yield return new WaitUntil(() => StopConditionMet());
                if (_stopFlag)
                {
                    _stopFlag = false;
                    State = ReproStrategyState.Stopped;
                    yield break;
                }

                PerformReproduction();
                _generationCount++;
            }
        }

        private bool StopConditionMet()
        {
            var timeMet = Time.time - SecondsStartedAt > _generationLength;
            var ret = timeMet || Team.AliveCount() == 0 || _stopFlag;

            return ret;
        }


        public void StopImmediately()
        {
            _stopFlag = true;
        }


        private void PerformReproduction()
        {
            AtoStats.TrackGeneration(Team.Entities.Values.ToList(), _generationCount);
            var unitList = Team.Entities.Values;
            // var pruned = unitList.SkipWhile(unit => unit.CanReproduce());
            var sorted = unitList.OrderByDescending(unit => unit.GetScore()).ToList();
            foreach (var living in sorted)
            {
                var ei = living.Genome.EvaluationInfo;
                ei.SetFitness(living.GetScore());
            }

            var newGenomes = new List<NeatGenome>();
            var steps = _matingPoolSize / 2;
            for (var i = 0; i < steps; i++)
            {
                var parent1 = sorted[i * 2];
                var parent2 = sorted[i * 2 + 1];

                for (var j = 0; j < 2; j++)
                {
                    newGenomes.Add(parent1.Genome.CreateOffspring(parent2.Genome, _generationCount));
                }
            }

            while (newGenomes.Count < _targetPopulationSize)
            {
                newGenomes.Add(sorted[0].Genome.CreateOffspring(sorted[1].Genome, _generationCount));
            }

            Team.Empty();
            Team.Spawn(newGenomes);
        }
    }

    public class AllAtOnceReproductionStatistics : IReproductionStatistics
    {
        // private NeatGenome _bestGenome;
        private float _bestScore;
        private double _maxComplexity;
        public float RunningSeconds;
        private uint _bestGeneration;

        public float BestScore()
        {
            return _bestScore;
        }

        public float GetRunningSeconds()
        {
            return RunningSeconds;
        }

        public void TrackGeneration(List<Living> units, uint generation)
        {
            foreach (var unit in units)
            {
                var score = unit.GetScore();
                if (score > _bestScore)
                {
                    _bestScore = score;
                    _bestGeneration = generation;
                }
        
                if (unit.Genome.Complexity > _maxComplexity)
                {
                    _maxComplexity = unit.Genome.Complexity;
                }
            }
        
            DarwinDebug.Log($@"Generation {generation}:
Best score: {_bestScore}
Max complexity: {_maxComplexity}
Best generation: {_bestGeneration}
");
        }
    }
}