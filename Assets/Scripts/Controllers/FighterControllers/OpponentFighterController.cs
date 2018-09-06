using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OpponentFighterController : FighterController
{
    new public void Awake()
    {
        base.Awake();

        HealthText = GameObject.FindGameObjectWithTag("OpponentHealthText").GetComponent<Text>();
    }

    public override IEnumerator PerformTurn(Action<BattleAction> callback)
    {
        BattleAction action = BattleAction.HEAL;

        callback(action);
        yield break;
    }
}
