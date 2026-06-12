using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AI_BMK : MonoBehaviour
{
    public MazeData mazeData;

    public float moveSpeed = 2f;
    public float cellSize = 0.5f;

    private float mapMinX = -6.5f;
    private float mapMinY = -4.5f;

    public int movePerSearch = 5;

    private Vector2Int goal;

    void Start()
    {
        goal = new Vector2Int(
            mazeData.endX,
            mazeData.endY
        );

        Vector2Int start =
            new Vector2Int(
                mazeData.aiStartX,
                mazeData.aiStartY
            );

        transform.position =
            GridToWorld(start);

        StartCoroutine(AILoop());
    }

    IEnumerator AILoop()
    {
        while (true)
        {
            Vector2Int current =
                WorldToGrid(transform.position);

            if (current == goal)
            {
                Debug.Log("AI 도착!");
                yield break;
            }

            List<Vector2Int> path =
                FindPathBFS(current, goal);

            if (path == null)
            {
                Debug.Log("길 없음");
                yield return new WaitForSeconds(0.2f);
                continue;
            }

            int moveCount =
                Mathf.Min(
                    movePerSearch,
                    path.Count - 1
                );

            for (int i = 1; i <= moveCount; i++)
            {
                yield return MoveToCell(path[i]);
            }

            yield return null;
        }
    }

    List<Vector2Int> FindPathBFS( //dfs가 계속 오류나서 임시로 bfs를 좀 쓰겠슴
        Vector2Int start,
        Vector2Int end)
    {
        int cols = mazeData.cols;
        int rows = mazeData.rows;

        bool[,] visited =
            new bool[cols, rows];

        Vector2Int[,] parent =
            new Vector2Int[cols, rows];

        Queue<Vector2Int> queue =
            new Queue<Vector2Int>();

        queue.Enqueue(start);
        visited[start.x, start.y] = true;

        int[] dx = { 1, -1, 0, 0 };
        int[] dy = { 0, 0, 1, -1 };

        bool found = false;

        while (queue.Count > 0)
        {
            Vector2Int current =
                queue.Dequeue();

            if (current == end)
            {
                found = true;
                break;
            }

            for (int i = 0; i < 4; i++)
            {
                int nx = current.x + dx[i];
                int ny = current.y + dy[i];

                if (nx < 0 ||
                    ny < 0 ||
                    nx >= cols ||
                    ny >= rows)
                    continue;

                if (visited[nx, ny])
                    continue;

                int idx =
                    nx + ny * cols;

                if (mazeData.tileTypes[idx] == 0)
                    continue;

                visited[nx, ny] = true;

                parent[nx, ny] =
                    current;

                queue.Enqueue(
                    new Vector2Int(nx, ny));
            }
        }

        if (!found)
            return null;

        List<Vector2Int> path =
            new List<Vector2Int>();

        Vector2Int node = end;

        while (node != start)
        {
            path.Add(node);
            node = parent[node.x, node.y];
        }

        path.Add(start);

        path.Reverse();

        return path;
    }

    IEnumerator MoveToCell(
        Vector2Int targetCell)
    {
        Vector3 targetPos =
            GridToWorld(targetCell);

        while (
            Vector3.Distance(
                transform.position,
                targetPos)
            > 0.01f)
        {
            transform.position =
                Vector3.MoveTowards(
                    transform.position,
                    targetPos,
                    moveSpeed * Time.deltaTime
                );

            yield return null;
        }

        transform.position =
            targetPos;
    }

    Vector2Int WorldToGrid(
        Vector3 pos)
    {
        int x =
            Mathf.RoundToInt(
                (pos.x - mapMinX)
                / cellSize);

        int y =
            Mathf.RoundToInt(
                (pos.y - mapMinY)
                / cellSize);

        return new Vector2Int(x, y);
    }

    Vector3 GridToWorld(
        Vector2Int grid)
    {
        float wx =
            mapMinX +
            grid.x * cellSize;

        float wy =
            mapMinY +
            grid.y * cellSize;

        return new Vector3(
            wx,
            wy,
            -1
        );
    }
}