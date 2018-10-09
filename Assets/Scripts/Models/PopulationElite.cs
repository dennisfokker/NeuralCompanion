using System;
using System.Collections.Generic;
using UnityEngine;

public struct PopulationElite
{
    public string Identifier;
    public float[] Weights;

    public PopulationElite(string identifier, float[] weights)
    {
        Identifier = identifier;
        Weights = weights;
    }

    public override bool Equals(object obj)
    {
        if (!(obj is PopulationElite))
        {
            return false;
        }

        var elite = (PopulationElite) obj;
        return Identifier == elite.Identifier;
    }

    public override int GetHashCode()
    {
        var hashCode = 392502444;
        hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Identifier);
        hashCode = hashCode * -1521134295 + EqualityComparer<float[]>.Default.GetHashCode(Weights);
        return hashCode;
    }

    public static bool operator ==(PopulationElite elite1, PopulationElite elite2)
    {
        return elite1.Equals(elite2);
    }

    public static bool operator !=(PopulationElite elite1, PopulationElite elite2)
    {
        return !(elite1 == elite2);
    }
}
