using System;
using System.Collections.Generic;

[Serializable]
public struct BattleActionResults
{
    public ActionType ActionType;
    public List<BattleActionResult> BattleActionResultList;
}
