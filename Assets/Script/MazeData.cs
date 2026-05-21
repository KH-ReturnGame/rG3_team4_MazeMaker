using UnityEngine;

[CreateAssetMenu(fileName = "MazeData", menuName = "Maze/MazeData")]
public class MazeData : ScriptableObject
{
    public int cols;
    public int rows;
    public bool[] isWall;
    public int[] tileTypes;
}