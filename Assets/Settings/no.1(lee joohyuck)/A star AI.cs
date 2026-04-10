using System.Collections.Generic;
using UnityEngine;

public class AStarAI : MonoBehaviour
{
    private const int INF = 1000000000;

    private class Node
    {
        public int x;
        public int y;
        public int g;
        public int h;

        public int F => g + h;

        public Node(int x, int y, int g, int h)
        {
            this.x = x;
            this.y = y;
            this.g = g;
            this.h = h;
        }
    }

    private struct Parent
    {
        public int px;
        public int py;

        public Parent(int px, int py)
        {
            this.px = px;
            this.py = py;
        }
    }

    private int[] dx = { -1, 1, 0, 0 };
    private int[] dy = { 0, 0, -1, 1 };

    void Start()
    {
        int[,] maze =
        {
            {0, 0, 0, 1, 0},
            {1, 1, 0, 1, 0},
            {0, 0, 0, 0, 0},
            {0, 1, 1, 1, 0},
            {0, 0, 0, 1, 0}
        };

        Vector2Int start = new Vector2Int(0, 0);
        Vector2Int goal = new Vector2Int(4, 4);

        List<Vector2Int> path = AStar(maze, start, goal);

        if (path.Count == 0)
        {
            Debug.Log("경로를 찾을 수 없습니다.");
        }
        else
        {
            string result = "찾은 경로: ";
            foreach (Vector2Int p in path)
            {
                result += $"({p.x}, {p.y}) ";
            }
            Debug.Log(result);
        }
    }

    int Heuristic(int x1, int y1, int x2, int y2)
    {
        return Mathf.Abs(x1 - x2) + Mathf.Abs(y1 - y2); // 맨해튼 거리
    }

    Node GetLowestFNode(List<Node> openList)
    {
        Node bestNode = openList[0];

        for (int i = 1; i < openList.Count; i++)
        {
            if (openList[i].F < bestNode.F)
            {
                bestNode = openList[i];
            }
            else if (openList[i].F == bestNode.F && openList[i].h < bestNode.h)
            {
                bestNode = openList[i];
            }
        }

        return bestNode;
    }

    public List<Vector2Int> AStar(int[,] maze, Vector2Int start, Vector2Int goal)
    {
        int n = maze.GetLength(0);
        int m = maze.GetLength(1);

        List<Node> openList = new List<Node>();
        bool[,] closed = new bool[n, m];
        int[,] gCost = new int[n, m];
        Parent[,] parent = new Parent[n, m];

        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < m; j++)
            {
                gCost[i, j] = INF;
                parent[i, j] = new Parent(-1, -1);
            }
        }

        int sx = start.x;
        int sy = start.y;
        int gx = goal.x;
        int gy = goal.y;

        gCost[sx, sy] = 0;
        openList.Add(new Node(sx, sy, 0, Heuristic(sx, sy, gx, gy)));

        while (openList.Count > 0)
        {
            Node current = GetLowestFNode(openList);
            openList.Remove(current);

            int x = current.x;
            int y = current.y;

            if (closed[x, y])
                continue;

            closed[x, y] = true;

            if (x == gx && y == gy)
            {
                List<Vector2Int> path = new List<Vector2Int>();
                int cx = gx;
                int cy = gy;

                while (!(cx == sx && cy == sy))
                {
                    path.Add(new Vector2Int(cx, cy));
                    Parent p = parent[cx, cy];
                    cx = p.px;
                    cy = p.py;
                }

                path.Add(new Vector2Int(sx, sy));
                path.Reverse();
                return path;
            }

            for (int dir = 0; dir < 4; dir++)
            {
                int nx = x + dx[dir];
                int ny = y + dy[dir];

                if (nx < 0 || ny < 0 || nx >= n || ny >= m)
                    continue;

                if (maze[nx, ny] == 1)
                    continue; // 벽

                if (closed[nx, ny])
                    continue;

                int newG = gCost[x, y] + 1;

                if (newG < gCost[nx, ny])
                {
                    gCost[nx, ny] = newG;
                    parent[nx, ny] = new Parent(x, y);

                    int h = Heuristic(nx, ny, gx, gy);
                    openList.Add(new Node(nx, ny, newG, h));
                }
            }
        }

        return new List<Vector2Int>(); // 경로 없음
    }
}