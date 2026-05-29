using UnityEngine;

[CreateAssetMenu(fileName = "MazeData", menuName = "Maze/MazeData")]
public class MazeData : ScriptableObject
{
    public int cols;
    public int rows;
    public bool[] isWall;
    public int[] tileTypes;  // 0=Block, 1=Obstacle, 2=Boost, 3=Empty

    public int startX;
    public int startY;
    public int endX;
    public int endY;

    public int totalBudget = 5000;
    public int usedCost;
}