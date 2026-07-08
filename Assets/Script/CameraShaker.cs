using UnityEngine;
using System.Collections;

public class CameraShaker : MonoBehaviour
{
    public static CameraShaker Instance;

    [Header("Shake Settings")]
    public float duration = 0.3f;      // 흔들림 지속 시간
    public float magnitude = 0.15f;    // 흔들림 강도
    public AnimationCurve curve =      // 충격파 느낌 - 강하게 시작해서 빠르게 감소
        AnimationCurve.EaseInOut(0, 1, 1, 0);

    private Vector3 originalPos;

    void Awake()
    {
        Instance = this;
        originalPos = transform.localPosition;
    }

    public void Shake()
    {
        StopAllCoroutines();
        StartCoroutine(ShakeRoutine());
    }

    IEnumerator ShakeRoutine()
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float strength = curve.Evaluate(elapsed / duration) * magnitude;

            // 랜덤 방향으로 흔들기
            float offsetX = Random.Range(-1f, 1f) * strength;
            float offsetY = Random.Range(-1f, 1f) * strength;

            transform.localPosition = originalPos + new Vector3(offsetX, offsetY, 0);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPos;
    }
}