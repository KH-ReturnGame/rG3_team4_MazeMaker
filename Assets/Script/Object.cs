using UnityEngine;

public class Object : MonoBehaviour
{
    private Vector3 offset;
    void OnMouseDown()
    {
        transform.SetParent(null); // 부모 관계 해제
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0;
        offset = transform.position - mouseWorldPos;
    }

    void OnMouseDrag()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0;
        transform.position = mouseWorldPos + offset;
    }

    void OnMouseUp()
    {
        // 현재 위치에서 가장 가까운 셀 중앙에 정렬(타일맵 이용 안함-수정 가능성 O)
        float snappedX = Mathf.Floor(transform.position.x) + 0.5f;
        float snappedY = Mathf.Floor(transform.position.y) + 0.5f;
        // 맵 범위 제한 
        snappedX = Mathf.Clamp(snappedX, -8.5f, 4.5f);
        snappedY = Mathf.Clamp(snappedY, -4.5f, 4.5f);
        transform.position = new Vector3(snappedX, snappedY, 0);

        Destroy(this);
    }
}
