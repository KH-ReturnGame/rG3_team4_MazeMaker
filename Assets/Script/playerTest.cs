using UnityEngine;

public class PlayerTest : MonoBehaviour
{
    public MazeData mazeData;
    public float moveSpeed = 2f;
    public float cellSize = 0.5f;

    private float mapMinX = -9.0f;
    private float mapMinY = -4.5f;

    private bool isMoving = false;
    private Vector3 targetPos;
    private bool gameOver = false;

    void Start()
    {
        // AI_BMK와 같은 startCell에서 시작
        float sx = mapMinX + mazeData.startX * cellSize;
        float sy = mapMinY + mazeData.startY * cellSize;
        transform.position = new Vector3(sx, sy, 0);
        targetPos = transform.position;
    }

    void Update()
    {
        if (gameOver) return;

        if (!isMoving)
            TryMove();

        MoveToTarget();
    }

    void TryMove()
    {
        Vector2Int dir = Vector2Int.zero;

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) dir = Vector2Int.up;
        else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) dir = Vector2Int.down;
        else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) dir = Vector2Int.left;
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) dir = Vector2Int.right;

        if (dir == Vector2Int.zero) return;

        // 현재 그리드 위치
        Vector2Int currentCell = WorldToGrid(transform.position);
        Vector2Int nextCell = currentCell + dir;

        // 범위 체크
        if (nextCell.x < 0 || nextCell.x >= mazeData.cols) return;
        if (nextCell.y < 0 || nextCell.y >= mazeData.rows) return;

        // 벽 체크 (block == 0)
        int idx = nextCell.x + nextCell.y * mazeData.cols;
        if (mazeData.tileTypes[idx] == 0) return;

        targetPos = GridToWorld(nextCell);
        isMoving = true;
    }

    void MoveToTarget()
    {
        if (!isMoving) return;

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPos,
            moveSpeed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, targetPos) < 0.01f)
        {
            transform.position = targetPos;
            isMoving = false;

            // 도착 체크
            Vector2Int currentCell = WorldToGrid(transform.position);
            if (currentCell.x == mazeData.endX && currentCell.y == mazeData.endY)
            {
                gameOver = true;
                Debug.Log("Player Win!");
                GameManager.Instance.OnPlayerReached();
            }
        }
    }

    Vector2Int WorldToGrid(Vector3 pos)
    {
        int x = Mathf.RoundToInt((pos.x - mapMinX) / cellSize);
        int y = Mathf.RoundToInt((pos.y - mapMinY) / cellSize);
        return new Vector2Int(x, y);
    }

    Vector3 GridToWorld(Vector2Int grid)
    {
        float wx = mapMinX + grid.x * cellSize;
        float wy = mapMinY + grid.y * cellSize;
        return new Vector3(wx, wy, 0);
    }

    public void OnAIReached()
    {
        if (gameOver) return;
        gameOver = true;
        Debug.Log("AI Win!");
    }
}