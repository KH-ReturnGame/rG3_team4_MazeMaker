using UnityEngine;
using System.Collections;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance;

    public enum TurnState
    {
        PlayerTurn,
        AITurn
    }

    public TurnState currentTurn;

    public AI_BMK ai;

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
        if (playerActionUsed)
            return;

        playerActionUsed = true;

        StartCoroutine(StartAITurn());
    }

    IEnumerator StartAITurn()
    {
        currentTurn = TurnState.AITurn;

        Debug.Log("AI 턴");

        yield return ai.TakeTurn();

        StartPlayerTurn();
    }

    public bool CanPlayerAct()
    {
        return currentTurn ==
               TurnState.PlayerTurn &&
               !playerActionUsed;
    }
}