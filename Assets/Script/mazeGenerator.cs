using UnityEngine;
using System.Collections.Generic;

public class MazeAutoGenerator : MonoBehaviour
{
    public GameObject blockPrefab;

    [Header("Pointer Prefabs")]
    public GameObject startPointerPrefab;
    public GameObject endPointerPrefab;
    public GameObject aiPointerPrefab;

    public MazeData mazeData;

    public float cellSize = 0.5f;

    private float mapMinX = -8.0f;
    private float mapMaxX = 5.5f;
    private float mapMinY = -4.5f;
    private float mapMaxY = 5f;

    private int cols;
    private int rows;

    private bool[,] isWall;

    private List<GameObject> spawnedObjects =
        new List<GameObject>();

    private Vector2Int playerStart;
    private Vector2Int goal;
    private Vector2Int aiStart;

    public void OnClickGenerate()
    {
        foreach (GameObject obj in spawnedObjects)
        {
            if (obj != null)
                Destroy(obj);
        }

        spawnedObjects.Clear();

        cols = Mathf.RoundToInt(
            (mapMaxX - mapMinX) / cellSize);

        rows = Mathf.RoundToInt(
            (mapMaxY - mapMinY) / cellSize);

        GenerateRandomMap();

        // MazeData에게 저장을 맡긴다.
        mazeData.SaveData(
            cols,
            rows,
            isWall,
            playerStart,
            goal,
            aiStart);

        PlaceTiles();

        PlacePointers();
    }

    void GenerateRandomMap()
    {
        isWall = new bool[cols, rows];

        playerStart = new Vector2Int(0, 0);
        goal = new Vector2Int(cols / 2, rows / 2);
        aiStart = new Vector2Int(cols - 1, rows - 1);

        float blockRate = 0.15f;

        for (int x = 0; x < cols; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                isWall[x, y] =
                    Random.value < blockRate;
            }
        }

        isWall[playerStart.x,
               playerStart.y] = false;

        isWall[goal.x,
               goal.y] = false;

        isWall[aiStart.x,
               aiStart.y] = false;

        // 플레이어 시작점 주변 확보
        for (int x = playerStart.x;
             x <= playerStart.x + 1;
             x++)
        {
            for (int y = playerStart.y;
                 y <= playerStart.y + 1;
                 y++)
            {
                if (x >= 0 &&
                    x < cols &&
                    y >= 0 &&
                    y < rows)
                {
                    isWall[x, y] = false;
                }
            }
        }

        // AI 시작점 주변 확보
        for (int x = aiStart.x - 1;
             x <= aiStart.x;
             x++)
        {
            for (int y = aiStart.y - 1;
                 y <= aiStart.y;
                 y++)
            {
                if (x >= 0 &&
                    x < cols &&
                    y >= 0 &&
                    y < rows)
                {
                    isWall[x, y] = false;
                }
            }
        }

        // 목표 주변 확보
        for (int x = goal.x - 1;
             x <= goal.x + 1;
             x++)
        {
            for (int y = goal.y - 1;
                 y <= goal.y + 1;
                 y++)
            {
                if (x >= 0 &&
                    x < cols &&
                    y >= 0 &&
                    y < rows)
                {
                    isWall[x, y] = false;
                }
            }
        }
    }

    void PlaceTiles()
    {
        for (int x = 0; x < cols; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                if (!isWall[x, y])
                    continue;

                Vector3 pos =
                    GridToWorld(x, y, 0);

                GameObject obj =
                    Instantiate(
                        blockPrefab,
                        pos,
                        Quaternion.identity);

                spawnedObjects.Add(obj);
            }
        }
    }

    void PlacePointers()
    {
        if (startPointerPrefab != null)
        {
            spawnedObjects.Add(
                Instantiate(
                    startPointerPrefab,
                    GridToWorld(
                        playerStart.x,
                        playerStart.y,
                        -1),
                    Quaternion.identity));
        }

        if (endPointerPrefab != null)
        {
            spawnedObjects.Add(
                Instantiate(
                    endPointerPrefab,
                    GridToWorld(
                        goal.x,
                        goal.y,
                        -1),
                    Quaternion.identity));
        }

        if (aiPointerPrefab != null)
        {
            spawnedObjects.Add(
                Instantiate(
                    aiPointerPrefab,
                    GridToWorld(
                        aiStart.x,
                        aiStart.y,
                        -1),
                    Quaternion.identity));
        }
    }

    Vector3 GridToWorld(
        int x,
        int y,
        float z)
    {
        float wx =
            mapMinX + x * cellSize;

        float wy =
            mapMinY + y * cellSize;

        return new Vector3(wx, wy, z);
    }
}