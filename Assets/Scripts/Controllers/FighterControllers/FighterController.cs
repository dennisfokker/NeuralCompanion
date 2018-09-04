using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FighterController : MonoBehaviour
{
    public bool FlipRenderer = false;
    public Color FighterColor = Color.white;

    public GameManager GameManager { get; private set; }
    public SpriteRenderer BackgroundRenderer { get; private set; }
    public SpriteRenderer ActionRenderer { get; private set; }

    public void Awake()
    {
        ActionRenderer = GetComponent<SpriteRenderer>();
        GameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();

        if (FlipRenderer)
            transform.localScale = Vector3.Scale(transform.localScale, new Vector3(-1, 1, 1));
        ActionRenderer.color = FighterColor;
    }

    public abstract IEnumerator PerformTurn(Action callback);
}
