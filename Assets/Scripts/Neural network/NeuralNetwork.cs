using System.Collections.Generic;
using UnityEngine;

public class NeuralNetwork
{
    public float Fitness { get; set; }

    private List<NeuronLayer> layers = new List<NeuronLayer>();

    public NeuralNetwork()
    {
        CreateNetwork(NeuralParameters.NUM_INPUTS, NeuralParameters.NUM_OUTPUTS, NeuralParameters.NUM_HIDDEN_LAYERS, NeuralParameters.NEURONS_PER_HIDDEN_LAYER);
    }

    public NeuralNetwork(int numInputs, int numOutputs, int numHiddenLayers, int neuronsPerHiddenLayer)
    {
        CreateNetwork(numInputs, numOutputs, numHiddenLayers, neuronsPerHiddenLayer);
    }

    public List<List<List<float>>> GetWeights()
    {
        List<List<List<float>>> weights = new List<List<List<float>>>();
        
        // Each layer
        foreach (NeuronLayer layer in layers)
        {
            List<List<float>> layerWeights = new List<List<float>>();

            // Each layer's neuron
            foreach (Neuron neuron in layer.Neurons)
            {
                // Add weights of neuron
                layerWeights.Add(new List<float>(neuron.Weights));
            }

            weights.Add(layerWeights);
        }

        return weights;
    }

    public int GetNumberOfWeights()
    {
        int count = 0;

        // Each layer
        foreach (NeuronLayer layer in layers)
        {
            // Each layer's neuron
            foreach (Neuron neuron in layer.Neurons)
            {
                count += neuron.Weights.Length;
            }
        }

        return count;
    }

    public void PutWeights(List<List<List<float>>> weights)
    {
        // Each layer
        for (int i = 0; i < layers.Count; i++)
        {
            // Each layer's neuron
            for (int j = 0; j < layers[i].Neurons.Length; j++)
            {
                // Insert weights for this neuron
                weights[i][j].CopyTo(layers[i].Neurons[j].Weights);
            }
        }
    }

    public void PutWeights(float[] weights)
    {
        int weight = 0;

        // Each layer
        for (int i = 0; i < layers.Count; ++i)
        {
            // Each layer's neuron
            for (int j = 0; j < layers[i].Neurons.Length; ++j)
            {
                // Insert weights for this neuron
                for (int k = 0; k < layers[i].Neurons[j].Weights.Length; ++k)
                {
                    layers[i].Neurons[j].Weights[k] = weights[weight++];
                }
            }
        }
    }

    public List<float> Update(List<float> inputs)
    {
        List<float> outputs = new List<float>();
        
        // Error check
        if (inputs.Count != layers[0].Neurons[0].Weights.Length - 1)
        {
            return null;
        }

        // Each layer
        for (int i = 0; i < layers.Count; ++i)
        {
            // Update input (output of previous layer)
            if (i > 0)
            {
                inputs = new List<float>(outputs);
            }
            outputs.Clear();

            // Each layer's neuron
            for (int j = 0; j < layers[i].Neurons.Length; ++j)
            {
                float netinput = 0;

                int NumInputs = layers[i].Neurons[j].Weights.Length;

                // Sum the weights x inputs
                for (int k = 0; k < NumInputs - 1; ++k)
                {
                    netinput += layers[i].Neurons[j].Weights[k] * inputs[k];
                }

                // Add in the bias
                netinput += layers[i].Neurons[j].Weights[NumInputs - 1] * NeuralParameters.BIAS;

                // Add neuron's output (put through sigmoid function)
                outputs.Add(Sigmoid(netinput, NeuralParameters.ACTIVATION_RESPONSE));
            }
        }

        return outputs;
    }

    public float Sigmoid(float activation, float response)
    {
        return 1 / (1 + Mathf.Exp(-activation / response));
    }

    public void Reset()
    {
        Fitness = 0;
    }

    private void CreateNetwork(int numInputs, int numOutputs, int numHiddenLayers, int neuronsPerHiddenLayer)
    {
        // Check if single layer
        if (numHiddenLayers > 0)
        {
            // Make first layer (bit different than other hidden layers)
            layers.Add(new NeuronLayer(neuronsPerHiddenLayer, numInputs));

            // Make rest of hidden layers
            for (int i = 1; i < numHiddenLayers; ++i)
            {
                layers.Add(new NeuronLayer(neuronsPerHiddenLayer, neuronsPerHiddenLayer));
            }

            layers.Add(new NeuronLayer(numOutputs, neuronsPerHiddenLayer));
        }
        else
        {
            layers.Add(new NeuronLayer(numOutputs, numInputs));
        }
    }
}