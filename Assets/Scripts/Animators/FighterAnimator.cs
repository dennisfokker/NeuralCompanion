using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FighterAnimator
{
    public static void ReceiveDamage(GameObject gameObject, Action onComplete = null)
    {
        if (LeanTween.isTweening(gameObject))
            LeanTween.cancel(gameObject);

        LeanTween.moveX(gameObject, gameObject.transform.position.x + 0.15f, 0.3f)
            .setEaseShake()
            .setRepeat(2)
            .setOnComplete(onComplete);
    }

    public static void NoDamage(GameObject gameObject, Action onComplete = null)
    {
        if (LeanTween.isTweening(gameObject))
            LeanTween.cancel(gameObject);

        LeanTween.moveY(gameObject, gameObject.transform.position.y + 0.5f, 0.3f)
            .setEaseOutCubic()
            .setOnComplete(() =>
            {
                LeanTween.moveY(gameObject, gameObject.transform.position.y - 0.5f, 0.7f)
                    .setEaseOutBounce()
                    .setOnComplete(onComplete);
            });
    }

    public static void ReceiveHealth(GameObject gameObject, Color originalColor, Action onComplete = null)
    {
        if (LeanTween.isTweening(gameObject))
            LeanTween.cancel(gameObject);
        
        LeanTween.color(gameObject, Color.green, 0.25f)
            .setEaseInCubic()
            .setOnComplete(() =>
            {
                LeanTween.color(gameObject, originalColor, 0.25f)
                    .setEaseInCubic()
                    .setOnComplete(() =>
                    {
                        LeanTween.color(gameObject, Color.green, 0.25f)
                        .setEaseInCubic()
                        .setOnComplete(() =>
                            {
                                LeanTween.color(gameObject, originalColor, 0.25f)
                                    .setEaseInCubic()
                                    .setOnComplete(onComplete);
                            });
                    });
            });
    }
}
