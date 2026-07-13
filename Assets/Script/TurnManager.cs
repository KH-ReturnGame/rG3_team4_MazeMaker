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
    private Transform selectedAITransform;

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
        ai_BMK.gameObject.SetActive(false);
        ai_MYJ.gameObject.SetActive(false);
        ai_AStar.gameObject.SetActive(false);

        switch (mazeData.selectedAI)
        {
            case 0:
                ai_BMK.gameObject.SetActive(true);
                selectedAITransform = ai_BMK.transform;
                Debug.Log("AI_BMK 활성화");
                break;
            case 1:
                ai_MYJ.gameObject.SetActive(true);
                selectedAITransform = ai_MYJ.transform;
                Debug.Log("AI_MYJ 활성화");
                break;
            case 2:
                ai_AStar.gameObject.SetActive(true);
                selectedAITransform = ai_AStar.transform;
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

        // 카메라 AI에 포커스
        yield return CameraController.Instance.FocusOnAI(selectedAITransform);

        // 선택된 AI 턴 실행
        if (mazeData.selectedAI == 0)
            yield return StartCoroutine(ai_BMK.TakeTurn());
        else if (mazeData.selectedAI == 1)
            yield return StartCoroutine(ai_MYJ.TakeTurn());
        else if (mazeData.selectedAI == 2)
            yield return StartCoroutine(ai_AStar.TakeTurn());

        // 카메라 원래대로 복귀
        yield return CameraController.Instance.ReturnToDefault();

        StartPlayerTurn();
    }

    public bool CanPlayerAct()
    {
        return currentTurn == TurnState.PlayerTurn && !playerActionUsed;
    }
}