using UnityEngine;

public class MazeLoader : MonoBehaviour
{
    public MazeData mazeData;
    public GameObject blockPrefab;
    public GameObject obstaclePrefab;
    public GameObject boostPrefab;
    public GameObject emptyPrefab; // ¾øÀ¸¸é null·Î µÎ¸é ½ºÅµµÊ

    private float mapMinX = -9.0f;
    private float mapMinY = -4.5f;
    private float cellSize = 0.5f;

    void Start()
    {
        LoadMaze();
    }

    void LoadMaze()
    {
        for (int x = 0; x < mazeData.cols; x++)
        {
            for (int y = 0; y < mazeData.rows; y++)
            {
                int idx = x + y * mazeData.cols;
                int type = mazeData.tileTypes[idx];

                GameObject prefab = type == 0 ? blockPrefab
                                  : type == 1 ? obstaclePrefab
                                  : type == 2 ? boostPrefab
                                  : emptyPrefab; // type == 3

                if (prefab == null) continue; // emptyPrefab ¾øÀ¸¸é ½ºÅµ

                float wx = mapMinX + x * cellSize;
                float wy = mapMinY + y * cellSize;
                Instantiate(prefab, new Vector3(wx, wy, 0), Quaternion.identity);
            }
        }
    }
}