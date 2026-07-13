using UnityEngine;

public class Position : MonoBehaviour
{
    [Header("보드 설정")]
    public int columns = 27;
    public int rows = 19;

    [Header("보드 크기")]
    public float boardWidth = 13.50071f;
    public float boardHeight = 9.498872f;

    [Header("선 설정")]
    public Color lineColor = Color.red;

    private Material lineMaterial;

    void OnEnable()
    {
        Camera.onPostRender += DrawGrid;
    }

    void OnDisable()
    {
        Camera.onPostRender -= DrawGrid;
    }

    void Start()
    {
        Shader shader = Shader.Find("Hidden/Internal-Colored");
        lineMaterial = new Material(shader);
        lineMaterial.hideFlags = HideFlags.HideAndDontSave;
        lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
        lineMaterial.SetInt("_ZWrite", 0);
    }

    void DrawGrid(Camera cam)
    {
        if (lineMaterial == null) return;

        lineMaterial.SetPass(0);

        GL.PushMatrix();
        GL.LoadProjectionMatrix(cam.projectionMatrix);
        GL.modelview = cam.worldToCameraMatrix;

        GL.Begin(GL.LINES);
        GL.Color(lineColor);

        float cellWidth = boardWidth / columns;
        float cellHeight = boardHeight / rows;

        Vector3 bottomLeft = transform.position
            - new Vector3(boardWidth / 2f, boardHeight / 2f, 0);
        Vector3 topRight = bottomLeft
            + new Vector3(boardWidth, boardHeight, 0);

        // 세로선
        for (int x = 0; x <= columns; x++)
        {
            float px = bottomLeft.x + x * cellWidth;
            GL.Vertex3(px, bottomLeft.y, 0);
            GL.Vertex3(px, topRight.y, 0);
        }

        // 가로선
        for (int y = 0; y <= rows; y++)
        {
            float py = bottomLeft.y + y * cellHeight;
            GL.Vertex3(bottomLeft.x, py, 0);
            GL.Vertex3(topRight.x, py, 0);
        }

        GL.End();
        GL.PopMatrix();
    }

    void OnDrawGizmos()
    {
        Gizmos.color = lineColor;

        float cellWidth = boardWidth / columns;
        float cellHeight = boardHeight / rows;

        Vector3 bottomLeft = transform.position
            - new Vector3(boardWidth / 2f, boardHeight / 2f, 0);
        Vector3 topRight = bottomLeft
            + new Vector3(boardWidth, boardHeight, 0);

        for (int x = 0; x <= columns; x++)
        {
            float px = bottomLeft.x + x * cellWidth;
            Gizmos.DrawLine(
                new Vector3(px, bottomLeft.y, 0),
                new Vector3(px, topRight.y, 0)
            );
        }

        for (int y = 0; y <= rows; y++)
        {
            float py = bottomLeft.y + y * cellHeight;
            Gizmos.DrawLine(
                new Vector3(bottomLeft.x, py, 0),
                new Vector3(topRight.x, py, 0)
            );
        }
    }
}