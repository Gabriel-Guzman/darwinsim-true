using System.Collections.Generic;
using System.Linq;
using Abilities;
using Entities;
using LocalNEAT;
using Sensors;
using UnityEngine;

public enum GameState
{
    Stopped,
    Running,
}

public enum ReproductionStrategy
{
    AllAtOnce,
    Continuous,
}

public class GameManager : MonoBehaviour
{
    public Living unitPrefab;
    public GameObject arenaPrefab;
    public TreeBehavior treePrefab;
    public GameState State { get; private set; }


    #region generation options

    // how many ticks to wait before the next mating cycle
    public int secondsBetweenSex = 500;

    #endregion

    private EntityManager<TreeBehavior> _treeManager;
    private ReproductionStrategyManager _reproductionStrategyManager;

    public void Init()
    {
        if (State == GameState.Running) return;
        var senses = new List<ISense>
        {
            new Sight(60, 4, 5),
            new Taste(),
            new Sensors.Touch()
        };

        var abilities = new List<IAbility> { new Movement() };

        var collisionHandlers = new List<ILivingCollisionHandler>
        {
            new EatTreesCollisionHandler(1 / 5f),
            new TouchedSomethingCollisionHandler()
        };

        var treeManager = new EntityManager<TreeBehavior>("Tree", arenaPrefab, treePrefab);
        treeManager.Spawn(10);
        _treeManager = treeManager;

        var team1 = new Team(
            "Team 1",
            unitPrefab,
            abilities,
            senses,
            collisionHandlers,
            _treeManager
        );

        var aao = new AllAtOnceRepro(
            team1,
            (uint)secondsBetweenSex,
            6,
            12
        );
        var con = new ContinuousRepro(
            team1,
            300,
            100,
            0.08f,
            0.75f,
            0.5f
        );
        var manager = new ReproductionStrategyManager(new List<IReproStrategy> { aao, con });
        _reproductionStrategyManager = manager;
        StartCoroutine(manager.StartStrategyCoroutine(ReproductionStrategy.AllAtOnce));

        State = GameState.Running;
    }

    public void ToggleStrategy()
    {
        if (_reproductionStrategyManager.Current == ReproductionStrategy.AllAtOnce)
        {
            StartCoroutine(_reproductionStrategyManager.StartStrategyCoroutine(ReproductionStrategy.Continuous));
        }
        else
        {
            StartCoroutine(_reproductionStrategyManager.StartStrategyCoroutine(ReproductionStrategy.AllAtOnce));
        }
    }

    private void FixedUpdate()
    {
        if (State == GameState.Stopped)
        {
        }
    }
}