using UnityEngine;

[CreateAssetMenu(fileName = "MazeData", menuName = "Maze/MazeData")]
public class MazeData : ScriptableObject
{
    public int cols;
    public int rows;

    public bool[] isWall;
    public int[] tileTypes;

    public int startX;
    public int startY;

    public int endX;
    public int endY;

    public int aiStartX;
    public int aiStartY;

    public int selectedAI;

    public void SaveData(
        int cols,
        int rows,
        bool[,] walls,
        Vector2Int playerStart,
        Vector2Int goal,
        Vector2Int aiStart)
    {
        this.cols = cols;
        this.rows = rows;

        isWall = new bool[cols * rows];
        tileTypes = new int[cols * rows];

        startX = playerStart.x;
        startY = playerStart.y;

        endX = goal.x;
        endY = goal.y;

        aiStartX = aiStart.x;
        aiStartY = aiStart.y;

        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < cols; x++)
            {
                int index = x + y * cols;

                isWall[index] = walls[x, y];
                tileTypes[index] = walls[x, y] ? 0 : 3;
            }
        }
    }
}