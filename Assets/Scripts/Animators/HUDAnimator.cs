using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HUDAnimator
{
    public static void ShowWin(RectTransform gameObject, Action onComplete = null)
    {
        if (LeanTween.isTweening(gameObject))
            return;

        LeanTween.scale(gameObject, Vector3.one, 2f)
            .setEaseOutElastic()
            .setOnComplete(onComplete);
    }
    public static void HideWin(RectTransform gameObject, Action onComplete = null)
    {
        if (LeanTween.isTweening(gameObject))
            return;

        LeanTween.scale(gameObject, Vector3.zero, 1f)
            .setEaseInBack()
            .setOnComplete(onComplete);
    }
}
