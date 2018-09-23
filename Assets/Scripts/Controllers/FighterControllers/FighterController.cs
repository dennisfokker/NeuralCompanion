using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class FighterController : MonoBehaviour
{
    public int Target { get; internal set; }
    public bool FlipRenderer { get; internal set; }
    public Color FighterColor { get; internal set; }

    public virtual SpriteRenderer BackgroundRenderer { get; internal set; }
    public virtual SpriteRenderer ActionRenderer { get; internal set; }

    internal Action<string> healthChangeAction = new Action<string>((s) => { /* nothing */ });
    internal GameController GameController;

    public virtual float Health {
        get {
            return health;
        }
        set {
            animateHealthChange(value - health, () => healthChangeAction(value.ToString()));

            health = value;
        }
    }

    private float health;

    public virtual void Awake()
    {
        ActionRenderer = transform.Find("ActionRenderer")?.GetComponent<SpriteRenderer>();
        BackgroundRenderer = transform.Find("BackgroundRenderer")?.GetComponent<SpriteRenderer>();

        if (transform.position.x > 0)
        {
            FlipRenderer = true;
            FighterColor = Color.red;
        }
        else
        {
            FlipRenderer = false;
            FighterColor = Color.blue;
        }

        if (FlipRenderer)
            transform.localScale = Vector3.Scale(transform.localScale, new Vector3(-1, 1, 1));
        if (BackgroundRenderer != null)
            BackgroundRenderer.color = FighterColor;

        Target = -1;

        GameController = GameManager.Instance.GameController;
    }

    public virtual void Start()
    {
        health = GameManager.Instance.MaxHealth;
        if (BackgroundRenderer != null)
            healthChangeAction(health.ToString());
    }

    public virtual void ClearActionIcon()
    {
        ActionRenderer.sprite = null;
    }
    
    public virtual IEnumerator PerformTurn(Dictionary<int, FighterController> fighters, Dictionary<int, BattleAction> previousActions, Action<BattleAction> callback)
    {
        callback(GetAction(fighters, previousActions));
        yield break;
    }

    public abstract BattleAction GetAction(Dictionary<int, FighterController> fighters, Dictionary<int, BattleAction> previousActions);

    internal virtual void animateHealthChange(float difference, Action callback)
    {
        if (BackgroundRenderer == null)
            return;

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
