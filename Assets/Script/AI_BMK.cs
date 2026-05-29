using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AI_BMK : MonoBehaviour
{
    public MazeData mazeData;

    public Transform startPoint;
    public Transform endPoint;

    public float moveSpeed = 2f;
    public float cellSize = 0.5f;

    private float mapMinX = -9.0f;
    private float mapMinY = -4.5f;

    private bool[,] visited;
    private List<Vector2Int> path = new List<Vector2Int>();

    void Start()
    {
        Debug.Log("AI Start");

        if (startPoint == null || endPoint == null)
        {
            Debug.Log("StartPoint 또는 EndPoint 없음");
            return;
        }

        transform.position = startPoint.position;

        int cols = mazeData.cols;
        int rows = mazeData.rows;

        visited = new bool[cols, rows];

        Vector2Int start = WorldToGrid(startPoint.position);
        Vector2Int end = WorldToGrid(endPoint.position);

        bool success = DFS(start.x, start.y, end);

        Debug.Log("DFS 결과: " + success);
        Debug.Log("Path Count: " + path.Count);

        if (success)
        {
            StartCoroutine(MoveAlongPath());
        }
    }

    bool DFS(int x, int y, Vector2Int end)
    {
        int cols = mazeData.cols;
        int rows = mazeData.rows;

        // 범위 체크
        if (x < 0 || y < 0 || x >= cols || y >= rows)
            return false;

        // 이미 방문
        if (visited[x, y])
            return false;

        int idx = x + y * cols;

        // 벽이면 못 감
        if (mazeData.tileTypes[idx] == 0)
            return false;

        visited[x, y] = true;

        Vector2Int current = new Vector2Int(x, y);

        path.Add(current);

        // 도착
        if (current == end)
            return true;

        // 상하좌우 탐색
        int[] dx = { 1, -1, 0, 0 };
        int[] dy = { 0, 0, 1, -1 };

        for (int i = 0; i < 4; i++)
        {
            int nx = x + dx[i];
            int ny = y + dy[i];

            if (DFS(nx, ny, end))
                return true;
        }

        // 막다른 길이면 경로 제거
        path.RemoveAt(path.Count - 1);

        return false;
    }

    IEnumerator MoveAlongPath()
    {
        foreach (Vector2Int cell in path)
        {
            Vector3 targetPos = GridToWorld(cell);

            while (Vector3.Distance(transform.position, targetPos) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    targetPos,
                    moveSpeed * Time.deltaTime
                );

                yield return null;
            }
        }
    }

    Vector2Int WorldToGrid(Vector3 pos)
    {
        int x = Mathf.RoundToInt((pos.x - mapMinX) / cellSize);
        int y = Mathf.RoundToInt((pos.y - mapMinY) / cellSize);

        return new Vector2Int(x, y);
    }

    Vector3 GridToWorld(Vector2Int grid)
    {
        float wx = mapMinX + grid.x * cellSize;
        float wy = mapMinY + grid.y * cellSize;

        return new Vector3(wx, wy, 0);
    }
}