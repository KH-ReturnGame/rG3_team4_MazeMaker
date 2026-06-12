using UnityEngine;

public class MazeLoader : MonoBehaviour
{
    public MazeData mazeData;

    public GameObject blockPrefab;

    public GameObject playerPrefab;
    public GameObject aiPrefab;
    public GameObject goalPrefab;

    private float mapMinX = -6.5f;
    private float mapMinY = -4.5f;

    private float cellSize = 0.5f;

    void Start()
    {
        LoadMaze();
        SpawnCharacters();
    }

    void LoadMaze()
    {
        if (mazeData == null)
        {
            Debug.LogError("MazeData가 연결되지 않았습니다.");
            return;
        }

        for (int x = 0; x < mazeData.cols; x++)
        {
            for (int y = 0; y < mazeData.rows; y++)
            {
                int idx = x + y * mazeData.cols;

                int type = mazeData.tileTypes[idx];

                // 벽이 아니면 스킵
                if (type != 0)
                    continue;

                float wx =
                    mapMinX +
                    x * cellSize;

                float wy =
                    mapMinY +
                    y * cellSize;

                Instantiate(
                    blockPrefab,
                    new Vector3(wx, wy, 0),
                    Quaternion.identity);
            }
        }
    }

    void SpawnCharacters()
    {
        Vector3 playerPos =
            new Vector3(
                mapMinX + mazeData.startX * cellSize,
                mapMinY + mazeData.startY * cellSize,
                -1);

        Vector3 aiPos =
            new Vector3(
                mapMinX + mazeData.aiStartX * cellSize,
                mapMinY + mazeData.aiStartY * cellSize,
                -1);

        Vector3 goalPos =
            new Vector3(
                mapMinX + mazeData.endX * cellSize,
                mapMinY + mazeData.endY * cellSize,
                -1);

        if (playerPrefab != null)
        {
            Instantiate(
                playerPrefab,
                playerPos,
                Quaternion.identity);
        }

        if (aiPrefab != null)
        {
            Instantiate(
                aiPrefab,
                aiPos,
                Quaternion.identity);
        }

        if (goalPrefab != null)
        {
            Instantiate(
                goalPrefab,
                goalPos,
                Quaternion.identity);
        }
    }
}