using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarAI : MonoBehaviour
{
    private const int INF = 1000000000;

    [SerializeField] private int visionRange = 3;
    public float moveSpeed = 2f;
    public float cellSize = 0.5f;
    public int movePerSearch = 5;

    private class Node
    {
        public int x, y, g, h;
        public int F => g + h;

        public Node(int x, int y, int g, int h)
        {
            this.x = x; this.y = y;
            this.g = g; this.h = h;
        }
    }

    private struct Parent
    {
        public int px, py;
        public Parent(int px, int py) { this.px = px; this.py = py; }
    }

    private int[] dx = { -1, 1, 0, 0 };
    private int[] dy = { 0, 0, -1, 1 };

    private Vector2Int goal;

    private float mapMinX = -8.0f;
    private float mapMinY = -4.5f;

    public MazeData mazeData;

    void Start()
    {
        goal = new Vector2Int(mazeData.endX, mazeData.endY);

        Vector2Int start = new Vector2Int(mazeData.aiStartX, mazeData.aiStartY);
        transform.position = GridToWorld(start);

        Debug.Log($"AStarAI 출발: ({start.x}, {start.y}) → 목표: ({goal.x}, {goal.y})");
    }

    public IEnumerator TakeTurn()
    {
        Vector2Int current = WorldToGrid(transform.position);

        if (current == goal)
        {
            Debug.Log("AStarAI 도착!");
            yield break;
        }

        List<Vector2Int> localPath = LocalAStar(current, goal, visionRange);

        if (localPath == null || localPath.Count < 2)
        {
            Debug.Log("AStarAI 길 없음");
            yield break;
        }

        int moveCount = Mathf.Min(movePerSearch, localPath.Count - 1);

        for (int i = 1; i <= moveCount; i++)
        {
            yield return MoveToCell(localPath[i]);
        }

        // AI_BMK, AI_MYJ, AStarAI 각각 TakeTurn() 끝에 추가
        if (WorldToGrid(transform.position) == goal)
        {
            if (GameEndManager.Instance != null)
                GameEndManager.Instance.OnAIReached();
        }
    }

    public List<Vector2Int> LocalAStar(Vector2Int start, Vector2Int goalPos, int range)
    {
        int n = mazeData.cols;
        int m = mazeData.rows;

        int sx = start.x, sy = start.y;
        int gx = goalPos.x, gy = goalPos.y;

        bool goalInRange = (System.Math.Abs(gx - sx) + System.Math.Abs(gy - sy)) <= range;

        List<Node> openList = new List<Node>();
        bool[,] closed = new bool[n, m];
        int[,] gCost = new int[n, m];
        Parent[,] parent = new Parent[n, m];

        for (int i = 0; i < n; i++)
            for (int j = 0; j < m; j++)
            {
                gCost[i, j] = INF;
                parent[i, j] = new Parent(-1, -1);
            }

        gCost[sx, sy] = 0;
        openList.Add(new Node(sx, sy, 0, Heuristic(sx, sy, gx, gy)));

        Node bestEdgeNode = null;

        while (openList.Count > 0)
        {
            Node cur = GetLowestFNode(openList);
            openList.Remove(cur);

            int x = cur.x, y = cur.y;

            if (closed[x, y]) continue;
            closed[x, y] = true;

            if (x == gx && y == gy)
                return ReconstructPath(parent, sx, sy, gx, gy);

            for (int dir = 0; dir < 4; dir++)
            {
                int nx = x + dx[dir];
                int ny = y + dy[dir];

                if (nx < 0 || ny < 0 || nx >= n || ny >= m) continue;

                int idx = nx + ny * mazeData.cols;
                if (mazeData.tileTypes[idx] == 0) continue;
                if (closed[nx, ny]) continue;

                int distFromStart = System.Math.Abs(nx - sx) + System.Math.Abs(ny - sy);
                if (distFromStart > range) continue;

                int newG = gCost[x, y] + 1;
                if (newG >= gCost[nx, ny]) continue;

                gCost[nx, ny] = newG;
                parent[nx, ny] = new Parent(x, y);

                int h = Heuristic(nx, ny, gx, gy);
                Node neighbor = new Node(nx, ny, newG, h);
                openList.Add(neighbor);

                if (!goalInRange)
                {
                    if (bestEdgeNode == null || h < bestEdgeNode.h ||
                        (h == bestEdgeNode.h && newG < bestEdgeNode.g))
                        bestEdgeNode = neighbor;
                }
            }
        }

        if (!goalInRange && bestEdgeNode != null)
            return ReconstructPath(parent, sx, sy, bestEdgeNode.x, bestEdgeNode.y);

        return null;
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

    List<Vector2Int> ReconstructPath(Parent[,] parent, int sx, int sy, int tx, int ty)
    {
        List<Vector2Int> path = new List<Vector2Int>();
        int cx = tx, cy = ty;

        while (!(cx == sx && cy == sy))
        {
            path.Add(new Vector2Int(cx, cy));
            Parent p = parent[cx, cy];
            if (p.px == -1) return null;
            cx = p.px;
            cy = p.py;
        }

        path.Add(new Vector2Int(sx, sy));
        path.Reverse();
        return path;
    }

    int Heuristic(int x1, int y1, int x2, int y2)
        => System.Math.Abs(x1 - x2) + System.Math.Abs(y1 - y2);

    Node GetLowestFNode(List<Node> openList)
    {
        Node best = openList[0];
        for (int i = 1; i < openList.Count; i++)
        {
            if (openList[i].F < best.F ||
                (openList[i].F == best.F && openList[i].h < best.h))
                best = openList[i];
        }
        return best;
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