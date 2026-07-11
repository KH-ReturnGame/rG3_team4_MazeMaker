using UnityEngine;

public class Position : MonoBehaviour
{
    [Header("보드 설정")]
    public int columns = 27;
    public int rows = 19;

    [Header("보드 크기")]
    public float boardWidth = 13.50071f;
    public float boardHeight = 9.498872f;

    [Header("점 설정")]
    public float pointRadius = 0.05f;
    public Color pointColor = Color.red;

    private void OnDrawGizmos()
    {
        Gizmos.color = pointColor;

        float cellWidth = boardWidth / columns;
        float cellHeight = boardHeight / rows;

        Vector3 bottomLeft = transform.position - new Vector3(boardWidth / 2f, boardHeight / 2f, 0);

        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < columns; x++)
            {
                Vector3 pos = bottomLeft +
                              new Vector3(
                                  (x + 0.5f) * cellWidth,
                                  (y + 0.5f) * cellHeight,
                                  0);

                Gizmos.DrawSphere(pos, pointRadius);
            }
        }
    }
}
