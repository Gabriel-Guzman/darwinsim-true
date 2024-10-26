using System.Collections.Generic;
using System.Linq;
using Abilities;
using Entities;
using Sensors;
using UnityEngine;
using UnityEngine.Assertions;
using UnitySharpNEAT.SharpNEAT.Decoders;
using UnitySharpNEAT.SharpNEAT.Genomes.Neat;

public interface ITeam
{
    Color Color { get; }
    Dictionary<string, Living> Entities { get; }
    void InitializePopulation(uint count, uint generation);
    void Spawn(List<NeatGenome> genomes);
    void Spawn(NeatGenome genome);
    void Fill(uint size, uint generation);
    void RemoveEntity(string name);
    void RemoveEntity(Living living);
    int AliveCount();
    void Empty();
}
public class Team : EntityManager<Living>, ITeam
{
    public readonly NeatGenomeFactory GenomeFactory;
    public readonly EntityManager<TreeBehavior> TreeManager;
    private GameObject _container;

    public Team(string name,
        Living prefab,
        List<IAbility> abilities,
        List<ISense> senses,
        List<ILivingCollisionHandler> collisionHandlers,
        EntityManager<TreeBehavior> treeManager) : base("Team " + name, treeManager.Arena, prefab)
    {
        Assert.AreNotEqual(abilities.Count, 0, "Abilities list cannot be empty.");
        Assert.AreNotEqual(senses.Count, 0, "Senses list cannot be empty.");
        // Assert.IsTrue(initialPopulationSize > 0, "Population size must be greater than zero.");
        // Assert.IsTrue(initialPopulationSize > matingPoolSize, "Population size must be greater than mating pool size.");
        TreeManager = treeManager;
        Name = name;
        Abilities = abilities;
        Senses = senses;
        CollisionHandlers = collisionHandlers;

        // InitialPopulationSize = initialPopulationSize;
        Color = Color.red;

        var nodesRequiredSum = 0;
        foreach (var sense in Senses)
        {
            nodesRequiredSum += sense.NodesRequired();
        }

        var outputNodesRequiredSum = 0;
        foreach (var ability in Abilities)
        {
            outputNodesRequiredSum += ability.NodesRequired();
        }

        GenomeFactory = new NeatGenomeFactory(nodesRequiredSum, outputNodesRequiredSum);
    }

    public void InitializePopulation(uint count, uint birthGeneration)
    {
        var genomes = GenomeFactory.CreateGenomeList((int)count, birthGeneration);
        Spawn(genomes);
    }

    public void Spawn(List<NeatGenome> genomes)
    {
        for (var i = 0; i < genomes.Count; i++)
        {
            Spawn(genomes[i]);
        }
    }

    public void Spawn(NeatGenome genome)
    {
        var unit = Spawn();
        
        unit.BlackBox = FastAcyclicNetworkFactory.CreateFastAcyclicNetwork(genome);
        unit.Genome = genome;
        
        unit.Abilities.AddRange(Abilities);
        unit.Senses.AddRange(Senses);
        unit.CollisionHandlers.AddRange(CollisionHandlers);
        
        unit.TreeManager = TreeManager;
        unit.Color = Color;
    }

    public void Fill(uint count, uint birthGeneration)
    {
        var units = Entities.Values.ToList();
        while (Entities.Count < count)
        {
            var parent = units[Random.Range(0, units.Count)];
            var newChild = parent.Genome.CreateOffspring(birthGeneration);
            Spawn(newChild);
        }
    }

    public int AliveCount()
    {
        var sum = 0;
        foreach (var unit in Entities.Values)
        {
            if (unit.Health > 0)
            {
                sum++;
            }
        }

        return sum;
    }

    public string Name;

    public readonly List<IAbility> Abilities;
    public readonly List<ISense> Senses;
    public readonly List<ILivingCollisionHandler> CollisionHandlers;

    public Color Color { get; }
}