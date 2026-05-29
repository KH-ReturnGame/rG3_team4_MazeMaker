using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private bool gameOver = false;

    public PlayerTest player;
    public AI_BMK ai;

    void Awake()
    {
        Instance = this;
    }

    public void OnPlayerReached()
    {
        if (gameOver) return;
        gameOver = true;
        Debug.Log("Player Win!");
        player.enabled = false;
    }

    public void OnAIReached()
    {
        if (gameOver) return;
        gameOver = true;
        Debug.Log("AI Win!");
        player.OnAIReached();
    }
}