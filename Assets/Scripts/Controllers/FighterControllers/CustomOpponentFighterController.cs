using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomOpponentFighterController : FighterController
{
    public override string Identifier {
        get
        {
            return sequence;
        }
    }

    private string sequence = "HEAL";
    private List<ActionType> actionSequence = new List<ActionType>();
    private int currentIndex = 0;

    public override void Awake()
    {
        base.Awake();
        
        healthChangeAction = UIController.Instance.UpdateOpponentHealth;

        ParseSequence(UIController.Instance.GetCustomAISequence());
        if (actionSequence.Count <= 0)
            actionSequence = new List<ActionType>() { ActionType.HEAL };
    }

    public override BattleAction GetAction(Dictionary<int, FighterController> fighters, Dictionary<int, BattleAction> previousFigherActions)
    {
        if (Target < 0)
            Target = getTarget(fighters);

        ActionType action = actionSequence[currentIndex++];

        if (currentIndex >= actionSequence.Count)
            currentIndex = 0;

        return new BattleAction(action, Target);
    }

    private void ParseSequence(string rawSequence)
    {
        if (rawSequence.Length <= 0)
        {
            sequence = "HEAL";
            actionSequence = new List<ActionType>() { ActionType.HEAL };
            return;
        }
        rawSequence = rawSequence.ToUpper();

        actionSequence = new List<ActionType>();
        string[] parts = rawSequence.Split(',', '.', '|', '/', '\\', ':');
        for (int i = 0; i < parts.Length; i++)
        {
            string part = parts[i];
            part = part.Trim(' ', '\t', '\n');
            ActionType action = ActionType.NOTHING;

            switch (part)
            {
                case "Q":
                case "MGC":
                    action = ActionType.MAGIC;
                    break;
                case "W":
                case "ATK":
                    action = ActionType.ATTACK;
                    break;
                case "A":
                case "HEAL":
                    action = ActionType.HEAL;
                    break;
                case "S":
                case "DEF":
                    action = ActionType.DEFEND;
                    break;
            }
            if (action != ActionType.NOTHING)
            {
                actionSequence.Add(action);
                continue;
            }

            foreach(ActionType at in Enum.GetValues(typeof(ActionType)))
            {
                if (at.ToString().ToUpper().StartsWith(part))
                {
                    actionSequence.Add(at);
                    break;
                }
            }
        }

        if (actionSequence.Count <= 0)
        {
            sequence = "HEAL";
            actionSequence = new List<ActionType>() { ActionType.HEAL };
            return;
        }

        sequence = "";
        foreach (ActionType at in actionSequence)
        {
            sequence += at.ToString() + ",";
        }
        // Remove trailing comma
        sequence.Remove(sequence.Length - 1);
    }
    
    private int getTarget(Dictionary<int, FighterController> fighters)
    {
        foreach (KeyValuePair<int, FighterController> entry in fighters)
        {
            if (entry.Value == this)
                continue;

            return entry.Key;
        }

        return 0;
    }
}
