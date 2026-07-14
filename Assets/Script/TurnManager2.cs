using UnityEngine;
using System.Collections;

public class TurnManagerBattle : MonoBehaviour
{
    public static TurnManagerBattle Instance;

    public enum TurnState { Player1Turn, Player2Turn }
    public TurnState currentTurn;

    public MazeData mazeData;

    private bool player1ActionUsed;
    private bool player2ActionUsed;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        StartPlayer1Turn();
    }

    public void StartPlayer1Turn()
    {
        currentTurn = TurnState.Player1Turn;
        player1ActionUsed = false;
        Debug.Log("Player1 ео");
    }

    public void StartPlayer2Turn()
    {
        currentTurn = TurnState.Player2Turn;
        player2ActionUsed = false;
        Debug.Log("Player2 ео");
    }

    public void UsePlayerAction()
    {
        if (player1ActionUsed) return;
        player1ActionUsed = true;
        StartPlayer2Turn();
    }

    public void UsePlayer2Action()
    {
        if (player2ActionUsed) return;
        player2ActionUsed = true;
        StartPlayer1Turn();
    }

    public bool CanPlayerAct()
    {
        return currentTurn == TurnState.Player1Turn && !player1ActionUsed;
    }

    public bool CanPlayer2Act()
    {
        return currentTurn == TurnState.Player2Turn && !player2ActionUsed;
    }
}