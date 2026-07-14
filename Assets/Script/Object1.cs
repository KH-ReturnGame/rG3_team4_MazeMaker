using UnityEngine;

public class ObjectBattle : MonoBehaviour
{
    private Vector3 offset;
    private bool placed = false;
    private Transform originalSlot;
    private Vector3 originalPos;

    private const float blockMinX = -2.0f;
    private const float blockMaxX = -1.0f;
    private const float blockMinY = -0.5f;
    private const float blockMaxY = 0.5f;

    void OnMouseDown()
    {
        if (!TurnManagerBattle.Instance.CanPlayerAct() &&
            !TurnManagerBattle.Instance.CanPlayer2Act())
            return;

        originalSlot = transform.parent;
        originalPos = transform.position;
        transform.SetParent(null);

        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0;
        offset = transform.position - mouseWorldPos;

        MazeData mazeData = TurnManagerBattle.Instance.mazeData;
        float mapMinX = -8.0f;
        float mapMinY = -4.5f;
        float cellSize = 0.5f;

        int gridX = Mathf.RoundToInt((originalPos.x - mapMinX) / cellSize);
        int gridY = Mathf.RoundToInt((originalPos.y - mapMinY) / cellSize);

        if (gridX >= 0 && gridY >= 0 &&
            gridX < mazeData.cols && gridY < mazeData.rows)
        {
            int idx = gridX + gridY * mazeData.cols;
            mazeData.tileTypes[idx] = 1;
        }

        placed = false;
    }

    void OnMouseDrag()
    {
        if (placed) return;

        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0;
        transform.position = mouseWorldPos + offset;
    }

    void OnMouseUp()
    {
        if (placed) return;

        float snappedX = Mathf.Floor(transform.position.x * 2) / 2 + 0.5f;
        float snappedY = Mathf.Floor(transform.position.y * 2) / 2 + 0.5f;
        snappedX = Mathf.Clamp(snappedX, -9.0f, 4.5f);
        snappedY = Mathf.Clamp(snappedY, -4.5f, 4.5f);

        Vector3 snappedPos = new Vector3(snappedX, snappedY, 0);

        if (IsInBlockedZone(snappedX, snappedY))
        {
            transform.SetParent(originalSlot);
            transform.position = originalPos;
            Debug.Log("중앙 구역엔 배치 불가!");
            return;
        }

        if (IsOccupied(snappedPos))
        {
            transform.SetParent(originalSlot);
            transform.position = originalPos;
            return;
        }

        transform.position = snappedPos;
        placed = true;

        MazeData mazeData = TurnManagerBattle.Instance.mazeData;
        float mapMinX = -8.0f;
        float mapMinY = -4.5f;
        float cellSize = 0.5f;

        int gridX = Mathf.RoundToInt((snappedX - mapMinX) / cellSize);
        int gridY = Mathf.RoundToInt((snappedY - mapMinY) / cellSize);

        if (gridX >= 0 && gridY >= 0 &&
            gridX < mazeData.cols && gridY < mazeData.rows)
        {
            int idx = gridX + gridY * mazeData.cols;
            mazeData.tileTypes[idx] = 0;
            Debug.Log($"블록 배치: 그리드({gridX},{gridY}) → 벽 처리");
        }

        CameraShaker.Instance.Shake();

        if (TurnManagerBattle.Instance.CanPlayerAct())
            TurnManagerBattle.Instance.UsePlayerAction();
        else if (TurnManagerBattle.Instance.CanPlayer2Act())
            TurnManagerBattle.Instance.UsePlayer2Action();
    }

    bool IsInBlockedZone(float x, float y)
    {
        return x >= blockMinX && x <= blockMaxX &&
               y >= blockMinY && y <= blockMaxY;
    }

    bool IsOccupied(Vector3 pos)
    {
        ObjectBattle[] allObjects = FindObjectsByType<ObjectBattle>(FindObjectsSortMode.None);
        foreach (ObjectBattle obj in allObjects)
        {
            if (obj == this) continue;
            if (obj.placed && Vector3.Distance(obj.transform.position, pos) < 0.1f)
                return true;
        }
        return false;
    }
}