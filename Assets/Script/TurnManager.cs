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

    // 실제 선택된 AI 코루틴만 실행하기 위한 참조
    private MonoBehaviour selectedAI;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        ActivateSelectedAI();
        StartPlayerTurn();
    }

    void ActivateSelectedAI()
    {
        // 일단 전부 비활성화
        ai_BMK.gameObject.SetActive(false);
        ai_MYJ.gameObject.SetActive(false);
        ai_AStar.gameObject.SetActive(false);

        // 선택된 AI만 활성화
        switch (mazeData.selectedAI)
        {
            case 0:
                ai_BMK.gameObject.SetActive(true);
                selectedAI = ai_BMK;
                Debug.Log("AI_BMK 활성화");
                break;
            case 1:
                ai_MYJ.gameObject.SetActive(true);
                selectedAI = ai_MYJ;
                Debug.Log("AI_MYJ 활성화");
                break;
            case 2:
                ai_AStar.gameObject.SetActive(true);
                selectedAI = ai_AStar;
                Debug.Log("AStarAI 활성화");
                break;
        }
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

        // 선택된 AI만 TakeTurn 실행
        if (mazeData.selectedAI == 0)
            yield return StartCoroutine(ai_BMK.TakeTurn());
        else if (mazeData.selectedAI == 1)
            yield return StartCoroutine(ai_MYJ.TakeTurn());
        else if (mazeData.selectedAI == 2)
            yield return StartCoroutine(ai_AStar.TakeTurn());

        StartPlayerTurn();
    }

    public bool CanPlayerAct()
    {
        return currentTurn == TurnState.PlayerTurn && !playerActionUsed;
    }
}