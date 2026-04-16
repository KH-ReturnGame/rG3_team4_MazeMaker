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
        // 현재 위치에서 가장 가까운 셀 중앙으로 스냅
        float snappedX = Mathf.Floor(transform.position.x) + 0.5f;
        float snappedY = Mathf.Floor(transform.position.y) + 0.5f;
        transform.position = new Vector3(snappedX, snappedY, 0);

        Destroy(this);
    }
}
