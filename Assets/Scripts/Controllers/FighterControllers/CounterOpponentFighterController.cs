using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CounterOpponentFighterController : FighterController
{
    public int MaxDuration = 3;

    private int duration = 1;
    private ActionType previousAction = ActionType.NOTHING;

    public override void Awake()
    {
        base.Awake();
        
        healthChangeAction = UIController.Instance.UpdateOpponentHealth;
    }

    public override BattleAction GetAction(Dictionary<int, FighterController> fighters, Dictionary<int, BattleAction> previousFigherActions)
    {
        if (Target < 0)
            Target = getTarget(fighters);

        ActionType action = ActionType.NOTHING;
        duration--;
        if (duration > 0)
        {
            action = previousAction;
            return new BattleAction(action, Target);
        }

        switch (previousFigherActions[Target].ActionType)
        {
            case ActionType.ATTACK:
                action = ActionType.MAGIC;
                break;
            case ActionType.DEFEND:
                action = ActionType.HEAL;
                break;
            case ActionType.HEAL:
                action = ActionType.ATTACK;
                break;
            case ActionType.MAGIC:
                action = ActionType.DEFEND;
                break;
            case ActionType.NOTHING:
                action = ActionType.MAGIC;
                break;
        }

        previousAction = action;
        duration = UnityEngine.Random.Range(1, MaxDuration + 1);

        return new BattleAction(action, Target);
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
