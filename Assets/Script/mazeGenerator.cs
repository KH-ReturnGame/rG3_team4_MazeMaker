using UnityEngine;
using System.Collections.Generic;

public class MazeAutoGenerator : MonoBehaviour
{
    public GameObject blockPrefab;
    public GameObject obstaclePrefab;
    public GameObject boostPrefab;
    public GameObject emptyPrefab; // 빈칸 프리팹 (없으면 null 처리)

    [Header("Pointer Prefabs")]
    public GameObject startPointerPrefab;
    public GameObject endPointerPrefab;

    public float cellSize = 0.5f;

    private float mapMinX = -9.0f;
    private float mapMaxX = 5.0f;
    private float mapMinY = -4.5f;
    private float mapMaxY = 5.0f;

    private int cols;
    private int rows;

    private bool[,] isWall;
    private List<GameObject> spawnedObjects = new List<GameObject>();

    public MazeData mazeData;

    private Vector2Int startCell;
    private Vector2Int endCell;

    // 비용 설정
    private const int COST_BLOCK = 10;
    private const int COST_OBSTACLE = 30;
    private const int COST_BOOST = 20;
    private const int COST_EMPTY = 0;
    private const int TOTAL_BUDGET = 5000;

    public void OnClickGenerate()
    {
        foreach (GameObject obj in spawnedObjects)
            if (obj != null) Destroy(obj);
        spawnedObjects.Clear();

        cols = Mathf.RoundToInt((mapMaxX - mapMinX) / cellSize);
        rows = Mathf.RoundToInt((mapMaxY - mapMinY) / cellSize);

        GenerateMaze();

        startCell = new Vector2Int(0, 0);
        endCell = FindFarthestOpenCell(startCell);

        SaveMazeData();
        PlaceTiles();
        PlacePointers();
    }

    void GenerateMaze()
    {
        isWall = new bool[cols, rows];
        for (int x = 0; x < cols; x++)
            for (int y = 0; y < rows; y++)
                isWall[x, y] = true;

        bool[,] visited = new bool[cols, rows];
        DFS(0, 0, visited);
    }

    void DFS(int cx, int cy, bool[,] visited)
    {
        visited[cx, cy] = true;
        isWall[cx, cy] = false;

        int[] dirs = { 0, 1, 2, 3 };
        ShuffleArray(dirs);

        foreach (int dir in dirs)
        {
            int nx = cx, ny = cy;
            int mx = cx, my = cy;

            if (dir == 0) { ny += 2; my += 1; }
            else if (dir == 1) { ny -= 2; my -= 1; }
            else if (dir == 2) { nx -= 2; mx -= 1; }
            else if (dir == 3) { nx += 2; mx += 1; }

            if (nx < 0 || nx >= cols || ny < 0 || ny >= rows) continue;
            if (visited[nx, ny]) continue;

            isWall[mx, my] = false;
            DFS(nx, ny, visited);
        }
    }

    void SaveMazeData()
    {
        if (mazeData == null)
        {
            Debug.LogError("MazeData가 연결되어 있지 않습니다.");
            return;
        }

        mazeData.cols = cols;
        mazeData.rows = rows;
        mazeData.isWall = new bool[cols * rows];
        mazeData.tileTypes = new int[cols * rows];

        mazeData.startX = startCell.x;
        mazeData.startY = startCell.y;
        mazeData.endX = endCell.x;
        mazeData.endY = endCell.y;
        mazeData.totalBudget = TOTAL_BUDGET;

        // 1단계: 벽은 block(0), 빈칸은 일단 전부 empty(3)으로 초기화
        for (int x = 0; x < cols; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                int idx = x + y * cols;
                mazeData.isWall[idx] = isWall[x, y];
                mazeData.tileTypes[idx] = isWall[x, y] ? 0 : 3; // 0=Block, 3=Empty
            }
        }

        // 2단계: 벽(block) 비용 먼저 계산
        int usedCost = 0;
        for (int x = 0; x < cols; x++)
            for (int y = 0; y < rows; y++)
                if (isWall[x, y]) usedCost += COST_BLOCK;

