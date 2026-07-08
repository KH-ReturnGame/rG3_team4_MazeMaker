using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public MazeData mazeData;

    public void LoadAIEscapeScene()
    {
        mazeData.selectedAI = Random.Range(0, 3);
        Debug.Log($"º±≈√µ» AI: {mazeData.selectedAI} (0=BMK, 1=MYJ, 2=AStar)");

        SceneManager.LoadScene("AIEscapeScene");
    }
}