using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public static bool paused { get; set; }

    public GameObject GroundTilePrefab;
    public GameObject LevelBarrierPrefab;

    public GameObject FighterPrefab;

    public int GroundHeight = 0;
    public int LevelWidth = 10;
    public Position PlayerStartPosition = new Position(-1, 1);
    public Position OpponentStartPosition = new Position(0, 1);

    public float MaxHealth = 20;

    public Type NextFighterController { get; set; }
    public float Offset { get; private set; }
    public GameController GameController { get; private set; }
    public NeuralNetworkManager NeuralNetworkManager { get; private set; }

    private FighterController PlayerFighterController;
    private FighterController OpponentFighterController;

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

        GameController = GetComponent<GameController>();
        NeuralNetworkManager = GetComponent<NeuralNetworkManager>();
        NextFighterController = NextFighterController ?? typeof(HealOpponentFighterController);
        paused = true;
    }

    void Start ()
	{
        InitializeMap();
        InitializeFighters();
    }

    public void InitializeMap()
    {
        Offset = LevelWidth % 2 == 0 ? 0.5f : 0f;
        for (int i = 0; i < LevelWidth / 2f + 0.5f - Offset; i++)
            Instantiate(GroundTilePrefab, new Vector3(i + Offset, 0), Quaternion.identity);
        for (int i = -1; i >= -LevelWidth / 2; i--)
            Instantiate(GroundTilePrefab, new Vector3(i + Offset, 0), Quaternion.identity);

        Instantiate(LevelBarrierPrefab, new Vector3(LevelWidth * -0.5f - 0.5f, 0), Quaternion.identity);
        Instantiate(LevelBarrierPrefab, new Vector3(LevelWidth * 0.5f + 0.5f, 0), Quaternion.identity);
    }

    public void InitializeFighters()
    {
        if (PlayerFighterController != null)
            Destroy(PlayerFighterController.gameObject);
        if (OpponentFighterController != null)
            Destroy(OpponentFighterController.gameObject);

        GameObject playerGO = Instantiate(FighterPrefab, new Vector3(PlayerStartPosition.X + Offset, PlayerStartPosition.Y), Quaternion.identity);
        PlayerFighterController = playerGO.AddComponent<NeuralFighterController>();
        GameObject opponentGO = Instantiate(FighterPrefab, new Vector3(OpponentStartPosition.X + Offset, OpponentStartPosition.Y), Quaternion.identity);
        OpponentFighterController = (FighterController) opponentGO.AddComponent(NextFighterController);

        GameController.FighterControllers = new Dictionary<int, FighterController>
        {
            { 0, PlayerFighterController },
            { 1, OpponentFighterController }
        };

        GameController.ResetGame();
    }
}
