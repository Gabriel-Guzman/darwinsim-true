using System.Collections;

namespace LocalNEAT
{
    public enum ReproStrategyState
    {
        Running,
        Stopped,
    }
    public interface IReproStrategy
    {
        ReproStrategyState State { get; }
        ReproductionStrategy Type { get; }
        ITeam Team { get; }
        IReproductionStatistics Statistics { get; }
        IEnumerator Coroutine();
        string Details { get; }
        void StopImmediately();
    }

    public interface IReproductionStatistics
    {
        float BestScore();
        float GetRunningSeconds();
    }
}