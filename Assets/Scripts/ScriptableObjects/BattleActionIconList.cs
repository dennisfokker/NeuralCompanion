using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BattleActionIconListData", menuName = "Battle action/Icon list")]
public class BattleActionIconList : ScriptableObject
{
    public List<BattleActionIcon> BattleActionIcons;
}
