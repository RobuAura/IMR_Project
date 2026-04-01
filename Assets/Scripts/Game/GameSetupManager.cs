using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;
using TMPro;

public class GameSetupManager : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_Dropdown difficultyDropdown;   // Easy / Medium / Hard
    public GameObject vsComputerPanel;        // Panel containing the toggle (hidden on Easy)
    public Toggle vsComputerToggle;           // "Play vs Computer" checkbox
    public Button startButton;
    public Button backButton;

    [Header("Animation Settings")]
    public float clickScale = 1.08f;
    public float clickDuration = 0.12f;

    [Header("Best Time")]
    public TextMeshProUGUI bestTimeText;

    // Difficulty indices matching the dropdown order
    private const int EASY = 0;
    private const int MEDIUM = 1;
    private const int HARD = 2;

    private CanvasGroup vsCanvasGroup;

    void Start()
    {
        vsCanvasGroup = vsComputerPanel.GetComponent<CanvasGroup>();
        if (vsCanvasGroup == null)
            vsCanvasGroup = vsComputerPanel.AddComponent<CanvasGroup>();

        vsComputerToggle.isOn = false;

        if (difficultyDropdown.options.Count == 0)
        {
            difficultyDropdown.options.Add(new TMP_Dropdown.OptionData("Easy"));
            difficultyDropdown.options.Add(new TMP_Dropdown.OptionData("Medium"));
            difficultyDropdown.options.Add(new TMP_Dropdown.OptionData("Hard"));
            difficultyDropdown.RefreshShownValue();
        }

        difficultyDropdown.onValueChanged.AddListener(OnDifficultyChanged);

        // Start hidden, alpha la 1 ca sa nu blocheze input
        vsComputerPanel.SetActive(false);
        vsCanvasGroup.alpha = 1f;
        vsCanvasGroup.interactable = true;
        vsCanvasGroup.blocksRaycasts = true;

        UpdateBestTimeDisplay(difficultyDropdown.value);
    }

    void OnDifficultyChanged(int index)
    {
        bool showVsOption = (index == MEDIUM || index == HARD);

        if (showVsOption)
        {
            vsComputerPanel.SetActive(true);
            // Fade in nicely
            CanvasGroup cg = vsComputerPanel.GetComponent<CanvasGroup>();
            if (cg != null)
            {
                cg.alpha = 0f;
                cg.DOFade(1f, 0.25f);
            }
        }
        else
        {
            // On Easy: hide panel and reset toggle
            vsComputerToggle.isOn = false;
            vsComputerPanel.SetActive(false);
        }

        Debug.Log($"Difficulty changed to: {difficultyDropdown.options[index].text}");

        UpdateBestTimeDisplay(index);
    }


    public void OnStartButtonClicked()
    {
        AnimateButton(startButton);

        // Save settings so GameScene can read them
        int difficulty = difficultyDropdown.value;          // 0=Easy, 1=Medium, 2=Hard
        bool vsComputer = vsComputerToggle.isOn && vsComputerPanel.activeSelf;

        PlayerPrefs.SetInt("MemoryGame_Difficulty", difficulty);
        PlayerPrefs.SetInt("MemoryGame_VsComputer", vsComputer ? 1 : 0);
        PlayerPrefs.Save();

        Debug.Log($"Starting game — Difficulty: {difficulty}, VsComputer: {vsComputer}");

        float totalDelay = (clickDuration * 2) + 0.03f;
        DOVirtual.DelayedCall(totalDelay, () => {
            SceneManager.LoadScene("MemoryGameScene");
        });
    }

    public void OnBackButtonClicked()
    {
        AnimateButton(backButton);

        float totalDelay = (clickDuration * 2) + 0.03f;
        DOVirtual.DelayedCall(totalDelay, () => {
            SceneManager.LoadScene("MainMenu");
        });
    }

    void AnimateButton(Button button)
    {
        if (button == null) return;
        Vector3 originalScale = button.transform.localScale;
        button.transform.DOKill();

        Sequence seq = DOTween.Sequence();
        seq.Append(button.transform.DOScale(originalScale * clickScale, clickDuration).SetEase(Ease.OutQuad));
        seq.Append(button.transform.DOScale(originalScale, clickDuration).SetEase(Ease.InQuad));
    }

    void UpdateBestTimeDisplay(int difficulty)
    {
        int bestTime = PlayerPrefs.GetInt("BestTime_" + difficulty, -1);
        if (bestTime == -1)
            bestTimeText.text = "Best Time: --:--";
        else
        {
            int minutes = bestTime / 60;
            int seconds = bestTime % 60;
            bestTimeText.text = string.Format("Best Time {0:00}:{1:00}", minutes, seconds);
        }
    }
}