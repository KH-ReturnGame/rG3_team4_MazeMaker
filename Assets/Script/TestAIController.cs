using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TestAIController : MonoBehaviour
{
   
    public MazeData mazeData;

    public Transform startPoint;
    public Transform endPoint;

    public float moveSpeed = 2f;
    public float cellSize = 0.5f;

    private float mapMinX = -9.0f;
    private float mapMinY = -4.5f;

    void Start()
    {
        Debug.Log("AI Start");

        transform.position = startPoint.position;

        List<Vector2Int> path = FindPath();

        Debug.Log("Path Count: " + path.Count);

        StartCoroutine(MoveAlongPath(path));
    }

    List<Vector2Int> FindPath()
    {
        int cols = mazeData.cols;
        int rows = mazeData.rows;

        Vector2Int start = WorldToGrid(startPoint.position);
        Vector2Int end = WorldToGrid(endPoint.position);

        Queue<Vector2Int> queue = new Queue<Vector2Int>();

        bool[,] visited = new bool[cols, rows];

        Dictionary<Vector2Int, Vector2Int> parent =
            new Dictionary<Vector2Int, Vector2Int>();

        queue.Enqueue(start);
        visited[start.x, start.y] = true;

        int[] dx = { 1, -1, 0, 0 };
        int[] dy = { 0, 0, 1, -1 };

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();

            if (current == end)
                break;

            for (int i = 0; i < 4; i++)
            {
                int nx = current.x + dx[i];
                int ny = current.y + dy[i];

                if (nx < 0 || ny < 0 ||
                    nx >= cols || ny >= rows)
                    continue;

                if (visited[nx, ny])
                    continue;

                int idx = nx + ny * cols;

                // 벽이면 못 감
                if (mazeData.tileTypes[idx] == 0)
                    continue;

                visited[nx, ny] = true;

                Vector2Int next =
                    new Vector2Int(nx, ny);

                parent[next] = current;

                queue.Enqueue(next);
            }
        }

        List<Vector2Int> path =
            new List<Vector2Int>();

        Vector2Int step = end;

        while (step != start)
        {
            path.Add(step);

            if (!parent.ContainsKey(step))
                break;

            step = parent[step];
        }

        path.Reverse();

        return path;
    }

    IEnumerator MoveAlongPath(List<Vector2Int> path)
    {
        foreach (Vector2Int cell in path)
        {
            Vector3 targetPos = GridToWorld(cell);

            while (Vector3.Distance(transform.position, targetPos) > 0.01f)
            {
                transform.position =
                    Vector3.MoveTowards(
                        transform.position,
                        targetPos,
                        moveSpeed * Time.deltaTime);

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