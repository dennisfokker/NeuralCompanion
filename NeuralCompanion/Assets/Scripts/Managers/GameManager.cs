using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject GroundTilePrefab;
    public GameObject LevelBarrierPrefab;

    public GameObject PlayerFighterPrefab;
    public GameObject OpponentFighterPrefab;

    public int GroundHeight = 0;
    public int LevelWidth = 10;
    public Position PlayerStartPosition = new Position(-1, 1);
    public Position OpponentStartPosition = new Position(1, 1);

    private FighterController PlayerFighterController;
    private FighterController OpponentFighterController;

    void Start ()
	{
        float offset = LevelWidth % 2 == 0 ? 0.5f : 0f;
        for (int i = 0; i < LevelWidth / 2f + 0.5f; i++)
            Instantiate(GroundTilePrefab, new Vector3(i + offset, 0), Quaternion.identity);
        for (int i = -1; i >= -LevelWidth / 2; i--)
            Instantiate(GroundTilePrefab, new Vector3(i + offset, 0), Quaternion.identity);

        Instantiate(LevelBarrierPrefab, new Vector3(LevelWidth * -0.5f - 0.5f, 0), Quaternion.identity);
        Instantiate(LevelBarrierPrefab, new Vector3(LevelWidth * 0.5f + 0.5f, 0), Quaternion.identity);


    }
}
