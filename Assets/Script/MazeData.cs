using UnityEngine;

[CreateAssetMenu(fileName = "MazeData", menuName = "Maze/MazeData")]
public class MazeData : ScriptableObject
{
    public int cols;
    public int rows;

    public bool[] isWall;
    public int[] tileTypes;

    // 플레이어 시작점
    public int startX;
    public int startY;

    // 목표점
    public int endX;
    public int endY;

    // AI 시작점
    public int aiStartX;
    public int aiStartY;
}