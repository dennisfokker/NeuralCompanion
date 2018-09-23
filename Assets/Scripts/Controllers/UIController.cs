using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public static UIController Instance { get; private set; }

    [SerializeField]
    private Text PlayerHealthText;
    [SerializeField]
    private Text OpponentHealthText;

    [SerializeField]
    private Text WinLossText;

    [SerializeField]
    private Text SpeedText;

    [SerializeField]
    private GameObject MenuContainer;
    [SerializeField]
    private GameObject HUDContainer;

    void Awake()
	{
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
            return;
        }
    }

    void Start()
    {
        MenuContainer.SetActive(true);
        HUDContainer.SetActive(false);
    }

    public void UpdatePlayerHealth(string health)
    {
        PlayerHealthText.text = health;
    }

    public void UpdateOpponentHealth(string health)
    {
        OpponentHealthText.text = health;
    }

    public void ShowWinLoss(string message, Color color)
    {
        WinLossText.text = message;
        WinLossText.color = color;
        HUDAnimator.ShowWin(WinLossText.rectTransform);
    }

    public void HideWinLoss()
    {
        HUDAnimator.HideWin(WinLossText.rectTransform);
    }

    public void SetSpeed(float speed)
    {
        SpeedText.text = speed + "x";
    }

    public void SetHealFighter()
    {
        GameManager.Instance.NextFighterController = typeof(HealOpponentFighterController);
        HideMenu();
    }

    public void SetDefendFighter()
    {
        GameManager.Instance.NextFighterController = typeof(DefendOpponentFighterController);
        HideMenu();
    }

    public void SetMagicFighter()
    {
        GameManager.Instance.NextFighterController = typeof(MagicOpponentFighterController);
        HideMenu();
    }

    public void SetAttackFighter()
    {
        GameManager.Instance.NextFighterController = typeof(AttackOpponentFighterController);
        HideMenu();
    }

    public void SetCounterFighter()
    {
        GameManager.Instance.NextFighterController = typeof(CounterOpponentFighterController);
        HideMenu();
    }

    public void HideMenu()
    {
        GameManager.Instance.InitializeFighters();
        MenuContainer.SetActive(false);
        HUDContainer.SetActive(true);
        GameManager.paused = false;
    }

    public void ShowMenu()
    {
        GameManager.paused = true;
        Time.timeScale = 1;
        SetSpeed(1);
        MenuContainer.SetActive(true);
        HUDContainer.SetActive(false);
    }
}
