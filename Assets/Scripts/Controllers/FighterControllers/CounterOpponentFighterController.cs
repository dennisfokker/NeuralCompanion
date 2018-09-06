using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CounterOpponentFighterController : FighterController
{
    public int MaxDuration = 3;
    private int duration = 1;
    private BattleAction previousAction = BattleAction.NOTHING;

    new public void Awake()
    {
        base.Awake();

        healthChangeAction = UIController.Instance.UpdateOpponentHealth;
    }

    public override IEnumerator PerformTurn(Action<BattleAction> callback)
    {
        BattleAction action = BattleAction.NOTHING;
        duration--;
        if (duration > 0)
        {
            action = previousAction;
            callback(action);
            yield break;
        }

        if (PreviousOpponentsAction.Count != 0)
        {
            switch (PreviousOpponentsAction[0])
            {
                case BattleAction.ATTACK:
                    action = BattleAction.MAGIC;
                    break;
                case BattleAction.DEFEND:
                    action = BattleAction.HEAL;
                    break;
                case BattleAction.HEAL:
                    action = BattleAction.ATTACK;
                    break;
                case BattleAction.MAGIC:
                    action = BattleAction.DEFEND;
                    break;
                case BattleAction.NOTHING:
                    action = BattleAction.MAGIC;
                    break;
            }
            previousAction = action;
            duration = UnityEngine.Random.Range(1, MaxDuration + 1);

            callback(action);
            yield break;
        }

        callback(action);
        yield break;
    }
}
