using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeuralNetworkManager : MonoBehaviour
{
    private Dictionary<int, NeuralNetworkPopulation> fighterPopulations = new Dictionary<int, NeuralNetworkPopulation>();

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

            GameManager.Instance.StartCoroutine(DoGeneration(entry.Key, fighters[nfc.GetTarget(fighters)]));
        }
    }

    public IEnumerator DoGeneration(int identifier, FighterController opponentFighterController)
    {
        Debug.Log("Training...");

        NeuralNetworkPopulation nnp = fighterPopulations[identifier];
        NeuralFighterController fighter = GameManager.Instance.gameObject.AddComponent<NeuralFighterController>();
        FighterController opponentFighter = (FighterController) GameManager.Instance.gameObject.AddComponent(opponentFighterController.GetType());

        Dictionary<int, FighterController> fighters = new Dictionary<int, FighterController> { { 0, fighter }, { 1, opponentFighter } };
        Dictionary<int, BattleAction> previousFigherActions = new Dictionary<int, BattleAction> { { 0, new BattleAction(ActionType.NOTHING, 1) }, { 1, new BattleAction(ActionType.NOTHING, 0) } };
        for (int i = 0; i < nnp.Population.Count; i++)
        {
            int turn = 0;
            float fighterHealth = GameManager.Instance.MaxHealth;
            float opponentHealth = GameManager.Instance.MaxHealth;
            fighter.NeuralNetwork = nnp.Population[i];

            while (fighterHealth > 0 && opponentHealth > 0 && turn < NeuralParameters.MAX_TURNS)
            {
                BattleAction fighterAction = fighter.GetAction(fighters, previousFigherActions);
                BattleAction opponentAction = opponentFighter.GetAction(fighters, previousFigherActions);

                previousFigherActions[0] = fighterAction;
                previousFigherActions[1] = opponentAction;

                fighterHealth -= getReceivedDamage(fighterAction, opponentAction);
                opponentHealth -= getReceivedDamage(opponentAction, fighterAction);

                turn++;
            }

            float fitness = fighterHealth - opponentHealth;
            if (opponentHealth > 0 && fighterHealth > 0)
            {
                if (fitness > 0)
                    fitness *= 0.5f;
                else
                    fitness *= 1.5f;
            }
            else if (opponentHealth > 0 && fighterHealth <= 0)
            {
                fitness = float.MinValue;
            }

            nnp.Population[i].Fitness = fitness;
            Genome genome = nnp.GeneticAlgorithm.Population[i];
            genome.Fitness = fitness;
            nnp.GeneticAlgorithm.Population[i] = genome;

            yield return null;
        }
        
        nnp.Epoch();
        Destroy(fighter);
        Debug.Log("Finished training!");
        Debug.Log(string.Format("Best fitness: {0} - Average fitness: {1} - Worst fitness: {2}", nnp.BestFitness, Mathf.Round(nnp.AverageFitness * 100f) / 100f, nnp.WorstFitness));
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
}
