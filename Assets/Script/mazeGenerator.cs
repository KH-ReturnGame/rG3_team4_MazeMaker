using UnityEngine;
using System.Collections.Generic;

public class MazeAutoGenerator : MonoBehaviour
{
    public GameObject blockPrefab;
    public GameObject obstaclePrefab;
    public GameObject boostPrefab;

    public float cellSize = 1.0f;

    private float mapMinX = -9.0f;
    private float mapMaxX = 4.5f;
    private float mapMinY = -4.5f;
    private float mapMaxY = 4.5f;

    private int cols;
    private int rows;
    private bool[,] isWall;
    private List<GameObject> spawnedObjects = new List<GameObject>();

    public void OnClickGenerate()
    {
        // 기존 배치 전부 제거
        foreach (GameObject obj in spawnedObjects)
            Destroy(obj);
        spawnedObjects.Clear();

        cols = Mathf.RoundToInt((mapMaxX - mapMinX) / cellSize);
        rows = Mathf.RoundToInt((mapMaxY - mapMinY) / cellSize);

        GenerateMaze();
        PlaceTiles();
    }

    void GenerateMaze()
    {
        isWall = new bool[cols, rows];

        // 전부 벽으로 초기화
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
            if (dir == 1) { ny -= 2; my -= 1; }
            if (dir == 2) { nx -= 2; mx -= 1; }
            if (dir == 3) { nx += 2; mx += 1; }

            if (nx < 0 || nx >= cols || ny < 0 || ny >= rows) continue;
            if (visited[nx, ny]) continue;

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
                float wx = mapMinX + x * cellSize + cellSize * 0.5f;
                float wy = mapMinY + y * cellSize + cellSize * 0.5f;
                Vector3 pos = new Vector3(wx, wy, 0);

                GameObject prefab;

                if (isWall[x, y])
                {
                    prefab = blockPrefab;
                }
                else
                {
                    // 통로 칸은 Obstacle/Boost 랜덤, 혹은 비워둘 수도 있음
                    prefab = Random.value > 0.5f ? obstaclePrefab : boostPrefab;
                }

                GameObject instance = Instantiate(prefab, pos, Quaternion.identity);
                spawnedObjects.Add(instance);
            }
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
}