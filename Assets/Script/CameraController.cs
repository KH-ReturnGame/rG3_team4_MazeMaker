using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance;

    void Awake()
    {
        Instance = this;
    }

    public IEnumerator FocusOnAI(Transform target)
    {
        yield break;
    }

    public IEnumerator ReturnToDefault()
    {
        yield break;
    }
}