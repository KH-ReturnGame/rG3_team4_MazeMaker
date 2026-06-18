using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarAI : MonoBehaviour
{
    private const int INF = 1000000000;

    [SerializeField] private int visionRange = 3;
    [SerializeField] private float stepDelay = 0.2f;

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

    private int[,] maze;
    private Vector2Int current;
    private Vector2Int goal;

    void Start()
    {
        maze = new int[,]
        {
            {0, 0, 0, 1, 0},
            {1, 1, 0, 1, 0},
            {0, 0, 0, 0, 0},
            {0, 1, 1, 1, 0},
            {0, 0, 0, 1, 0}
        };

        current = new Vector2Int(0, 0);
        goal = new Vector2Int(4, 4);

        StartCoroutine(RunLocalAStar());
    }

    IEnumerator RunLocalAStar()
    {
        Debug.Log($"출발: ({current.x}, {current.y}) → 목표: ({goal.x}, {goal.y})");

        while (current != goal)
        {
            List<Vector2Int> localPath = LocalAStar(maze, current, goal, visionRange);

            if (localPath == null || localPath.Count < 2)
            {
                Debug.Log("범위 내에서 다음 경로를 찾을 수 없습니다. 이동을 중단합니다.");
                yield break;
            }

            Vector2Int next = localPath[1];
            Debug.Log($"({current.x},{current.y}) → ({next.x},{next.y})  [로컬 경로 길이: {localPath.Count}]");

            current = next;

            yield return new WaitForSeconds(stepDelay);
        }

        Debug.Log($"목표 도달! 최종 위치: ({current.x}, {current.y})");
    }

    public List<Vector2Int> LocalAStar(int[,] maze, Vector2Int start, Vector2Int goalPos, int range)
    {
        int n = maze.GetLength(0);
        int m = maze.GetLength(1);

        int sx = start.x, sy = start.y;
        int gx = goalPos.x, gy = goalPos.y;

        bool goalInRange = (Mathf.Abs(gx - sx) + Mathf.Abs(gy - sy)) <= range;

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
                if (maze[nx, ny] == 1) continue;
                if (closed[nx, ny]) continue;

                int distFromStart = Mathf.Abs(nx - sx) + Mathf.Abs(ny - sy);
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
                    {
                        bestEdgeNode = neighbor;
                    }
                }
            }
        }

        if (!goalInRange && bestEdgeNode != null)
        {
            return ReconstructPath(parent, sx, sy, bestEdgeNode.x, bestEdgeNode.y);
        }

        return null;
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
        => Mathf.Abs(x1 - x2) + Mathf.Abs(y1 - y2);

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
}