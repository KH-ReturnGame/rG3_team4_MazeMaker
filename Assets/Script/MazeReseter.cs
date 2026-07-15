using UnityEngine;

public class MazeReseter : MonoBehaviour
{
    public MazeData mazeData;

    private void Awake()
    {
        mazeData.ClearData();
        Debug.Log("미로 데이터를 초기화했습니다.");
    }
}