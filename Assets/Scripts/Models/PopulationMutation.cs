using System;
using UnityEngine;

[Serializable]
public struct PopulationMutation
{
    public float Percentage;
    public float MutationRate;
    public float MaxPertubation;

    public PopulationMutation(float percentage, float mutationRate, float maxPertubation)
    {
        Percentage = percentage;
        MutationRate = mutationRate;
        MaxPertubation = maxPertubation;
    }

    public override bool Equals(object obj)
    {
        if (!(obj is PopulationMutation))
        {
            return false;
        }

        var mutation = (PopulationMutation) obj;
        return Percentage == mutation.Percentage &&
               MutationRate == mutation.MutationRate &&
               MaxPertubation == mutation.MaxPertubation;
    }

    public override int GetHashCode()
    {
        var hashCode = 555031021;
        hashCode = hashCode * -1521134295 + Percentage.GetHashCode();
        hashCode = hashCode * -1521134295 + MutationRate.GetHashCode();
        hashCode = hashCode * -1521134295 + MaxPertubation.GetHashCode();
        return hashCode;
    }

    public static bool operator ==(PopulationMutation mutation1, PopulationMutation mutation2)
    {
        return mutation1.Equals(mutation2);
    }

    public static bool operator !=(PopulationMutation mutation1, PopulationMutation mutation2)
    {
        return !(mutation1 == mutation2);
    }
}
