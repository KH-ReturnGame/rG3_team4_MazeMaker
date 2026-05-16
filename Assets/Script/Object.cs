using UnityEngine;

public class Object : MonoBehaviour
{
    private Vector3 offset;
    private bool placed = false;
    private Transform originalSlot;
    private Vector3 originalPos;
    public Tile tiledata; //오브젝트랑 연결

    void OnMouseDown()
    {
        if (placed) return;

        originalSlot = transform.parent;
        originalPos = transform.position; //원래 위치 저장

        transform.SetParent(null); // 부모 관계 해제
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0;
        offset = transform.position - mouseWorldPos;
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

        // 현재 위치에서 가장 가까운 셀 중앙에 정렬(타일맵 이용 안함-수정 가능성 O)
        float snappedX = Mathf.Floor(transform.position.x*2)/2 + 0.5f;
        float snappedY = Mathf.Floor(transform.position.y*2)/2 + 0.5f;
        // 맵 범위 제한 
        snappedX = Mathf.Clamp(snappedX, -9.0f, 4.5f);
        snappedY = Mathf.Clamp(snappedY, -4.5f, 4.5f);
        Vector3 snappedPos = new Vector3(snappedX, snappedY, 0);

        if (IsOccupied(snappedPos))
        {
            // 배치 실패하면 원래 위치로
            transform.SetParent(originalSlot);
            transform.position = originalPos;
            return;
        }

        transform.position = snappedPos;
        placed = true;
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
