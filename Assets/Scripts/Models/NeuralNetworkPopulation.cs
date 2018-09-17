using System;
using System.Collections.Generic;
using UnityEngine;

public class NeuralNetworkPopulation
{
    public float BestFitness { get { return GeneticAlgorithm.BestFitness; } }
    public float AverageFitness { get { return GeneticAlgorithm.AverageFitness; } }
    public float WorstFitness { get { return GeneticAlgorithm.WorstFitness; } }
    public float TotalFitness { get { return GeneticAlgorithm.TotalFitness; } }
    public float Generation { get { return GeneticAlgorithm.Generation; } }
    public List<Genome> Genomes { get { return GeneticAlgorithm.Population; } }
    public Genome FittestGenome { get { return GeneticAlgorithm.Population[GeneticAlgorithm.FittestGenomeIndex]; } }
    public NeuralNetwork FittestNeuralNetwork { get { return Population[GeneticAlgorithm.FittestGenomeIndex]; } }

    public GeneticAlgorithm GeneticAlgorithm;
    public List<NeuralNetwork> Population;

    public NeuralNetworkPopulation()
    {
        Population = new List<NeuralNetwork>();
        for (int i = 0; i < NeuralParameters.NUM_ENTITIES; i++)
            Population.Add(new NeuralNetwork());

        GeneticAlgorithm = new GeneticAlgorithm(NeuralParameters.NUM_ENTITIES, Population[0].GetNumberOfWeights());

        for (int i = 0; i < NeuralParameters.NUM_ENTITIES; i++)
            Population[i].PutWeights(Genomes[i].Weights);
    }

    public NeuralNetworkPopulation(GeneticAlgorithm geneticAlgorithm, List<NeuralNetwork> population)
    {
        GeneticAlgorithm = geneticAlgorithm;
        Population = population;
    }

    public void Epoch()
    {
        GeneticAlgorithm.Epoch();
        
        for (int i = 0; i < Population.Count; ++i)
        {
            Population[i].PutWeights(Genomes[i].Weights);
            Population[i].Reset();
        }
    }

    public override bool Equals(object obj)
    {
        if (!(obj is NeuralNetworkPopulation))
        {
            return false;
        }

        var population = (NeuralNetworkPopulation) obj;
        return EqualityComparer<GeneticAlgorithm>.Default.Equals(GeneticAlgorithm, population.GeneticAlgorithm) &&
               EqualityComparer<List<NeuralNetwork>>.Default.Equals(Population, population.Population);
    }

    public override int GetHashCode()
    {
        var hashCode = -1597182661;
        hashCode = hashCode * -1521134295 + EqualityComparer<GeneticAlgorithm>.Default.GetHashCode(GeneticAlgorithm);
        hashCode = hashCode * -1521134295 + EqualityComparer<List<NeuralNetwork>>.Default.GetHashCode(Population);
        return hashCode;
    }

    public static bool operator ==(NeuralNetworkPopulation population1, NeuralNetworkPopulation population2)
    {
        return population1.Equals(population2);
    }

    public static bool operator !=(NeuralNetworkPopulation population1, NeuralNetworkPopulation population2)
    {
        return !(population1 == population2);
    }
}
