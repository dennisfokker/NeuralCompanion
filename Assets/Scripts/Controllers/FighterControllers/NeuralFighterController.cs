using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NeuralFighterController : FighterController
{
    public NeuralNetwork NeuralNetwork { get; set; }

    private Dictionary<int, List<BattleAction>> previousOpponentActions = new Dictionary<int, List<BattleAction>>();

    public override void Awake()
    {
        base.Awake();
        
        healthChangeAction = UIController.Instance.UpdatePlayerHealth;
    }

    public override BattleAction GetAction(Dictionary<int, FighterController> fighters, Dictionary<int, BattleAction> previousFigherActions)
    {
        if (Target < 0)
            Target = GetTarget(fighters);

        updateOpponentPreviousActions(fighters, previousFigherActions);

        if (NeuralNetwork == null)
        {
            return new BattleAction(ActionType.NOTHING, Target);
        }

        List<float> opponentLastActions = new List<float>();
        List<BattleAction> opponentPreviousBattleActions = previousOpponentActions[Target];
        for (int i = opponentPreviousBattleActions.Count - 1; i >= opponentPreviousBattleActions.Count - 5; i--)
        {
            if (i < 0)
            {
                opponentLastActions.Add((float) ActionType.NOTHING);
                continue;
            }

            opponentLastActions.Add((float) opponentPreviousBattleActions[i].ActionType);
        }

        List<float> input = new List<float> { Health, GameController.FighterControllers[Target]?.Health ?? 0f };
        input.AddRange(opponentLastActions);
        List<float> output = NeuralNetwork.Update(input);

        float total = 0;
        foreach (float f in output)
            total += f;
        Dictionary<ActionType, float> percentages = new Dictionary<ActionType, float>();
        for (int i = 1; i <= output.Count; i++)
        {
            percentages.Add((ActionType) i, output[i - 1] * (100 / total));
            //Debug.Log("Output for " + ((ActionType) i).ToString() + " = " + output[i - 1] + "% (" + output[i - 1] * (100 / total) + "%)");
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
        }
        //Debug.Log("rnd = " + rnd);

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

    private void updateOpponentPreviousActions(Dictionary<int, FighterController> fighters, Dictionary<int, BattleAction> previousFigherActions)
    {
        if (previousOpponentActions.Count < fighters.Count)
        {
            foreach (KeyValuePair<int, FighterController> entry in fighters)
            {
                if (!previousOpponentActions.ContainsKey(entry.Key))
                    previousOpponentActions.Add(entry.Key, new List<BattleAction>());
            }

            return;
        }

        foreach (KeyValuePair<int, BattleAction> entry in previousFigherActions)
        {
            if (previousOpponentActions.ContainsKey(entry.Key))
                previousOpponentActions[entry.Key].Add(entry.Value);
            else
                previousOpponentActions.Add(entry.Key, new List<BattleAction>() { entry.Value });
        }
    }
}