using UnityEngine;
using System.Collections;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance;

    public enum TurnState { PlayerTurn, AITurn }
    public TurnState currentTurn;

    public AI_BMK ai_BMK;
    public AI_MYJ ai_MYJ;
    public AStarAI ai_AStar;
    public MazeData mazeData;

    private bool playerActionUsed;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        StartPlayerTurn();
    }

    public void StartPlayerTurn()
    {
        currentTurn = TurnState.PlayerTurn;
        playerActionUsed = false;
        Debug.Log("플레이어 턴");
    }

    public void UsePlayerAction()
    {
        if (playerActionUsed) return;
        playerActionUsed = true;
        StartCoroutine(StartAITurn());
    }

    IEnumerator StartAITurn()
    {
        currentTurn = TurnState.AITurn;
        Debug.Log("AI 턴");

        // 세 AI 동시에 턴 진행
        Coroutine c1 = StartCoroutine(ai_BMK.TakeTurn());
        Coroutine c2 = StartCoroutine(ai_MYJ.TakeTurn());
        Coroutine c3 = StartCoroutine(ai_AStar.TakeTurn());

        yield return c1;
        yield return c2;
        yield return c3;

        StartPlayerTurn();
    }

    public bool CanPlayerAct()
    {
        return currentTurn == TurnState.PlayerTurn && !playerActionUsed;
    }
}