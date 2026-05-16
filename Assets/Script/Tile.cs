using UnityEngine;

public enum TileType
{
    Block,
    Obstacle,
    Boost
}

public class Tile : MonoBehaviour
{
    public TileType type; // 타일 종류
    public int point;     // 포인트
}