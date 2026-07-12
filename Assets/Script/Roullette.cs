using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class RouletteManager : MonoBehaviour
{
    public MazeData mazeData;

    [Header("슬롯 UI")]
    public RectTransform slotContainer;
    public RectTransform cardPrefab;
    public float cardWidth = 300f;
    public float spinDuration = 2.5f;
    public float autoMoveDelay = 2f;

    [Header("반복 설정")]
    public int repeatCount = 5;

    [Header("결과 텍스트")]
    public TextMeshProUGUI resultText;

    [Header("카드 이미지")]
    public Sprite[] aiSprites; // 인스펙터에서 SLIME, BOX MONSTER, BAT 순서로 드래그

    private string[] aiNames = { "SLIME", "BOX MONSTER", "BAT" };
    private List<RectTransform> spawnedCards = new List<RectTransform>();

    void Start()
    {
        StartCoroutine(SpinRoulette());
    }

    void BuildCards()
    {
        foreach (var card in spawnedCards)
            Destroy(card.gameObject);
        spawnedCards.Clear();

        int totalCards = aiNames.Length * repeatCount;

        for (int r = 0; r < repeatCount; r++)
        {
            for (int i = 0; i < aiNames.Length; i++)
            {
                RectTransform card = Instantiate(cardPrefab, slotContainer);

                int index = r * aiNames.Length + i;
                float xPos = (index - (totalCards / 2f) + 0.5f) * cardWidth;
                card.anchoredPosition = new Vector2(xPos, 0);

                // 텍스트 숨기기
                TextMeshProUGUI text = card.GetComponentInChildren<TextMeshProUGUI>();
                if (text != null)
                    text.gameObject.SetActive(false);

                // 이미지 교체
                Image img = card.GetComponentInChildren<Image>();
                if (img != null && aiSprites != null && i < aiSprites.Length)
                    img.sprite = aiSprites[i];

                spawnedCards.Add(card);
            }
        }

        slotContainer.sizeDelta = new Vector2(
            cardWidth * totalCards,
            slotContainer.sizeDelta.y
        );
    }

    IEnumerator SpinRoulette()
    {
        resultText.text = "";
        BuildCards();

        int selected = mazeData.selectedAI;
        int lastSetStart = (repeatCount - 1) * aiNames.Length;
        int totalCards = aiNames.Length * repeatCount;

        float targetIndex = lastSetStart + selected;
        float targetX = -((targetIndex - (totalCards / 2f) + 0.5f) * cardWidth);

        float startIndex = 0;
        float startX = -((startIndex - (totalCards / 2f) + 0.5f) * cardWidth);

        slotContainer.anchoredPosition = new Vector2(startX, 0);
        yield return null;

        float elapsed = 0f;
        while (elapsed < spinDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / spinDuration;
            float eased = 1f - Mathf.Pow(1f - t, 3f);
            float currentX = Mathf.Lerp(startX, targetX, eased);
            slotContainer.anchoredPosition = new Vector2(currentX, 0);
            yield return null;
        }

        slotContainer.anchoredPosition = new Vector2(targetX, 0);
        resultText.text = $"Selected: {aiNames[selected]}";

        yield return new WaitForSeconds(autoMoveDelay);
        SceneManager.LoadScene("AIEscapeScene");
    }
}