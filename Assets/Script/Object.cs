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
        Destroy(this); // 스크립트 컴포넌트만 제거
    }
}
