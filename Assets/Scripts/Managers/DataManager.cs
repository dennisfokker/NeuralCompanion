using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set; }

    public Dictionary<BattleAction, BattleActionResults> BattleActionResults = new Dictionary<BattleAction, BattleActionResults>();
    public Dictionary<BattleAction, Sprite> BattleActionIcons = new Dictionary<BattleAction, Sprite>();

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
            BattleActionResults.Add(bars.BattleAction, bars);

        foreach (BattleActionIcon bai in BattleActionIconList.BattleActionIcons)
            BattleActionIcons.Add(bai.BattleAction, bai.Icon);
    }
}
