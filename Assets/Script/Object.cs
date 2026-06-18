using UnityEngine;

public class Object : MonoBehaviour
{
    private Vector3 offset;
    private bool placed = false;
    private Transform originalSlot;
    private Vector3 originalPos;

    void OnMouseDown()
    {
        if (!TurnManager.Instance.CanPlayerAct())
            return;

        originalSlot = transform.parent;
        originalPos = transform.position;

        transform.SetParent(null);

        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0;
        offset = transform.position - mouseWorldPos;

        // 기존 위치를 빈칸으로 변경
        MazeData mazeData = TurnManager.Instance.mazeData;
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

        if (IsOccupied(snappedPos))
        {
            transform.SetParent(originalSlot);
            transform.position = originalPos;
            return;
        }

        transform.position = snappedPos;
        placed = true;

        // mazeData tileTypes 갱신
        MazeData mazeData = TurnManager.Instance.mazeData;
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
            Debug.Log("Object MazeData = " + mazeData.GetInstanceID());
        }

        TurnManager.Instance.UsePlayerAction();
    }

    bool IsOccupied(Vector3 pos)
    {
        Object[] allObjects = FindObjectsByType<Object>(FindObjectsSortMode.None);
        foreach (Object obj in allObjects)
        {
            if (obj == this) continue;
            if (obj.placed && Vector3.Distance(obj.transform.position, pos) < 0.1f)
                return true;
        }
        return false;
    }
}