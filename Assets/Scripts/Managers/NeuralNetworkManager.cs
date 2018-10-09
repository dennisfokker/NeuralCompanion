using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NeuralNetworkManager : MonoBehaviour
{
    private Dictionary<int, NeuralNetworkPopulation> fighterPopulations = new Dictionary<int, NeuralNetworkPopulation>();
    private List<PopulationElite> previousElite = new List<PopulationElite>(10);

    private float[] previousBest = new float[0];

    void Start()
    {
        GameController.RoundStart += OnRoundStart;
    }

    public void OnRoundStart(Dictionary<int, FighterController> fighters)
    {
        foreach (KeyValuePair<int, FighterController> entry in fighters)
        {
            if (!(entry.Value is NeuralFighterController))
                continue;

            initializePopulation(entry.Key);

            NeuralFighterController nfc = (NeuralFighterController) entry.Value;
            nfc.NeuralNetwork = GetLastFittestNetwork(entry.Key);

            // First update the previous elite, then we can run another generation.
            string fighterIdentifier = fighters[nfc.GetTarget(fighters)].Identifier;
            float[] weights = fighterPopulations[entry.Key].FittestGenome.Weights;
            addPreviousElite(fighterIdentifier, weights);

            GameManager.Instance.StartCoroutine(DoGeneration(entry.Key, fighters[nfc.GetTarget(fighters)]));
        }
    }

    public IEnumerator DoGeneration(int identifier, FighterController opponentFighterController)
    {
        int simulationsPerFrame = 5;
        int simulationsLeft = 5;

        NeuralNetworkPopulation nnp = fighterPopulations[identifier];
        for (int i = 0; i < nnp.Population.Count; i++)
        {
            int turn = 0;
            NeuralFighterController fighter = GameManager.Instance.gameObject.AddComponent<NeuralFighterController>();
            FighterController opponentFighter = (FighterController) GameManager.Instance.gameObject.AddComponent(opponentFighterController.GetType());

            Dictionary<int, FighterController> fighters = new Dictionary<int, FighterController> { { 0, fighter }, { 1, opponentFighter } };
            Dictionary<int, BattleAction> previousFigherActions = new Dictionary<int, BattleAction> { { 0, new BattleAction(ActionType.NOTHING, 1) }, { 1, new BattleAction(ActionType.NOTHING, 0) } };
            fighter.NeuralNetwork = nnp.Population[i];

            fighter.Awake();
            fighter.Start();
            opponentFighter.Awake();
            opponentFighter.Start();

            while (fighter.Health > 0 && opponentFighter.Health > 0 && turn < NeuralParameters.MAX_TURNS)
            {
                BattleAction fighterAction = fighter.GetAction(fighters, previousFigherActions);
                BattleAction opponentAction = opponentFighter.GetAction(fighters, previousFigherActions);

                previousFigherActions[0] = fighterAction;
                previousFigherActions[1] = opponentAction;

                fighter.Health -= getReceivedDamage(fighterAction, opponentAction);
                opponentFighter.Health -= getReceivedDamage(opponentAction, fighterAction);

                turn++;
            }
            if (opponentFighter.Health < 0)
                opponentFighter.Health = 0;
            if (fighter.Health < 0)
                fighter.Health = 0;

            float fitness = fighter.Health - opponentFighter.Health - turn;
            if (opponentFighter.Health > 0 && fighter.Health > 0)
            {
                if (fitness > 0)
                    fitness *= NeuralParameters.POSITIVE_OUT_OF_TIME_MULTIPLIER;
                else
                    fitness *= NeuralParameters.NEGATIVE_OUT_OF_TIME_MULTIPLIER;
            }
            else if (opponentFighter.Health > 0 && fighter.Health <= 0)
            {
                fitness *= NeuralParameters.LOSE_MULTIPLIER;
            }

            nnp.Population[i].Fitness = fitness;
            Genome genome = nnp.GeneticAlgorithm.Population[i];
            genome.Fitness = fitness;
            nnp.GeneticAlgorithm.Population[i] = genome;

            Destroy(fighter);
            Destroy(opponentFighter);

            if (--simulationsLeft < 0)
            {
                yield return null;
                simulationsLeft = simulationsPerFrame;
            }
        }
        
        nnp.Epoch();
        nnp.EliteEpoch(previousElite);
        Debug.Log(string.Format("Best fitness: {0} - Average fitness: {1} - Worst fitness: {2}", nnp.BestFitness, Mathf.Round(nnp.AverageFitness * 100f) / 100f, nnp.WorstFitness));
        if (GameManager.Instance.DebugRun)
            Debug.Break();
    }

    public NeuralNetwork GetLastFittestNetwork(int identifier)
    {
        if (!fighterPopulations.ContainsKey(identifier))
            initializePopulation(identifier);

        return fighterPopulations[identifier].FittestNeuralNetwork;
    }

    private void initializePopulation(int identifier)
    {
        if (fighterPopulations.ContainsKey(identifier))
            return;

        NeuralNetworkPopulation nnp = new NeuralNetworkPopulation();
        fighterPopulations.Add(identifier, nnp);
    }

    private float getReceivedDamage(BattleAction fighterAction, BattleAction opponentAction)
    {
        BattleActionResults bars = DataManager.Instance.BattleActionResults[fighterAction.ActionType];
        float receivedDamage = 0;
        foreach (var bar in bars.BattleActionResultList)
            if (bar.ActionType == opponentAction.ActionType)
                receivedDamage = bar.ReceivingDamage;

        return receivedDamage;
    }

    private void addPreviousElite(string identifier, float[] weights)
    {
        for (int i = 0; i < previousElite.Count; i++)
        {
            if (previousElite[i].Identifier == identifier)
            {
                previousElite[i] = new PopulationElite(identifier, weights);
                return;
            }
        }

        if (previousElite.Count >= 10)
        {
            previousElite.RemoveAt(0);
        }

        previousElite.Add(new PopulationElite(identifier, weights));
    }
}
