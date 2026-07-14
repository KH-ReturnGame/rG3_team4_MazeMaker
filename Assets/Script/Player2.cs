using UnityEngine;

public class Player2 : MonoBehaviour
{
    public MazeData mazeData;
    public float moveSpeed = 2f;
    public float cellSize = 0.5f;

    private float mapMinX = -8.0f;
    private float mapMinY = -4.5f;

    private bool isMoving = false;
    private Vector3 targetPos;
    private bool gameOver = false;

    void Start()
    {
        Vector2Int startCell = new Vector2Int(mazeData.aiStartX, mazeData.aiStartY);
        transform.position = GridToWorld(startCell);
        targetPos = transform.position;
    }

    void Update()
    {
        if (gameOver) return;
        if (!isMoving) TryMove();
        MoveToTarget();
    }

    void TryMove()
    {
        if (!TurnManagerBattle.Instance.CanPlayer2Act()) return;

        Vector2Int dir = Vector2Int.zero;

        if (Input.GetKey(KeyCode.I)) dir = Vector2Int.up;
        else if (Input.GetKey(KeyCode.K)) dir = Vector2Int.down;
        else if (Input.GetKey(KeyCode.J)) dir = Vector2Int.left;
        else if (Input.GetKey(KeyCode.L)) dir = Vector2Int.right;

        if (dir == Vector2Int.zero) return;

        Vector2Int currentCell = WorldToGrid(transform.position);
        Vector2Int nextCell = currentCell + dir;

        if (nextCell.x < 0 || nextCell.x >= mazeData.cols) return;
        if (nextCell.y < 0 || nextCell.y >= mazeData.rows) return;

        int idx = nextCell.x + nextCell.y * mazeData.cols;
        if (mazeData.tileTypes[idx] == 0) return;

        targetPos = GridToWorld(nextCell);
        isMoving = true;
        TurnManagerBattle.Instance.UsePlayer2Action();
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

            Vector2Int currentCell = WorldToGrid(transform.position);
            if (currentCell.x == mazeData.endX && currentCell.y == mazeData.endY)
            {
                gameOver = true;
                Debug.Log("Player2 Win!");
                if (GameEndManager.Instance != null)
                    GameEndManager.Instance.OnPlayer2Reached();
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

    public void OnOpponentReached()
    {
        if (gameOver) return;
        gameOver = true;
        Debug.Log("Player1 Win! Player2 Lose!");
    }
}