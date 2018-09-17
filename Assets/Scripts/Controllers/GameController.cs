using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static event Action<Dictionary<int, FighterController>> RoundStart;
    public static event Action<Dictionary<int, FighterController>> StartTurn;
    public static event Action<int, Dictionary<int, FighterController>> RoundEnd;

    public float TurnCooldownSeconds = 2f;

    public Dictionary<int, FighterController> FighterControllers { get; set; }
    public Dictionary<int, BattleAction> PreviousBattleActions { get; private set; }

    private Dictionary<int, BattleAction> battleActions = new Dictionary<int, BattleAction>();
    private int turn = 0;
    private int fighterCount;
    private bool StartRound = true;
    private bool TurnInProgress = false;

    void Awake()
    {
        if (FighterControllers == null)
            FighterControllers = new Dictionary<int, FighterController>();
        PreviousBattleActions = new Dictionary<int, BattleAction>();

        RoundStart = RoundStart ?? new Action<Dictionary<int, FighterController>>((fighters) => { });
        StartTurn = StartTurn ?? new Action<Dictionary<int, FighterController>>((fighters) => { });
        RoundEnd += (loser, fighters) =>
        {
            GameManager.Instance.StartCoroutine(EndOfRound(loser, fighters));
        };
    }

    void Update()
    {
        if (TurnInProgress)
            return;
        
        if (StartRound)
            RoundStart(FighterControllers);

        RequestActions();
    }

    private void RequestActions()
    {
        TurnInProgress = true;
        StartRound = false;
        turn++;
        StartTurn(FighterControllers);

        fighterCount = FighterControllers.Count;
        foreach (KeyValuePair<int, FighterController> entry in FighterControllers)
        {
            StartCoroutine(entry.Value.PerformTurn(FighterControllers, PreviousBattleActions, ba => ProcessAction(ba, entry.Key)));
        }
    }

    private void ProcessAction(BattleAction ba, int index)
    {
        battleActions.Add(index, ba);

        if (battleActions.Count == fighterCount)
            ProcessTurn();
    }

    private void ProcessTurn()
    {
        PreviousBattleActions = battleActions;

        foreach (KeyValuePair<int, BattleAction> entry in battleActions)
        {
            FighterController fc = FighterControllers[entry.Key];
            fc.ActionRenderer.sprite = DataManager.Instance.BattleActionIcons[entry.Value.ActionType];

            FighterController targetFC = FighterControllers[entry.Value.Target];
            BattleActionResults bars = DataManager.Instance.BattleActionResults[entry.Value.ActionType];
            float receivedDamage = 0;
            foreach (var bar in bars.BattleActionResultList)
                if (bar.ActionType == battleActions[entry.Value.Target].ActionType)
                    receivedDamage = bar.ReceivingDamage;

            fc.Health -= receivedDamage;
        }

        DelayUtil.WaitForSeconds(TurnCooldownSeconds + 1, () =>
        {
            foreach (KeyValuePair<int, BattleAction> entry in battleActions)
            {
                FighterControllers[entry.Key].ClearActionIcon();
            }
            battleActions = new Dictionary<int, BattleAction>();

            Dictionary<int, float> healthFighters = new Dictionary<int, float>();
            foreach (KeyValuePair<int, FighterController> entry in FighterControllers)
                healthFighters.Add(entry.Key, entry.Value.Health);

            int loser = GetRoundLoser(healthFighters);
            if (loser >= 0)
                RoundEnd(loser, FighterControllers);
            else
                TurnInProgress = false;
        });

    }

    private int GetRoundLoser(Dictionary<int, float> healthFighters)
    {
        int loser = -1;
        int lowest = -1;
        float lowestHealth = float.MaxValue;
        foreach (KeyValuePair<int, float> entry in healthFighters)
        {
            if (turn > NeuralParameters.MAX_TURNS && entry.Value < lowestHealth)
            {
                lowest = entry.Key;
                lowestHealth = entry.Value;
            }
            else if (entry.Value <= 0)
            {
                loser = entry.Key;
                break;
            }
        }

        if (turn > NeuralParameters.MAX_TURNS)
            loser = lowest;

        return loser;
    }

    private IEnumerator EndOfRound(int loser, Dictionary<int, FighterController> fighters)
    {
        UIController.Instance.ShowWinLoss(loser != 0 ? "WIN" : "LOSS", fighters[loser].FighterColor);

        yield return new WaitForSeconds(TurnCooldownSeconds * 3);

        UIController.Instance.HideWinLoss();
        GameManager.Instance.InitializeFighters();
        TurnInProgress = false;
        StartRound = true;
        turn = 0;
    }
}
