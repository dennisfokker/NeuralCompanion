using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FighterController : MonoBehaviour
{
    public bool FlipRenderer = false;
    public Color FighterColor = Color.white;
    public GameManager GameManager;

    public SpriteRenderer BackgroundRenderer { get; private set; }
    public SpriteRenderer ActionRenderer { get; private set; }

    public void Awake()
    {
        if (FlipRenderer)
            transform.position.Scale(new Vector3(-1, 1, 1));

        ActionRenderer = GetComponent<SpriteRenderer>();
        GameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }

    public abstract IEnumerator PerformTurn(Action callback);
}
