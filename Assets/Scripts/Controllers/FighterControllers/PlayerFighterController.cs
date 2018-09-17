using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerFighterController : FighterController
{
    private int target = -1;

    public override void Awake()
    {
        base.Awake();

        healthChangeAction = UIController.Instance.UpdatePlayerHealth;
    }

    public override IEnumerator PerformTurn(Dictionary<int, FighterController> fighters, Dictionary<int, BattleAction> previousFigherActions, Action<BattleAction> callback)
    {
        if (target < 0)
            target = getTarget(fighters);

        ActionType? action = GetPlayerInput();
        while (action == null)
        {
            action = GetPlayerInput();
            yield return null;
        }

        callback(new BattleAction(action ?? ActionType.NOTHING, target));
        yield break;
    }

    public override BattleAction GetAction(Dictionary<int, FighterController> fighters, Dictionary<int, BattleAction> previousActions)
    {
        throw new NotImplementedException();
    }

    private ActionType? GetPlayerInput()
    {
        if (Input.GetKeyUp(KeyCode.Q))
            return ActionType.MAGIC;
        if (Input.GetKeyUp(KeyCode.W))
            return ActionType.ATTACK;
        if (Input.GetKeyUp(KeyCode.A))
            return ActionType.HEAL;
        if (Input.GetKeyUp(KeyCode.S))
            return ActionType.DEFEND;
        return null;
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
