using UnityEngine;
using System.Collections.Generic;

public class MazeAutoGenerator1 : MonoBehaviour
{
    public GameObject blockPrefab;
    public GameObject obstaclePrefab;
    public GameObject boostPrefab;

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

    public void OnClickGenerate()
    {
        foreach (GameObject obj in spawnedObjects)
        {
            if (obj != null)
                Destroy(obj);
        }

        spawnedObjects.Clear();

        cols = Mathf.RoundToInt((mapMaxX - mapMinX) / cellSize);
        rows = Mathf.RoundToInt((mapMaxY - mapMinY) / cellSize);

        GenerateMaze1();

        startCell = new Vector2Int(0, 0);
        endCell = FindFarthestOpenCell(startCell);

        SaveMazeData();
        PlaceTiles();
        PlacePointers();
    }

    void GenerateMaze1()
    {
        isWall = new bool[cols, rows];

        for (int x = 0; x < cols; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                isWall[x, y] = true;
            }
        }

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
            int nx = cx;
            int ny = cy;
            int mx = cx;
            int my = cy;

            if (dir == 0)
            {
                ny += 2;
                my += 1;
            }
            else if (dir == 1)
            {
                ny -= 2;
                my -= 1;
            }
            else if (dir == 2)
            {
                nx -= 2;
                mx -= 1;
            }
            else if (dir == 3)
            {
                nx += 2;
                mx += 1;
            }

            if (nx < 0 || nx >= cols || ny < 0 || ny >= rows)
                continue;

            if (visited[nx, ny])
                continue;

            isWall[mx, my] = false;
            DFS(nx, ny, visited);
        }
    }

    void PlaceTiles()
    {
        for (int x = 0; x < cols; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                Vector3 pos = GridToWorld(x, y, 0f);

                int idx = x + y * cols;
                int type = mazeData.tileTypes[idx];

                GameObject prefab;

                if (type == 0)
                {
                    prefab = blockPrefab;
                }
                else if (type == 1)
                {
                    prefab = obstaclePrefab;
                }
                else
                {
                    prefab = boostPrefab;
                }

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
        {
            GameObject startPointer = Instantiate(startPointerPrefab, startPos, Quaternion.identity);
            spawnedObjects.Add(startPointer);
        }

        if (endPointerPrefab != null)
        {
            GameObject endPointer = Instantiate(endPointerPrefab, endPos, Quaternion.identity);
            spawnedObjects.Add(endPointer);
        }
    }

    Vector2Int FindFarthestOpenCell(Vector2Int start)
    {
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        int[,] distance = new int[cols, rows];

        for (int x = 0; x < cols; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                distance[x, y] = -1;
            }
        }

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

                if (nx < 0 || nx >= cols || ny < 0 || ny >= rows)
                    continue;

                if (isWall[nx, ny])
                    continue;

                if (distance[nx, ny] != -1)
                    continue;

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

            int temp = arr[i];
            arr[i] = arr[j];
            arr[j] = temp;
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

        for (int x = 0; x < cols; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                int idx = x + y * cols;

                mazeData.isWall[idx] = isWall[x, y];

                if (isWall[x, y])
                {
                    mazeData.tileTypes[idx] = 0;
                }
                else
                {
                    mazeData.tileTypes[idx] = Random.value > 0.5f ? 1 : 2;
                }
            }
        }
    }
}