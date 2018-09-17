using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeuralParameters : MonoBehaviour
{
    [Header("Brain size")]
    public int NumInputs = 7;
    public int NumHiddenLayers = 2;
    public int NeuronsPerHiddenLayer = 6;
    public int NumOutputs = 4;
    [Space(10)]

    [Header("Brain activity")]
    public float ActivationResponse = 1;
    public float Bias = -1;
    [Space(10)]

    [Header("Brain elasticity")]
    public float CrossoverRate = 0.7f;
    public float MutationRate = 0.1f;
    public float MaxPerturbation = 0.3f;
    [Space(10)]

    [Header("Simulation constraints")]
    public int NumEntities = 100;
    public int NumElite = 4;
    public int NumCopiesElite = 2;
    public int PercentExtinct = 40;
    public int MaxTurns = 30;

    public static int NUM_INPUTS { get; private set; }
    public static int NUM_HIDDEN_LAYERS { get; private set; }
    public static int NEURONS_PER_HIDDEN_LAYER { get; private set; }
    public static int NUM_OUTPUTS { get; private set; }
    public static float ACTIVATION_RESPONSE { get; private set; }
    public static float BIAS { get; private set; }
    public static float CROSSOVER_RATE { get; private set; }
    public static float MUTATION_RATE { get; private set; }
    public static float MAX_PERTURBATION { get; private set; }
    public static int NUM_ENTITIES { get; private set; }
    public static int NUM_ELITE { get; private set; }
    public static int NUM_COPIES_ELITE { get; private set; }
    public static int PERCENT_EXTINCT { get; private set; }
    public static int MAX_TURNS { get; private set; }

    void Awake ()
	{
        NUM_INPUTS = NumInputs;
        NUM_HIDDEN_LAYERS = NumHiddenLayers;
        NEURONS_PER_HIDDEN_LAYER = NeuronsPerHiddenLayer;
        NUM_OUTPUTS = NumOutputs;
        ACTIVATION_RESPONSE = ActivationResponse;
        BIAS = Bias;
        CROSSOVER_RATE = CrossoverRate;
        MUTATION_RATE = MutationRate;
        MAX_PERTURBATION = MaxPerturbation;
        NUM_ENTITIES = NumEntities; 
        NUM_ELITE = NumElite;
        NUM_COPIES_ELITE = NumCopiesElite;
        PERCENT_EXTINCT = PercentExtinct;
        MAX_TURNS = MaxTurns;
    }
}
