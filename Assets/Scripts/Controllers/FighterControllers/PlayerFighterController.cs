using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerFighterController : FighterController
{
    new public void Awake()
    {
        base.Awake();

        HealthText = GameObject.FindGameObjectWithTag("PlayerHealthText").GetComponent<Text>();
    }

    public override IEnumerator PerformTurn(Action<BattleAction> callback)
    {
        BattleAction? action = GetPlayerInput();
        while (action == null)
        {
            action = GetPlayerInput();
            yield return null;
        }

        callback((BattleAction) action);
        yield break;
    }

    private BattleAction? GetPlayerInput()
    {
        if (Input.GetKeyUp(KeyCode.Q))
            return BattleAction.MAGIC;
        if (Input.GetKeyUp(KeyCode.W))
            return BattleAction.ATTACK;
        if (Input.GetKeyUp(KeyCode.A))
            return BattleAction.HEAL;
        if (Input.GetKeyUp(KeyCode.S))
            return BattleAction.DEFEND;
        return null;
    }
}
