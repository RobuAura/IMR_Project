using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using System;

public class DailyChallengeManager : MonoBehaviour
{
    [Header("Panel")]
    public GameObject dailyChallengePanel;

    [Header("Challenge Texts")]
    public TextMeshProUGUI challenge1Text;
    public TextMeshProUGUI challenge2Text;
    public TextMeshProUGUI challenge3Text;

    [Header("Progress Bars")]
    public Image challenge1Bar;
    public Image challenge2Bar;
    public Image challenge3Bar;

    [Header("Buttons")]
    public Button dailyChallengeButton;
    public Button startPlayingButton;

    // Tintele provocarilor
    private const int TARGET1 = 5; // jocuri Easy
    private const int TARGET2 = 3; // castigate Medium vs AI
    private const int TARGET3 = 1; // castigate Hard vs AI

    // Progresul curent
    private int progress1 = 0;
    private int progress2 = 0;
    private int progress3 = 0;

    private bool rewardGiven = false;

    public Button backButton;

    void Start()
    {
        dailyChallengePanel.SetActive(false);
        CheckDailyReset();
        LoadProgress();

        if (PlayerPrefs.GetInt("LastGame_Completed", 0) == 1)
        {
            int diff = PlayerPrefs.GetInt("LastGame_Difficulty", 0);
            bool wonVsAI = PlayerPrefs.GetInt("LastGame_WonVsAI", 0) == 1;

            ReportGameResult(diff, wonVsAI);

            PlayerPrefs.SetInt("LastGame_Completed", 0);
            PlayerPrefs.Save();
        }

        UpdateUI();
    }

    void CheckDailyReset()
    {
        string lastReset = PlayerPrefs.GetString("DailyChallenge_LastReset", "");
        string today = DateTime.Now.ToString("yyyy-MM-dd");
        if (lastReset != today)
        {
            PlayerPrefs.SetInt("DailyChallenge_Progress1", 0);
            PlayerPrefs.SetInt("DailyChallenge_Progress2", 0);
            PlayerPrefs.SetInt("DailyChallenge_Progress3", 0);
            PlayerPrefs.SetInt("DailyChallenge_RewardGiven", 0);
            PlayerPrefs.SetInt("DailyChallenge_Notified1", 0);
            PlayerPrefs.SetInt("DailyChallenge_Notified2", 0);
            PlayerPrefs.SetInt("DailyChallenge_Notified3", 0);
            PlayerPrefs.SetInt("DailyChallenge_NotifiedAll", 0);
            PlayerPrefs.SetString("DailyChallenge_LastReset", today);
            PlayerPrefs.Save();
        }
    }

    void LoadProgress()
    {
        progress1 = PlayerPrefs.GetInt("DailyChallenge_Progress1", 0);
        progress2 = PlayerPrefs.GetInt("DailyChallenge_Progress2", 0);
        progress3 = PlayerPrefs.GetInt("DailyChallenge_Progress3", 0);
        rewardGiven = PlayerPrefs.GetInt("DailyChallenge_RewardGiven", 0) == 1;
    }

    void UpdateUI()
    {
        // Texte
        challenge1Text.text = "Play 5 Easy games: " + Mathf.Min(progress1, TARGET1) + "/" + TARGET1;
        challenge2Text.text = "Win 3 Medium games vs. Computer: " + Mathf.Min(progress2, TARGET2) + "/" + TARGET2;
        challenge3Text.text = "Win 1 Hard game vs. Computer: " + Mathf.Min(progress3, TARGET3) + "/" + TARGET3;

        // Bare de progres
        challenge1Bar.fillAmount = (float)progress1 / TARGET1;
        challenge2Bar.fillAmount = (float)progress2 / TARGET2;
        challenge3Bar.fillAmount = (float)progress3 / TARGET3;

        // Culoare verde daca e completata
        challenge1Bar.color = progress1 >= TARGET1 ? new Color(1f, 0.5f, 0f) : Color.yellow;
        challenge2Bar.color = progress2 >= TARGET2 ? new Color(1f, 0.5f, 0f) : Color.yellow;
        challenge3Bar.color = progress3 >= TARGET3 ? new Color(1f, 0.5f, 0f) : Color.yellow;
    }

    public void OpenPanel()
    {
        backButton.interactable = false;
        dailyChallengePanel.SetActive(true);
        dailyChallengePanel.transform.localScale = Vector3.one * 0.8f;
        dailyChallengePanel.transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack);
    }

    public void ClosePanel()
    {
        backButton.interactable = true;
        dailyChallengePanel.transform.DOScale(Vector3.one * 0.8f, 0.15f)
            .SetEase(Ease.InBack)
            .OnComplete(() => dailyChallengePanel.SetActive(false));
    }

    // Apelat din MemoryGameManager la sfarsitul jocului
    public void ReportGameResult(int difficulty, bool wonVsAI)
    {
        bool updated = false;

        // Provocarea 1: joaca 5 jocuri Easy
        if (difficulty == 0 && progress1 < TARGET1)
        {
            progress1++;
            PlayerPrefs.SetInt("DailyChallenge_Progress1", progress1);
            updated = true;
        }

        // Provocarea 2: castiga 3 Medium vs AI
        if (difficulty == 1 && wonVsAI && progress2 < TARGET2)
        {
            progress2++;
            PlayerPrefs.SetInt("DailyChallenge_Progress2", progress2);
            updated = true;
        }

        // Provocarea 3: castiga 1 Hard vs AI
        if (difficulty == 2 && wonVsAI && progress3 < TARGET3)
        {
            progress3++;
            PlayerPrefs.SetInt("DailyChallenge_Progress3", progress3);
            updated = true;
        }

        if (updated)
        {
            PlayerPrefs.Save();
            CheckAllCompleted();
        }
    }

    void CheckAllCompleted()
    {
        if (progress1 >= TARGET1 && progress2 >= TARGET2 && progress3 >= TARGET3 && !rewardGiven)
        {
            rewardGiven = true;
            PlayerPrefs.SetInt("DailyChallenge_RewardGiven", 1);

            // Adauga 2 hinturi
            int hints = PlayerPrefs.GetInt("HintsRemaining", 0);
            hints += 2;
            PlayerPrefs.SetInt("HintsRemaining", hints);
            PlayerPrefs.Save();

            Debug.Log("Toate provocarile zilnice completate! +2 Hinturi!");
        }
    }
}