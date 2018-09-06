using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameObject GroundTilePrefab;
    public GameObject LevelBarrierPrefab;

    public GameObject PlayerFighterPrefab;
    public GameObject OpponentFighterPrefab;

    public int GroundHeight = 0;
    public int LevelWidth = 10;
    public Position PlayerStartPosition = new Position(-1, 1);
    public Position OpponentStartPosition = new Position(0, 1);

    public float MaxHealth = 20;

    public float Offset { get; private set; }
    public GameController GameController { get; private set; }

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
        PlayerFighterController = Instantiate(PlayerFighterPrefab, new Vector3(PlayerStartPosition.X + Offset, PlayerStartPosition.Y), Quaternion.identity).GetComponent<FighterController>();
        PlayerFighterController.Health = MaxHealth;
        OpponentFighterController = Instantiate(OpponentFighterPrefab, new Vector3(OpponentStartPosition.X + Offset, OpponentStartPosition.Y), Quaternion.identity).GetComponent<FighterController>();
        OpponentFighterController.Health = MaxHealth;

        GameController.FighterControllers = new List<FighterController> { PlayerFighterController, OpponentFighterController };
    }
}