        // 3단계: 빈칸 셀들을 랜덤 순서로 섞어서 예산 내에서 obstacle/boost 배치
        List<int> openIndices = new List<int>();
        for (int x = 0; x < cols; x++)
            for (int y = 0; y < rows; y++)
                if (!isWall[x, y]) openIndices.Add(x + y * cols);

        ShuffleList(openIndices);

        foreach (int idx in openIndices)
        {
            // obstacle(30원) 또는 boost(10원) 중 랜덤 선택
            bool tryObstacle = Random.value > 0.5f;
            int cost = tryObstacle ? COST_OBSTACLE : COST_BOOST;

            if (usedCost + cost <= TOTAL_BUDGET)
            {
                mazeData.tileTypes[idx] = tryObstacle ? 1 : 2;
                usedCost += cost;
            }
            // 예산 초과면 empty(3) 유지
        }

        mazeData.usedCost = usedCost;
        Debug.Log($"총 사용 비용: {usedCost} / {TOTAL_BUDGET}");

        int emptyCnt = 0, obsCnt = 0, boostCnt = 0, blockCnt = 0;
        for (int i = 0; i < mazeData.tileTypes.Length; i++)
        {
            if (mazeData.tileTypes[i] == 0) blockCnt++;
            else if (mazeData.tileTypes[i] == 1) obsCnt++;
            else if (mazeData.tileTypes[i] == 2) boostCnt++;
            else if (mazeData.tileTypes[i] == 3) emptyCnt++;
        }
        Debug.Log($"Block:{blockCnt} Obstacle:{obsCnt} Boost:{boostCnt} Empty:{emptyCnt}");
    }

    void PlaceTiles()
    {
        for (int x = 0; x < cols; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                int idx = x + y * cols;
                int type = mazeData.tileTypes[idx];

                GameObject prefab = type == 0 ? blockPrefab
                                  : type == 1 ? obstaclePrefab
                                  : type == 2 ? boostPrefab
                                  : emptyPrefab; // type == 3

                if (prefab == null) continue; // 빈칸 프리팹 없으면 스킵

                Vector3 pos = GridToWorld(x, y, 0f);
                GameObject instance = Instantiate(prefab, pos, Quaternion.identity);
                spawnedObjects.Add(instance);
            }
        }
    }

    void PlacePointers()
    {
        Vector3 startPos = GridToWorld(startCell.x, startCell.y, -1f);
        Vector3 endPos = GridToWorld(endCell.x, endCell.y, -1f);

        if (startPointerPrefab != null)
            spawnedObjects.Add(Instantiate(startPointerPrefab, startPos, Quaternion.identity));

        if (endPointerPrefab != null)
            spawnedObjects.Add(Instantiate(endPointerPrefab, endPos, Quaternion.identity));
    }

    Vector2Int FindFarthestOpenCell(Vector2Int start)
    {
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        int[,] distance = new int[cols, rows];

        for (int x = 0; x < cols; x++)
            for (int y = 0; y < rows; y++)
                distance[x, y] = -1;

        queue.Enqueue(start);
        distance[start.x, start.y] = 0;

        Vector2Int farthest = start;
        int maxDistance = 0;

        int[] dx = { 1, -1, 0, 0 };
        int[] dy = { 0, 0, 1, -1 };

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();
            for (int i = 0; i < 4; i++)
            {
                int nx = current.x + dx[i];
                int ny = current.y + dy[i];

                if (nx < 0 || nx >= cols || ny < 0 || ny >= rows) continue;
                if (isWall[nx, ny]) continue;
                if (distance[nx, ny] != -1) continue;

                distance[nx, ny] = distance[current.x, current.y] + 1;
                queue.Enqueue(new Vector2Int(nx, ny));

                if (distance[nx, ny] > maxDistance)
                {
                    maxDistance = distance[nx, ny];
                    farthest = new Vector2Int(nx, ny);
                }
            }
        }

        return farthest;
    }

    Vector3 GridToWorld(int x, int y, float z)
    {
        float wx = mapMinX + x * cellSize;
        float wy = mapMinY + y * cellSize;
        return new Vector3(wx, wy, z);
    }

    void ShuffleArray(int[] arr)
    {
        for (int i = arr.Length - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (arr[i], arr[j]) = (arr[j], arr[i]);
        }
    }

    void ShuffleList(List<int> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}