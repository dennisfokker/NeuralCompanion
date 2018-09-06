using System;
using System.Collections.Generic;

[Serializable]
public struct BattleActionResults
{
    public BattleAction BattleAction;
    public List<BattleActionResult> BattleActionResultList;
}
