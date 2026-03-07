using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;
using System.Collections.Generic;
using TMPro;

public class MemoryGameManager : MonoBehaviour
{
    public static MemoryGameManager Instance;

    [Header("References")]
    public GameObject cardPrefab;
    public GridLayoutGroup gridLayoutGroup;
    public Button backButton;

    [Header("Quit Popup")]
    public GameObject quitConfirmPanel;
    public Button yesButton;
    public Button noButton;

    [Header("Settings")]
    public float flipBackDelay = 1.5f; // secunde pana se intorc inapoi

    [Header("Timer")]
    public TextMeshProUGUI timerText;

    [Header("Game Over")]
    public GameObject gameOverPanel;
    public TextMeshProUGUI timeResultText;
    public Button playAgainButton;
    public Button menuButton;

    private float timeElapsed = 0f;
    private bool timerRunning = false;

    private int rows;
    private int cols;
    private int difficulty;
    private bool vsComputer;

    private Card firstCard = null;
    private Card secondCard = null;
    private bool canSelect = true; // blocheaza selectia cat timp se verifica perechea

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        difficulty = PlayerPrefs.GetInt("MemoryGame_Difficulty", 0);
        vsComputer = PlayerPrefs.GetInt("MemoryGame_VsComputer", 0) == 1;

        switch (difficulty)
        {
            case 0: rows = 3; cols = 4; break;
            case 1: rows = 4; cols = 4; break;
            case 2: rows = 5; cols = 6; break;
        }

        SetupGrid();
        GenerateCards();

        timerRunning = true;
    }

    void Update()
    {
        if (!timerRunning) return;

        timeElapsed += Time.deltaTime;

        int minutes = Mathf.FloorToInt(timeElapsed / 60f);
        int seconds = Mathf.FloorToInt(timeElapsed % 60f);

        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    void SetupGrid()
    {
        RectTransform gridRect = gridLayoutGroup.GetComponent<RectTransform>();

        float gridWidth = gridRect.rect.width;
        float gridHeight = gridRect.rect.height;

        float spacingX = 10f;
        float spacingY = 10f;

        float cellWidth = (gridWidth - spacingX * (cols - 1)) / cols;
        float cellHeight = (gridHeight - spacingY * (rows - 1)) / rows;
        float cellSize = Mathf.Min(cellWidth, cellHeight);

        gridLayoutGroup.cellSize = new Vector2(cellSize, cellSize);
        gridLayoutGroup.spacing = new Vector2(spacingX, spacingY);
        gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayoutGroup.constraintCount = cols;
        gridLayoutGroup.childAlignment = TextAnchor.MiddleCenter;
    }

    void GenerateCards()
    {
        int totalCards = rows * cols;
        int totalPairs = totalCards / 2;

        List<int> cardIds = new List<int>();
        for (int i = 1; i <= totalPairs; i++)
        {
            cardIds.Add(i);
            cardIds.Add(i);
        }

        for (int i = cardIds.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            int temp = cardIds[i];
            cardIds[i] = cardIds[randomIndex];
            cardIds[randomIndex] = temp;
        }

        for (int i = 0; i < totalCards; i++)
        {
            GameObject cardObj = Instantiate(cardPrefab, gridLayoutGroup.transform);
            cardObj.name = "Card_" + i;

            Card card = cardObj.GetComponent<Card>();
            card.SetCard(cardIds[i]);
        }

        Debug.Log($"Generated {totalCards} cards ({rows}x{cols}) with {totalPairs} pairs");
    }

    // Apelat din Card.cs cand se apasa o carte
    public void OnCardSelected(Card card)
    {
        if (!canSelect) return;

        card.FlipToFront();

        if (firstCard == null)
        {
            // Prima carte selectata
            firstCard = card;
        }
        else
        {
            // A doua carte selectata
            secondCard = card;
            canSelect = false; // blocheaza selectia
            CheckMatch();
        }
    }

    void CheckMatch()
    {
        if (firstCard.CardId == secondCard.CardId)
        {
            DOVirtual.DelayedCall(0.5f, () =>
            {
                firstCard.SetMatched();
                secondCard.SetMatched();

                firstCard = null;
                secondCard = null;
                canSelect = true;
                if (AreAllMatched())
                {
                    timerRunning = false;
                    ShowGameOver();
                }
            });
        }
        else
        {
            // Nu e pereche - intoarce inapoi dupa delay
            DOVirtual.DelayedCall(flipBackDelay, () =>
            {
                firstCard.FlipToBack();
                secondCard.FlipToBack();

                firstCard = null;
                secondCard = null;
                canSelect = true;
                if (AreAllMatched())
                {
                    timerRunning = false;
                    ShowGameOver();
                }

            });
        }
    }

    bool AreAllMatched()
    {
        foreach (Transform child in gridLayoutGroup.transform)
        {
            Card card = child.GetComponent<Card>();
            if (card != null && !card.IsMatched)
                return false;
        }
        return true;
    }

    public void OnBackButtonClicked()
    {
        AnimateButton(backButton);
        DOVirtual.DelayedCall(0.25f, () =>
        {
            quitConfirmPanel.SetActive(true);

            // Animatie de aparitie
            quitConfirmPanel.transform.localScale = Vector3.one * 0.8f;
            quitConfirmPanel.transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack);
        });
    }

    // Butonul Yes — iese din joc
    public void OnYesButtonClicked()
    {
        AnimateButton(yesButton);
        DOVirtual.DelayedCall(0.3f, () =>
        {
            SceneManager.LoadScene("GameSetupScene");
        });
    }

    // Butonul No — inchide popup-ul
    public void OnNoButtonClicked()
    {
        AnimateButton(noButton);
        quitConfirmPanel.transform.DOScale(Vector3.one * 0.8f, 0.15f)
            .SetEase(Ease.InBack)
            .OnComplete(() => quitConfirmPanel.SetActive(false));
    }

    void ShowGameOver()
    {
        int minutes = Mathf.FloorToInt(timeElapsed / 60f);
        int seconds = Mathf.FloorToInt(timeElapsed % 60f);
        timeResultText.text = string.Format("Time: {0:00}:{1:00}", minutes, seconds);

        DOVirtual.DelayedCall(1.5f, () =>
        {
            gameOverPanel.SetActive(true);
            gameOverPanel.transform.localScale = Vector3.one * 0.8f;
            gameOverPanel.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);
        });
    }

    public void OnPlayAgainButtonClicked()
    {
        SceneManager.LoadScene("MemoryGameScene");
    }

    public void OnMenuButtonClicked()
    {
        SceneManager.LoadScene("GameSetupScene");
    }

    void AnimateButton(Button button)
    {
        if (button == null) return;
        Vector3 originalScale = button.transform.localScale;
        button.transform.DOKill();
        Sequence seq = DOTween.Sequence();
        seq.Append(button.transform.DOScale(originalScale * 1.08f, 0.12f).SetEase(Ease.OutQuad));
        seq.Append(button.transform.DOScale(originalScale, 0.12f).SetEase(Ease.InQuad));
    }
}