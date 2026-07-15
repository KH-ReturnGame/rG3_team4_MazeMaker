using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;




public class SceneLoader : MonoBehaviour
{
    public MazeData mazeData;
    public GameObject noMazeMessage;
    public float messageTime = 2f;

    private IEnumerator ShowNoMazeMessage()
    {
        noMazeMessage.SetActive(true);
        yield return new WaitForSeconds(messageTime);
        noMazeMessage.SetActive(false);
    }

    public void LoadAIEscapeScene()
    {
        if (!HasMaze())
        {
            StartCoroutine(ShowNoMazeMessage());
            return;
        }
        mazeData.selectedAI = Random.Range(0, 3);
        Debug.Log($"º±≈√µ» AI: {mazeData.selectedAI} (0=BMK, 1=MYJ, 2=AStar)");
        SceneManager.LoadScene("RouletteScene");
    }

    public void LoadPlayerBattleScene()
    {
        if (!HasMaze())
        {
            StartCoroutine(ShowNoMazeMessage());
            return;
        }
        SceneManager.LoadScene("PlayerBattleScene");
    }
    private bool HasMaze()
    {
        return mazeData != null && mazeData.hasMaze;
    }
}