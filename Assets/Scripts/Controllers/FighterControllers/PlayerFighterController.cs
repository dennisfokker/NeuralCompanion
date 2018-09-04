using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFighterController : FighterController
{
    public override IEnumerator PerformTurn(Action callback)
    {
        callback();
        yield break;
    }
}
