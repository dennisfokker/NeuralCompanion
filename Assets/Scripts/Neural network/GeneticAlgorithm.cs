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
        // Assign the given population to the classes population
        population = oldPopulation;

        // Reset the appropriate variables
        reset();
        
        // Sort the population (for scaling and elitism)
        Population.Sort();

        // Calculate total fitness
        calculateBestWorstTotalFitness();

        // Kill certain percentage
        //int count = (int) Mathf.Round(NeuralParameters.PERCENT_EXTINCT / (float) population.Count * 100);
        //Population.RemoveRange(Population.Count - 1 - count, count);

        // Create new population
        List<Genome> NewPopulation = createPopulation();

        // Finished: Assign new pop back into original pop
        population = NewPopulation;

        Generation++;

        return population;
    }

    public List<Genome> Epoch()
    {
        return Epoch(ref population);
    }

    public List<Genome> EliteEpoch(List<PopulationElite> previousElite)
    {
        List<Genome> eliteGenomes = new List<Genome>();

        // Loop over all the available elite
        foreach (PopulationElite pe in previousElite)
        {
            // Add original. Maybe it's good enough.
            eliteGenomes.Add(new Genome(pe.Weights, 0));

            float[] bestWeights = population[0].Weights;

            // For every mutation rate, add 2 crossovers (2 per crossover)
            foreach (PopulationMutation pm in NeuralParameters.POPULATION_MUTATIONS)
            {
                // Add original with actual elite crossover x2 (1x provides 2 entities)
                for (int i = 0; i < 2; i++)
                {
                    float[] baby1 = new float[bestWeights.Length];
                    float[] baby2 = new float[bestWeights.Length];
                    crossover(pe.Weights, bestWeights, ref baby1, ref baby2, NeuralParameters.CROSSOVER_RATE);

                    // Mutate.
                    mutate(ref baby1, pm.MutationRate, pm.MaxPertubation);
                    mutate(ref baby2, pm.MutationRate, pm.MaxPertubation);

                    eliteGenomes.Add(new Genome(baby1, 0));
                    eliteGenomes.Add(new Genome(baby2, 0));
                }
            }
        }

        population.AddRange(eliteGenomes);
        return eliteGenomes;
    }

    private List<Genome> createPopulation()
    {
        List<Genome> population = new List<Genome>();

        /*
        // Add a little elitism by adding in some copies of the fittest genomes. Has to be an even number.
        if ((NeuralParameters.NUM_COPIES_ELITE * NeuralParameters.NUM_ELITE) % 2 == 0)
        {
            grabNBest(NeuralParameters.NUM_ELITE, NeuralParameters.NUM_COPIES_ELITE, ref population);
        }

        // Repeat until a new population is generated
        while (population.Count < NeuralParameters.NUM_ENTITIES)
        {
            // Grab two chromosones
            Genome mom = getChromosomeRoulette();
            Genome dad = getChromosomeRoulette();

            // Create some offspring via crossover
            List<float> baby1 = new List<float>();
            List<float> baby2 = new List<float>();

            crossover(mom.Weights, dad.Weights, ref baby1, ref baby2, NeuralParameters.CROSSOVER_RATE);

            // Mutate
            mutate(ref baby1, NeuralParameters.MUTATION_RATE, NeuralParameters.MAX_PERTURBATION);
            mutate(ref baby2, NeuralParameters.MUTATION_RATE, NeuralParameters.MAX_PERTURBATION);

            // Copy into new population
            population.Add(new Genome(baby1, 0));
            population.Add(new Genome(baby2, 0));
        }
        */

        float[] bestWeights = this.population[0].Weights;
        foreach (PopulationMutation pm in NeuralParameters.POPULATION_MUTATIONS)
        {
            for (int i = 0; i < NeuralParameters.NUM_ENTITIES / 100 * pm.Percentage; i++)
            {
                float[] currentWeights = new float[bestWeights.Length];
                bestWeights.CopyTo(currentWeights, 0);
                mutate(ref currentWeights, pm.MutationRate, pm.MutationRate);
                population.Add(new Genome(currentWeights, 0));
            }
        }

        for (int i = population.Count; i < NeuralParameters.NUM_ENTITIES; i++)
        {
            population.Add(new Genome(bestWeights.Length));
        }

        return population;
    }

    private void crossover(float[] mom, float[] dad, ref float[] baby1, ref float[] baby2, float crossoverRate)
    {
        // Just return parents as offspring dependent on the rate or if parents are the same
        if (Random.value > crossoverRate || mom == dad)
        {
            mom.CopyTo(baby1, 0);
            dad.CopyTo(baby2, 0);

            return;
        }

        // Determine a crossover point
        int cp = Random.Range(0, Population[0].Weights.Length - 1);
        baby1 = new float[mom.Length];
        baby2 = new float[mom.Length];

        // Create the offspring
        for (int i = 0; i < cp; ++i)
        {
            baby1[i] = mom[i];
            baby2[i] = dad[i];
        }

        for (int i = cp; i < mom.Length; ++i)
        {
            baby1[i] = dad[i];
            baby2[i] = mom[i];
        }
    }

    private void mutate(ref float[] chromosome, float mutationRate, float maxPerturbation)
    {
        // Traverse the chromosome and mutate each weight dependent on the mutation rate
        for (int i = 0; i < chromosome.Length; ++i)
        {
            // Do we perturb this weight?
            if (Random.value < mutationRate)
            {
                // Add or subtract a small value to the weight
                chromosome[i] += (Random.Range(-1f, 1f) * maxPerturbation);
            }
        }
    }

    private Genome getChromosomeRoulette()
    {
        // Get scaled total fitness
        float scaledTotalFitness = TotalFitness - Population.Count * WorstFitness;
        // Generate a random number between 0 & total fitness count
        float slice = Random.value * scaledTotalFitness;

        // Go through the chromosones adding up the fitness so far. Favour fitter chromosones.
        double FitnessSoFar = 0;
        for (int i = 0; i < Population.Count; ++i)
        {
            FitnessSoFar += Population[i].Fitness - WorstFitness;

            // If the fitness so far > random number return the chromo at this point
            if (FitnessSoFar >= slice)
            {
                return Population[i];
            }
        }

        return Population[0];
    }

    private void grabNBest(int nBest, int numCopies, ref List<Genome> population)
    {
        int populationSize = Population.Count;

        // Add the required amount of copies of the n most fittest to the supplied vector
        while (--nBest >= 0)
        {
            for (int i = 0; i < numCopies; ++i)
            {
                population.Add(Population[nBest]);
            }
        }
    }

    private void calculateBestWorstTotalFitness()
    {
        BestFitness = Population[0].Fitness;
        WorstFitness = Population[Population.Count - 1].Fitness;

        TotalFitness = 0;
        for (int i = 0; i < Population.Count; ++i)
        {
            TotalFitness += Population[i].Fitness;
        }
    }

    private void reset()
    {
        TotalFitness = 0;
    }
}