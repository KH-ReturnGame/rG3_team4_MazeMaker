using UnityEngine;
using UnityEngine.SceneManagement;

public class GameEndManager : MonoBehaviour
{
    public static GameEndManager Instance;

    [Header("UI Äµ¹ö½º")]
    public GameObject winCanvas;
    public GameObject loseCanvas;
    public GameObject player2WinCanvas; // Player2 ½Â¸® ½Ã (PlayerBattleScene¿ë)

    private bool gameOver = false;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (winCanvas != null) winCanvas.SetActive(false);
        if (loseCanvas != null) loseCanvas.SetActive(false);
        if (player2WinCanvas != null) player2WinCanvas.SetActive(false);
    }

    // Player1 µµÂø (AI vs Player ¾À¿¡¼­ ÇÃ·¹ÀÌ¾î ½Â¸®)
    public void OnPlayerReached()
    {
        if (gameOver) return;
        gameOver = true;
        Debug.Log("Player1 Win!");
        if (winCanvas != null) winCanvas.SetActive(true);
    }

    // AI µµÂø (AI vs Player ¾À¿¡¼­ AI ½Â¸®)
    public void OnAIReached()
    {
        if (gameOver) return;
        gameOver = true;
        Debug.Log("AI Win!");
        if (loseCanvas != null) loseCanvas.SetActive(true);
    }

    // Player1 µµÂø (PlayerBattleScene)
    public void OnPlayer1Reached()
    {
        if (gameOver) return;
        gameOver = true;
        Debug.Log("Player1 Win!");
        if (winCanvas != null) winCanvas.SetActive(true);
    }

    // Player2 µµÂø (PlayerBattleScene)
    public void OnPlayer2Reached()
    {
        if (gameOver) return;
        gameOver = true;
        Debug.Log("Player2 Win!");
        if (player2WinCanvas != null) player2WinCanvas.SetActive(true);
    }

    public void OnClickRestart()
    {
        SceneManager.LoadScene("MazeMakingScene");
    }
}