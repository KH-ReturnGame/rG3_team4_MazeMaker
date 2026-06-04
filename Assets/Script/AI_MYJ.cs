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

    private float mapMinX = -9.0f;
    private float mapMinY = -4.5f;

    private List<Vector2Int> path = new List<Vector2Int>();

    void Start()
    {
        Debug.Log("AI_Dijkstra Start");

        if (startPoint == null || endPoint == null)
        {
            Debug.Log("StartPoint 또는 EndPoint 없음");
            return;
        }

        transform.position = startPoint.position;

        Vector2Int start = WorldToGrid(startPoint.position);
        Vector2Int end = WorldToGrid(endPoint.position);

        bool success = Dijkstra(start, end);

        Debug.Log("Dijkstra 결과: " + success);
        Debug.Log("Path Count: " + path.Count);

        if (success)
            StartCoroutine(MoveAlongPath());
    }

    bool Dijkstra(Vector2Int start, Vector2Int end)
    {
        int cols = mazeData.cols;
        int rows = mazeData.rows;

        // 각 노드까지의 최단 거리
        int[,] dist = new int[cols, rows];
        // 경로 역추적용 부모 노드
        Vector2Int[,] parent = new Vector2Int[cols, rows];

        // 초기화
        for (int x = 0; x < cols; x++)
            for (int y = 0; y < rows; y++)
            {
                dist[x, y] = int.MaxValue;
                parent[x, y] = new Vector2Int(-1, -1);
            }

        dist[start.x, start.y] = 0;

        // (거리, 좌표) 형태의 우선순위 큐 역할을 하는 SortedSet
        // Unity에 PriorityQueue가 없으므로 List + 정렬로 대체
        List<(int cost, Vector2Int node)> openList = new List<(int, Vector2Int)>();
        openList.Add((0, start));

        int[] dx = { 1, -1, 0, 0 };
        int[] dy = { 0, 0, 1, -1 };

        while (openList.Count > 0)
        {
            // 비용이 가장 작은 노드 꺼내기
            openList.Sort((a, b) => a.cost.CompareTo(b.cost));
            var (curCost, cur) = openList[0];
            openList.RemoveAt(0);

            // 이미 더 짧은 경로로 처리된 노드면 스킵
            if (curCost > dist[cur.x, cur.y])
                continue;

            // 도착점 도달
            if (cur == end)
            {
                ReconstructPath(parent, start, end);
                return true;
            }

            // 인접 노드 탐색
            for (int i = 0; i < 4; i++)
            {
                int nx = cur.x + dx[i];
                int ny = cur.y + dy[i];

                // 범위 체크
                if (nx < 0 || ny < 0 || nx >= cols || ny >= rows)
                    continue;

                int idx = nx + ny * cols;

                // 벽이면 스킵
                if (mazeData.tileTypes[idx] == 0)
                    continue;

                // 이동 비용: 모든 타일을 동일하게 1로 처리
                int newCost = dist[cur.x, cur.y] + 1;

                if (newCost < dist[nx, ny])
                {
                    dist[nx, ny] = newCost;
                    parent[nx, ny] = cur;
                    openList.Add((newCost, new Vector2Int(nx, ny)));
                }
            }
        }

        // 경로 없음
        return false;
    }

    // parent 배열을 역추적해서 path 복원
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
        path.Reverse(); // 시작 → 끝 순서로 뒤집기
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