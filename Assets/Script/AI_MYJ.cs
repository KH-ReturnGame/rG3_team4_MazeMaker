using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AI_MYJ : MonoBehaviour
{
    public MazeData mazeData;
    public Transform startPoint;
    public Transform endPoint;
    public float moveSpeed = 2f;
    public float cellSize = 0.5f;
    public int movePerSearch = 5;

    private float mapMinX = -8.0f;
    private float mapMinY = -4.5f;

    private List<Vector2Int> path = new List<Vector2Int>();

    void Start()
    {
        Debug.Log("AI_MYJ Start");

        if (startPoint == null || endPoint == null)
        {
            Debug.Log("StartPoint 또는 EndPoint 없음");
            return;
        }

        transform.position = startPoint.position;
        StartCoroutine(AILoop());
    }

    IEnumerator AILoop()
    {
        Vector2Int end = WorldToGrid(endPoint.position);

        while (true)
        {
            Vector2Int current = WorldToGrid(transform.position);

            if (current == end)
            {
                Debug.Log("AI 도착!");
                yield break;
            }

            bool success = Dijkstra(current, end);

            if (!success)
            {
                Debug.Log("길 없음");
                yield return new WaitForSeconds(0.2f);
                continue;
            }

            int moveCount = Mathf.Min(movePerSearch, path.Count - 1);

            for (int i = 1; i <= moveCount; i++)
            {
                yield return MoveToCell(path[i]);
            }

            yield return null;
        }
    }

    bool Dijkstra(Vector2Int start, Vector2Int end)
    {
        int cols = mazeData.cols;
        int rows = mazeData.rows;

        int[,] dist = new int[cols, rows];
        Vector2Int[,] parent = new Vector2Int[cols, rows];

        for (int x = 0; x < cols; x++)
            for (int y = 0; y < rows; y++)
            {
                dist[x, y] = int.MaxValue;
                parent[x, y] = new Vector2Int(-1, -1);
            }

        dist[start.x, start.y] = 0;

        List<(int cost, Vector2Int node)> openList = new List<(int, Vector2Int)>();
        openList.Add((0, start));

        int[] dx = { 1, -1, 0, 0 };
        int[] dy = { 0, 0, 1, -1 };

        while (openList.Count > 0)
        {
            openList.Sort((a, b) => a.cost.CompareTo(b.cost));
            var (curCost, cur) = openList[0];
            openList.RemoveAt(0);

            if (curCost > dist[cur.x, cur.y])
                continue;

            if (cur == end)
            {
                ReconstructPath(parent, start, end);
                return true;
            }

            for (int i = 0; i < 4; i++)
            {
                int nx = cur.x + dx[i];
                int ny = cur.y + dy[i];

                if (nx < 0 || ny < 0 || nx >= cols || ny >= rows)
                    continue;

                int idx = nx + ny * cols;

                if (mazeData.tileTypes[idx] == 0)
                    continue;

                int newCost = dist[cur.x, cur.y] + 1;

                if (newCost < dist[nx, ny])
                {
                    dist[nx, ny] = newCost;
                    parent[nx, ny] = cur;
                    openList.Add((newCost, new Vector2Int(nx, ny)));
                }
            }
        }

        return false;
    }

    void ReconstructPath(Vector2Int[,] parent, Vector2Int start, Vector2Int end)
    {
        path.Clear();
        Vector2Int cur = end;

        while (cur != start)
        {
            path.Add(cur);
            cur = parent[cur.x, cur.y];
        }

        path.Add(start);
        path.Reverse();
    }

    IEnumerator MoveToCell(Vector2Int targetCell)
    {
        Vector3 targetPos = GridToWorld(targetCell);

        while (Vector3.Distance(transform.position, targetPos) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPos,
                moveSpeed * Time.deltaTime
            );
            yield return null;
        }

        transform.position = targetPos;
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
        return new Vector3(wx, wy, -1);
    }
}