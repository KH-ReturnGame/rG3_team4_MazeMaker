using UnityEngine;
using UnityEngine.SceneManagement;

public class GameEndManager : MonoBehaviour
{
    public static GameEndManager Instance;

    [Header("UI 캔버스")]
    public GameObject winCanvas;
    public GameObject loseCanvas;

    private bool gameOver = false;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        winCanvas.SetActive(false);
        loseCanvas.SetActive(false);
    }

    public void OnPlayerReached()
    {
        if (gameOver) return;
        gameOver = true;
        Debug.Log("Player Win!");
        winCanvas.SetActive(true);
    }

    public void OnAIReached()
    {
        if (gameOver) return;
        gameOver = true;
        Debug.Log("AI Win!");
        loseCanvas.SetActive(true);
    }

    // 다시 시작 버튼
    public void OnClickRestart()
    {
        SceneManager.LoadScene("Title");
    }
}