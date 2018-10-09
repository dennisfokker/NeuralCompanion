using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NeuralFighterController : FighterController
{
    public List<float> LastOutput = new List<float>();

    public NeuralNetwork NeuralNetwork { get; set; }
    public override string Identifier {
        get
        {
            return "neural";
        }
    }

    private Dictionary<int, List<BattleAction>> previousFighterActions = new Dictionary<int, List<BattleAction>>();
    private int self = -1;

    public override void Awake()
    {
        base.Awake();
        
        healthChangeAction = UIController.Instance.UpdatePlayerHealth;
    }

    public override BattleAction GetAction(Dictionary<int, FighterController> fighters, Dictionary<int, BattleAction> previousFigherActions)
    {
        if (Target < 0)
            Target = GetTarget(fighters);
        if (self < 0)
            self = getSelf(fighters);

        updatePreviousFighterActions(fighters, previousFigherActions);

        if (NeuralNetwork == null)
        {
            return new BattleAction(ActionType.NOTHING, Target);
        }

        List<float> opponentLastActions = new List<float>();
        List<BattleAction> opponentPreviousBattleActions = previousFighterActions[Target];
        for (int i = opponentPreviousBattleActions.Count - 1; i >= opponentPreviousBattleActions.Count - 5; i--)
        {
            if (i < 0)
            {
                opponentLastActions.Add((float) ActionType.NOTHING);
                continue;
            }

            opponentLastActions.Add((float) opponentPreviousBattleActions[i].ActionType);
        }
        List<float> ownLastActions = new List<float>();
        List<BattleAction> ownPreviousBattleActions = previousFighterActions[Target];
        for (int i = ownPreviousBattleActions.Count - 1; i >= ownPreviousBattleActions.Count - 5; i--)
        {
            if (i < 0)
            {
                ownLastActions.Add((float) ActionType.NOTHING);
                continue;
            }

            ownLastActions.Add((float) ownPreviousBattleActions[i].ActionType);
        }

        List<float> input = new List<float> { Health, fighters[Target]?.Health ?? 0f };
        input.AddRange(opponentLastActions);
        input.AddRange(ownLastActions);
        List<float> output = NeuralNetwork.Update(input);

        /*float total = 0;
        foreach (float f in output)
            total += f;
        Dictionary<ActionType, float> percentages = new Dictionary<ActionType, float>();
        for (int i = 1; i <= output.Count; i++)
        {
            percentages.Add((ActionType) i, output[i - 1] * (100 / total));
        }

        float rnd = UnityEngine.Random.Range(0f, 100f);
        float threshold = 0;
        ActionType action = ActionType.NOTHING;
        foreach (KeyValuePair<ActionType, float> entry in percentages)
        {
            threshold += entry.Value;
            if (rnd <= threshold)
            {
                action = entry.Key;
                break;
            }
        }*/

        int highest = 0;
        float highestValue = output[0];
        for (int i = 1; i < output.Count; i++)
        {
            if (output[i] <= highestValue)
                continue;

            highest = i;
            highestValue = output[i];
        }
        ActionType action = (ActionType) (highest + 1);
        LastOutput = output;

        return new BattleAction(action, Target);
    }

    public int GetTarget(Dictionary<int, FighterController> fighters)
    {
        foreach (KeyValuePair<int, FighterController> entry in fighters)
        {
            if (entry.Value == this)
                continue;

            return entry.Key;
        }

        return 0;
    }

    private void updatePreviousFighterActions(Dictionary<int, FighterController> fighters, Dictionary<int, BattleAction> previousFigherActions)
    {
        if (previousFighterActions.Count < fighters.Count)
        {
            foreach (KeyValuePair<int, FighterController> entry in fighters)
            {
                if (!previousFighterActions.ContainsKey(entry.Key))
                    previousFighterActions.Add(entry.Key, new List<BattleAction>());
            }

            return;
        }

        foreach (KeyValuePair<int, BattleAction> entry in previousFigherActions)
        {
            if (previousFighterActions.ContainsKey(entry.Key))
                previousFighterActions[entry.Key].Add(entry.Value);
            else
                previousFighterActions.Add(entry.Key, new List<BattleAction>() { entry.Value });
        }
    }

    private int getSelf(Dictionary<int, FighterController> fighters)
    {
        foreach (KeyValuePair<int, FighterController> entry in fighters)
        {
            if (entry.Value == this)
                return entry.Key;
        }

        return 0;
    }
}