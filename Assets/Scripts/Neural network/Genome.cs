using UnityEngine;

public struct Genome : System.IComparable
{
    public float[] Weights;
    public float Fitness;

    public Genome(int numInputs)
    {
        Weights = new float[numInputs];
        for (int i = 0; i < Weights.Length; i++)
        {
            Weights[i] = Random.Range(-1f, 1f);
        }
        Fitness = 0;
    }

    public Genome(float[] weights, float fitness)
    {
        Weights = weights;
        Fitness = fitness;
    }

    public int CompareTo(object obj)
    {
        if (!(obj is Genome))
            return 1;
        Genome g = (Genome) obj;
        if (Fitness == g.Fitness)
            return 0;
        else if (Fitness > g.Fitness)
            return -1;
        else
            return 1;
    }

    public static bool operator <(Genome a, Genome b)
    {
        return a.Fitness < b.Fitness;
    }

    public static bool operator >(Genome a, Genome b)
    {
        return a.Fitness > b.Fitness;
    }
}
