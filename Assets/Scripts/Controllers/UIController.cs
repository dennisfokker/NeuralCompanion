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
}
