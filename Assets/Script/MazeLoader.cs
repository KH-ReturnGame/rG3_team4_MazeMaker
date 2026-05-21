using UnityEngine;

public class MazeLoader : MonoBehaviour
{
    public MazeData mazeData;
    public GameObject blockPrefab;
    public GameObject obstaclePrefab;
    public GameObject boostPrefab;

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
                float wx = mapMinX + x * cellSize;
                float wy = mapMinY + y * cellSize;
                Vector3 pos = new Vector3(wx, wy, 0);

                int idx = x + y * mazeData.cols;
                int type = mazeData.tileTypes[idx];

                GameObject prefab = type == 0 ? blockPrefab
                                  : type == 1 ? obstaclePrefab
                                  : boostPrefab;

                Instantiate(prefab, pos, Quaternion.identity);
            }
        }
    }
}