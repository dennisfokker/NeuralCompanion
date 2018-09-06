using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public float TurnCooldownSeconds = 2f;

    public List<FighterController> FighterControllers { get; set; }

    private List<Tuple<BattleAction, FighterController>> BattleActions = new List<Tuple<BattleAction, FighterController>>();
    private int FighterCount;
    private bool TurnInProgress = false;

    void Start()
    {
        if (FighterControllers == null)
            FighterControllers = new List<FighterController>();
    }

    void Update()
    {
        if (TurnInProgress)
            return;

        bool roundOver = false;
        foreach (FighterController fc in FighterControllers)
        {
            if (fc.Health < 0)
            {
                roundOver = true;
                break;
            }
        }

        if (!roundOver)
        {
            RequestActions();
            return;
        }


    }

    private void RequestActions()
    {
        TurnInProgress = true;
        FighterCount = FighterControllers.Count;
        foreach (FighterController fc in FighterControllers)
        {
            StartCoroutine(fc.PerformTurn(ba => ProcessAction(ba, fc)));
        }
    }

    private void ProcessAction(BattleAction ba, FighterController fc)
    {
        BattleActions.Add(new Tuple<BattleAction, FighterController>(ba, fc));

        if (BattleActions.Count == FighterCount)
            ProcessTurn();
    }

    private void ProcessTurn()
    {
        for (int i = 0; i < BattleActions.Count; i++)
        {
            var entry = BattleActions[i];
            entry.Item2.ActionRenderer.sprite = DataManager.Instance.BattleActionIcons[entry.Item1];

            for (int j = 0; j < BattleActions.Count; j++)
            {
                if (j == i)
                    continue;

                BattleActionResults bars = DataManager.Instance.BattleActionResults[entry.Item1];
                float receivedDamage = 0;
                foreach (var bar in bars.BattleActionResultList)
                    if (bar.BattleAction == BattleActions[j].Item1)
                        receivedDamage = bar.ReceivingDamage;

                entry.Item2.Health -= receivedDamage;
            }
        }

        DelayUtil.WaitForSeconds(TurnCooldownSeconds, () =>
        {
            foreach (var entry in BattleActions)
            {
                entry.Item2.ClearActionIcon();
            }
            BattleActions = new List<Tuple<BattleAction, FighterController>>();
            TurnInProgress = false;
        });
    }
}
