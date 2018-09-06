using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealOpponentFighterController : FighterController
{
    new public void Awake()
    {
        base.Awake();

        healthChangeAction = UIController.Instance.UpdateOpponentHealth;
    }

    public override IEnumerator PerformTurn(Action<BattleAction> callback)
    {
        BattleAction action = BattleAction.HEAL;

        callback(action);
        yield break;
    }
}
