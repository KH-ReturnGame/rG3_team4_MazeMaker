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

    [Header("생성 설정")]
    [Range(0f, 1f)]
    public float blockRate = 0.35f;   // DFS 벽 중 실제로 남길 비율 (1/4)

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

        GenerateMazeDFS();

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

    void GenerateMazeDFS()
    {
        playerStart = new Vector2Int(0, 0);
        goal = new Vector2Int(cols / 2, rows / 2);
        aiStart = new Vector2Int(cols - 1, rows - 1);

        // 1단계: DFS로 완성된 미로 구조를 만든다
        bool[,] mazeWall = new bool[cols, rows];

        for (int x = 0; x < cols; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                mazeWall[x, y] = true;
            }
        }

        bool[,] visited = new bool[cols, rows];
        DFS(0, 0, visited, mazeWall);

        // 2단계: DFS가 만든 벽 중 blockRate 확률로만 실제 설치
        isWall = new bool[cols, rows];

        for (int x = 0; x < cols; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                if (mazeWall[x, y] && Random.value < blockRate)
                    isWall[x, y] = true;
                else
                    isWall[x, y] = false;
            }
        }

        // 3단계: 주요 지점 확보
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

    void DFS(
        int cx,
        int cy,
        bool[,] visited,
        bool[,] mazeWall)
    {
        visited[cx, cy] = true;
        mazeWall[cx, cy] = false;

        int[] dirs = { 0, 1, 2, 3 };
        ShuffleArray(dirs);

        foreach (int dir in dirs)
        {
            int nx = cx;
            int ny = cy;
            int mx = cx;
            int my = cy;

            if (dir == 0) { ny += 2; my += 1; }
            else if (dir == 1) { ny -= 2; my -= 1; }
            else if (dir == 2) { nx -= 2; mx -= 1; }
            else if (dir == 3) { nx += 2; mx += 1; }

            if (nx < 0 ||
                nx >= cols ||
                ny < 0 ||
                ny >= rows)
                continue;

            if (visited[nx, ny])
                continue;

            mazeWall[mx, my] = false;
            DFS(nx, ny, visited, mazeWall);
        }
    }

    void ShuffleArray(int[] arr)
    {
        for (int i = arr.Length - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (arr[i], arr[j]) = (arr[j], arr[i]);
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