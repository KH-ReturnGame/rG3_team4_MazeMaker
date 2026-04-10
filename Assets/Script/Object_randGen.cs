using UnityEngine;

public class Object_randGen : MonoBehaviour
{
    public GameObject prefab;

    public int count = 10;

    public float minX = -5f;
    public float maxX = 5f;
    public float minY = -3f;
    public float maxY = 3f;

    public void SpawnObjects()
    {
        for (int i = 0; i < count; i++)
        {
            float randX = Random.Range(minX, maxX);
            float randY = Random.Range(minY, maxY);

            Vector3 spawnPos = new Vector3(randX, randY, 0);
            Instantiate(prefab, spawnPos, Quaternion.identity);
        }
    }
}