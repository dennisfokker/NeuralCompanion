using UnityEngine;

public struct Neuron
{
    public readonly float[] Weights;

    public Neuron(int numInputs)
    {
        Weights = new float[numInputs + 1];

        for (int i = 0; i < Weights.Length; i++)
        {
            Weights[i] = Random.Range(-1f, 1f);
        }
    }
}
