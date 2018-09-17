public struct NeuronLayer
{
    public readonly Neuron[] Neurons;

    public NeuronLayer(int numNeurons, int numInputsPerNeuron)
    {
        Neurons = new Neuron[numNeurons];

        for (int i = 0; i < Neurons.Length; i++)
        {
            Neurons[i] = new Neuron(numInputsPerNeuron);
        }
    }
}
