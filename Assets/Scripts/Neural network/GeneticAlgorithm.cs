using System.Collections.Generic;
using UnityEngine;

public class GeneticAlgorithm
{
    public List<Genome> Population
    {
        get
        {
            return population;
        }
    }
    public float AverageFitness { get { return TotalFitness / Population.Count; } }
    public float TotalFitness { get; private set; }
    public float BestFitness { get; private set; }
    public float WorstFitness { get; private set; }
    public int FittestGenomeIndex { get; private set; }
    public int Generation { get; private set; }

    private List<Genome> population;

    public GeneticAlgorithm(int populationSize, int numWeights)
    {
        population = new List<Genome>();

        for (int i = 0; i < populationSize; i++)
        {
            Population.Add(new Genome(numWeights));
        }
    }

    public List<Genome> Epoch(ref List<Genome> oldPopulation)
    {
        // Hold original population size
        int populationSize = population.Count;

        // Assign the given population to the classes population
        population = oldPopulation;

        // Reset the appropriate variables
        reset();

        // Sort the population (for scaling and elitism)
        Population.Sort();

        // Calculate best, worst, average and total fitness
        calculateBestWorstAvgTot();

        // Kill certain percentage
        int count = (int) Mathf.Round(NeuralParameters.PERCENT_EXTINCT / (float) populationSize * 100);
        Population.RemoveRange(0, count);

        // Create a temporary vector to store new chromosones
        List<Genome> NewPopulation = new List<Genome>();

        // Add a little elitism by adding in some copies of the fittest genomes. Has to be an even number.
        if ((NeuralParameters.NUM_COPIES_ELITE * NeuralParameters.NUM_ELITE) % 2 == 0)
        {
            grabNBest(NeuralParameters.NUM_ELITE, NeuralParameters.NUM_COPIES_ELITE, ref NewPopulation);
        }
        
        // Repeat until a new population is generated
        while (NewPopulation.Count < populationSize)
        {
            // Grab two chromosones
            Genome mum = getChromosomeRoulette();
            Genome dad = getChromosomeRoulette();

            // Create some offspring via crossover
            List<float> baby1 = new List<float>();
            List<float> baby2 = new List<float>();

            crossover(mum.Weights, dad.Weights, ref baby1, ref baby2);

            // Mutate
            mutate(baby1);
            mutate(baby2);

            // Copy into new population
            NewPopulation.Add(new Genome(baby1.ToArray(), 0));
            NewPopulation.Add(new Genome(baby2.ToArray(), 0));
        }

        // Finished: Assign new pop back into original pop
        population = NewPopulation;

        Generation++;

        return Population;
    }

    public List<Genome> Epoch()
    {
        return Epoch(ref population);
    }

    private void crossover(float[] mom, float[] dad, ref List<float> baby1, ref List<float> baby2)
    {
        // Just return parents as offspring dependent on the rate or if parents are the same
        if (Random.value > NeuralParameters.CROSSOVER_RATE || mom == dad)
        {
            baby1 = new List<float>(mom);
            baby2 = new List<float>(dad);

            return;
        }

        // Determine a crossover point
        int cp = Random.Range(0, Population[0].Weights.Length - 1);

        // Create the offspring
        for (int i = 0; i < cp; ++i)
        {
            baby1.Add(mom[i]);
            baby2.Add(dad[i]);
        }

        for (int i = cp; i < mom.Length; ++i)
        {
            baby1.Add(dad[i]);
            baby2.Add(mom[i]);
        }


        return;
    }

    private List<float> mutate(List<float> chromosome)
    {
        // Traverse the chromosome and mutate each weight dependent on the mutation rate
        for (int i = 0; i < chromosome.Count; ++i)
        {
            // Do we perturb this weight?
            if (Random.value < NeuralParameters.MUTATION_RATE)
            {
                // Add or subtract a small value to the weight
                chromosome[i] += (Random.Range(-1f, 1f) * NeuralParameters.MAX_PERTURBATION);
            }
        }

        return chromosome;
    }

    private Genome getChromosomeRoulette()
    {
        // Generate a random number between 0 & total fitness count
        float Slice = Random.value * TotalFitness;

        // This will be set to the chosen chromosome
        Genome TheChosenOne = population[0];

        // Go through the chromosones adding up the fitness so far
        double FitnessSoFar = 0;

        for (int i = 0; i < Population.Count; ++i)
        {
            FitnessSoFar += population[i].Fitness;

            // If the fitness so far > random number return the chromo at this point
            if (FitnessSoFar >= Slice)
            {
                TheChosenOne = Population[i];

                break;
            }
        }

        return TheChosenOne;
    }

    private void grabNBest(int nBest, int numCopies, ref List<Genome> population)
    {
        int populationSize = Population.Count;

        // Add the required amount of copies of the n most fittest to the supplied vector
        while (--nBest >= 0)
        {
            for (int i = 0; i < numCopies; ++i)
            {
                population.Add(Population[(populationSize - 1) - nBest]);
            }
        }
    }

    private void calculateBestWorstAvgTot()
    {
        TotalFitness = 0;

        float HighestSoFar = float.MinValue;
        float LowestSoFar = float.MaxValue;

        for (int i = 0; i < Population.Count; ++i)
        {
            //update fittest if necessary
            if (Population[i].Fitness > HighestSoFar)
            {
                HighestSoFar = Population[i].Fitness;

                FittestGenomeIndex = i;

                BestFitness = HighestSoFar;
            }

            //update worst if necessary
            if (Population[i].Fitness < LowestSoFar)
            {
                LowestSoFar = Population[i].Fitness;

                WorstFitness = LowestSoFar;
            }

            TotalFitness += Population[i].Fitness;
        }
    }

    private void reset()
    {
        TotalFitness = 0;
        BestFitness = 0;
        WorstFitness = float.MaxValue;
    }
}