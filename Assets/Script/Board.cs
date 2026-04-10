using UnityEngine;

public class Board : MonoBehaviour
{
    public Transform[] slots;
    public GameObject tilePrefab;

    void Update() // 매 프레임마다 실행
    {
        FillEmptySlots();
    }

    void FillEmptySlots()
    {
        foreach (Transform slot in slots)
        {
            if (slot.childCount == 0)
            {
                GameObject instance = Instantiate(tilePrefab, slot.position, Quaternion.identity);
                instance.transform.SetParent(slot);
            }
        }
    }
}