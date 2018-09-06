using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class FighterController : MonoBehaviour
{
    public bool FlipRenderer = false;
    public Color FighterColor = Color.white;
    
    public SpriteRenderer BackgroundRenderer { get; internal set; }
    public SpriteRenderer ActionRenderer { get; internal set; }

    public List<BattleAction> PreviousOpponentsAction { get; set; }

    internal Action<string> healthChangeAction;

    public float Health {
        get {
            return health;
        }
        set {
            animateHealthChange(value - health, () => healthChangeAction(value.ToString()));

            health = value;
        }
    }

    private float health;

    public void Awake()
    {
        ActionRenderer = transform.Find("ActionRenderer").GetComponent<SpriteRenderer>();
        BackgroundRenderer = transform.Find("BackgroundRenderer").GetComponent<SpriteRenderer>();

        if (FlipRenderer)
            transform.localScale = Vector3.Scale(transform.localScale, new Vector3(-1, 1, 1));
        BackgroundRenderer.color = FighterColor;

        PreviousOpponentsAction = new List<BattleAction>();
    }

    public void Start()
    {
        health = GameManager.Instance.MaxHealth;
        healthChangeAction(health.ToString());
    }

    public void ClearActionIcon()
    {
        ActionRenderer.sprite = null;
    }

    public abstract IEnumerator PerformTurn(Action<BattleAction> callback);

    private void animateHealthChange(float difference, Action callback)
    {
        if (difference < 0)
        {
            FighterAnimator.ReceiveDamage(gameObject, callback);
            return;
        }
        if (difference > 0)
        {
            FighterAnimator.ReceiveHealth(BackgroundRenderer.gameObject, BackgroundRenderer.color, callback);
            return;
        }
        FighterAnimator.NoDamage(gameObject, callback);
    }
}
