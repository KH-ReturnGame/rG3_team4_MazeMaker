using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance;

    [Header("기본 상태")]
    public float defaultSize = 5f;         // 평소 orthographicSize
    public Vector3 defaultPosition;        // 평소 카메라 위치

    [Header("AI 포커스 상태")]
    public float focusSize = 1.5f;         // 확대 시 orthographicSize
    public float followSpeed = 5f;         // AI 따라가는 속도
    public float zoomSpeed = 3f;           // 줌 속도

    private Camera cam;
    private Transform followTarget;        // 현재 따라갈 대상
    private bool isFollowing = false;

    void Awake()
    {
        Instance = this;
        cam = GetComponent<Camera>();
    }

    void Start()
    {
        defaultPosition = transform.position;
    }

    void LateUpdate()
    {
        if (isFollowing && followTarget != null)
        {
            // AI 위치로 부드럽게 이동
            Vector3 targetPos = new Vector3(
                followTarget.position.x,
                followTarget.position.y,
                transform.position.z
            );
            transform.position = Vector3.Lerp(
                transform.position,
                targetPos,
                followSpeed * Time.deltaTime
            );

            // 줌인
            cam.orthographicSize = Mathf.Lerp(
                cam.orthographicSize,
                focusSize,
                zoomSpeed * Time.deltaTime
            );
        }
    }

    public IEnumerator FocusOnAI(Transform target)
    {
        followTarget = target;
        isFollowing = true;
        yield break;
    }

    public IEnumerator ReturnToDefault()
    {
        isFollowing = false;
        followTarget = null;

        // 원래 위치/줌으로 부드럽게 복귀
        float elapsed = 0f;
        float duration = 0.5f;

        Vector3 startPos = transform.position;
        float startSize = cam.orthographicSize;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            transform.position = Vector3.Lerp(startPos, defaultPosition, t);
            cam.orthographicSize = Mathf.Lerp(startSize, defaultSize, t);

            yield return null;
        }

        transform.position = defaultPosition;
        cam.orthographicSize = defaultSize;
    }
}