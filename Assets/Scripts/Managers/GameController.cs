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
        
        RequestActions();
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
            entry.Item2.PreviousOpponentsAction = new List<BattleAction>();

            for (int j = 0; j < BattleActions.Count; j++)
            {
                if (j == i)
                    continue;

                entry.Item2.PreviousOpponentsAction.Add(BattleActions[j].Item1);

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

            int loser = GetRoundLoser();
            if (loser >= 0)
                StartCoroutine(EndOfRound(loser));
            else
                TurnInProgress = false;
        });

    }

    private int GetRoundLoser()
    {
        int loser = -1;
        for (int i = 0; i < FighterControllers.Count; i++)
        {
            if (FighterControllers[i].Health < 0)
            {
                loser = i;
                break;
            }
        }
        return loser;
    }

    private IEnumerator EndOfRound(int loser)
    {
        UIController.Instance.ShowWinLoss(loser != 0 ? "WIN" : "LOSS", FighterControllers[loser].FighterColor);

        yield return new WaitForSeconds(TurnCooldownSeconds * 3);

        UIController.Instance.HideWinLoss();
        GameManager.Instance.InitializeFighters();
        TurnInProgress = false;
    }
}
