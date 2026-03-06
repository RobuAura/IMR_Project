using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

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

        for (int i = 0; i < totalCards; i++)
        {
            GameObject cardObj = Instantiate(cardPrefab, gridLayoutGroup.transform);
            cardObj.name = "Card_" + i;
        }
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
        // Pentru moment toate cartile sunt "diferite" — le intoarcem inapoi dupa delay
        // Cand adaugam ID-uri vom verifica perechea reala
        DOVirtual.DelayedCall(flipBackDelay, () =>
        {
            firstCard.FlipToBack();
            secondCard.FlipToBack();

            firstCard = null;
            secondCard = null;
            canSelect = true;
        });
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