using System;
using UnityEngine;

[Serializable]
public struct BattleAction
{
    public ActionType ActionType;
    public int Target;

    public BattleAction(ActionType actionType, int target)
    {
        ActionType = actionType;
        Target = target;
    }

    public override bool Equals(object obj)
    {
        if (!(obj is BattleAction))
        {
            return false;
        }

        var action = (BattleAction) obj;
        return ActionType == action.ActionType &&
               Target == action.Target;
    }

    public override int GetHashCode()
    {
        var hashCode = 1498119943;
        hashCode = hashCode * -1521134295 + base.GetHashCode();
        hashCode = hashCode * -1521134295 + ActionType.GetHashCode();
        hashCode = hashCode * -1521134295 + Target.GetHashCode();
        return hashCode;
    }
}
