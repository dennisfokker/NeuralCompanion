using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AttackOpponentFighterController : FighterController
{
    public override void Awake()
    {
        base.Awake();
        
        healthChangeAction = UIController.Instance.UpdateOpponentHealth;
    }

    public override BattleAction GetAction(Dictionary<int, FighterController> fighters, Dictionary<int, BattleAction> previousFigherActions)
    {
        if (Target < 0)
            Target = getTarget(fighters);

        ActionType action = ActionType.ATTACK;

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
