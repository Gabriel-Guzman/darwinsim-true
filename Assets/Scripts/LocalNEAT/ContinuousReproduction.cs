using System.Collections;
using System.Collections.Generic;
using Entities;
using UnityEngine;
using UnitySharpNEAT.SharpNEAT.Genomes.Neat;
using UnitySharpNEAT.SharpNEAT.Utility;

namespace LocalNEAT
{
    public class ContinuousRepro : IReproStrategy
    {
        public ReproStrategyState State { get; private set; } = ReproStrategyState.Stopped;
        public ReproductionStrategy Type => ReproductionStrategy.Continuous;
        public ITeam Team { get; }
        public ContinuousReproductionStatistics CRStatistics { get; } = new();

        public string Details { get; } = "Continuous reproduction";

        private readonly FastRandom _random = new FastRandom();

        private float _lastPollSeconds = 0f;
        private bool _stopFlag;
        private uint _generationCount;

        private readonly float _maxReproductionAgeSeconds;
        private readonly float _minReproductionAgeSeconds;
        private readonly float _chancePerUnitPerSecond;
        private readonly float _minHealth;
        private readonly float _pollingTimeSeconds;

        public ContinuousRepro(
            Team team,
            float maxReproductionAgeSeconds,
            float minReproductionAgeSeconds,
            float chancePerUnitPerSecond,
            float minHealth,
            float pollingTimeSeconds
        )
        {
            Team = team;
            _minReproductionAgeSeconds = minReproductionAgeSeconds;
            _maxReproductionAgeSeconds = maxReproductionAgeSeconds;
            _chancePerUnitPerSecond = chancePerUnitPerSecond;
            _minHealth = minHealth;
            _pollingTimeSeconds = pollingTimeSeconds;
        }

        public IReproductionStatistics Statistics => CRStatistics;

        private enum StopReason
        {
            Time,
            Extinction,
            Stopflag,
            None
        }

        public IEnumerator Coroutine()
        {
            State = ReproStrategyState.Running;

            while (true)
            {
                var reason = StopReason.None;
                yield return new WaitUntil(() => StopCondition(out reason));

                if (reason == StopReason.Time)
                {
                    PruneDeadUnits();
                    // reproduce
                    PerformReproduction();
                }
                else
                {
                    State = ReproStrategyState.Stopped;
                    yield break;
                }
            }
        }


        private bool StopCondition(out StopReason reason)
        {
            if (_stopFlag)
            {
                reason = StopReason.Stopflag;
                return true;
            }

            if (Team.AliveCount() == 0)
            {
                reason = StopReason.Extinction;
                return true;
            }

            if (Time.time - _lastPollSeconds > _pollingTimeSeconds)
            {
                reason = StopReason.Time;
                _lastPollSeconds = Time.time;
                return true;
            }

            reason = StopReason.None;
            return false;
        }

        private void PruneDeadUnits()
        {
            var deadUnits = new List<Living>();
            foreach (var living in Team.Entities.Values)
            {
                if (living.Health <= 0)
                {
                    deadUnits.Add(living);
                }
            }

            foreach (var deadUnit in deadUnits)
            {
                Team.RemoveEntity(deadUnit);
            }
        }

        private void PerformReproduction()
        {
            var viableUnits = new List<Living>();
            foreach (var living in Team.Entities.Values)
            {
                if (FitForReproduction(living))
                {
                    viableUnits.Add(living);
                }
            }

            if (viableUnits.Count < 2) return;

            var pickList = new List<Living>(viableUnits);
            while (pickList.Count > 1)
            {
                var parentIndex1 = _random.Next(pickList.Count);
                var parent1 = pickList[parentIndex1];
                pickList.RemoveAt(parentIndex1);
                var parent2 = pickList[_random.Next(pickList.Count)];

                // if chance per second * polling time > 1 (slow polls or very high sex chance)
                // then sex is guaranteed. this approximates the reproduction possibility, but
                // note that lower poll times will be more random.
                if (_random.NextDouble() < _chancePerUnitPerSecond * _pollingTimeSeconds)
                {
                    Debug.Log("Produced offspring");
                    var offspring = parent1.Genome.CreateOffspring(parent2.Genome, _generationCount);
                    Team.Spawn(offspring);
                }
            }

            _generationCount++;
        }

        private bool FitForReproduction(Living living)
        {
            var fit = true;

            // age of consent but not menopausal
            if (living.Age < _minReproductionAgeSeconds || living.Age > _maxReproductionAgeSeconds)
            {
                fit = false;
            }

            // is healthy enough
            if (living.Health < _minHealth)
            {
                fit = false;
            }

            return fit;
        }

        public void StopImmediately()
        {
            _stopFlag = true;
        }
    }

    public class ContinuousReproductionStatistics : IReproductionStatistics
    {
        private float _bestScore;
        private NeatGenome _bestGenome;

        public float BestScore()
        {
            return _bestScore;
        }

        public float GetRunningSeconds()
        {
            throw new System.NotImplementedException();
        }
    }
}