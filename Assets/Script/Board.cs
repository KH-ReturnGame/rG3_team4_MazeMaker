using UnityEngine;
public class Board : MonoBehaviour
{
    public Transform[] slots;
    public GameObject tilePrefab;

    void Update()
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