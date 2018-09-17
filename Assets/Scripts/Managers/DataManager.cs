using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set; }

    public Dictionary<ActionType, BattleActionResults> BattleActionResults = new Dictionary<ActionType, BattleActionResults>();
    public Dictionary<ActionType, Sprite> BattleActionIcons = new Dictionary<ActionType, Sprite>();

    [SerializeField]
    private BattleActionResultsList BattleActionResultsList;
    [SerializeField]
    private BattleActionIconList BattleActionIconList;

    void Awake ()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
            return;
        }

        foreach (BattleActionResults bars in BattleActionResultsList.BattleActionResults)
            BattleActionResults.Add(bars.ActionType, bars);

        foreach (BattleActionIcon bai in BattleActionIconList.BattleActionIcons)
            BattleActionIcons.Add(bai.ActionType, bai.Icon);
    }
}
