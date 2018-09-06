using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BattleActionResultsListData", menuName = "Battle action/Results list")]
public class BattleActionResultsList : ScriptableObject
{
    public List<BattleActionResults> BattleActionResults;
}
