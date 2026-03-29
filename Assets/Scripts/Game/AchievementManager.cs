using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using System;
using System.Collections.Generic;

public class AchievementManager : MonoBehaviour
{
    [Header("Panel")]
    public GameObject achievementsPanel;
    public Button achievementsButton;
    public Button continueButton;
    public Button backButton;

    [Header("Scroll Content")]
    public Transform scrollContent;
    public GameObject progressItemPrefab;
    public GameObject completedItemPrefab;
    public GameObject separatorPrefab; 

    // Pragurile
    private int[] thresholds = { 10, 50, 100, 150, 200 };

    // Categoriile
    private string[] categoryNames = {
        "Play Easy mode games",
        "Play Medium mode games",
        "Play Hard mode games",
        "Win Medium vs Computer game",
        "Win Hard vs Computer game"
    };

    private string[] categoryKeys = {
        "Achievements_EasyPlayed",
        "Achievements_MediumPlayed",
        "Achievements_HardPlayed",
        "Achievements_MediumWon",
        "Achievements_HardWon"
    };

    private int[] hintRewards = { 1, 1, 1, 2, 2 };

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        achievementsPanel.SetActive(false);
    }

    public void OpenPanel()
    {
        if (backButton != null) backButton.interactable = false;
        achievementsPanel.SetActive(true);
        achievementsPanel.transform.localScale = Vector3.one * 0.8f;
        achievementsPanel.transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack);
        PopulateAchievements();
    }

    public void ClosePanel()
    {
        if (backButton != null) backButton.interactable = true;
        achievementsPanel.transform.DOScale(Vector3.one * 0.8f, 0.15f)
            .SetEase(Ease.InBack)
            .OnComplete(() => achievementsPanel.SetActive(false));
    }

    void PopulateAchievements()
    {
        // Sterge itemele vechi
        foreach (Transform child in scrollContent)
            Destroy(child.gameObject);

        // Sectiunea de progres curent
        for (int i = 0; i < categoryKeys.Length; i++)
        {
            string key = categoryKeys[i];
            string name = categoryNames[i];

            int current = PlayerPrefs.GetInt(key, 0);
            int nextThreshold = GetNextThreshold(key, current);

            if (nextThreshold == -1) continue; // toate pragurile atinse

            // Creeaza item de progres
            GameObject item = Instantiate(progressItemPrefab, scrollContent);

            TextMeshProUGUI nameText = item.transform.Find("ProgressNameText").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI countText = item.transform.Find("ProgressCountText").GetComponent<TextMeshProUGUI>();
            Image progressBar = item.transform.Find("ProgressBar").GetComponent<Image>();

            int prevThreshold = GetPrevThreshold(key, nextThreshold);
            int progressInRange = current - prevThreshold;
            int rangeSize = nextThreshold - prevThreshold;

            nameText.text = name;
            countText.text = current + "/" + nextThreshold;
            progressBar.fillAmount = (float)progressInRange / rangeSize;
        }

        // Sectiunea "Completed:"
        List<(string name, int threshold, string date)> completed = new List<(string, int, string)>();

        for (int i = 0; i < categoryKeys.Length; i++)
        {
            string key = categoryKeys[i];
            string name = categoryNames[i];

            foreach (int threshold in thresholds)
            {
                string dateKey = key + "_Date_" + threshold;
                string date = PlayerPrefs.GetString(dateKey, "");

                if (date != "")
                    completed.Add((name, threshold, date));
            }
        }

        if (completed.Count > 0)
        {
            // Separator
            if (separatorPrefab != null)
                Instantiate(separatorPrefab, scrollContent);

            // Adauga realizarile completate
            foreach (var c in completed)
            {
                GameObject item = Instantiate(completedItemPrefab, scrollContent);

                TextMeshProUGUI nameText = item.transform.Find("CompletedNameText").GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI dateText = item.transform.Find("CompletedDateText").GetComponent<TextMeshProUGUI>();

                nameText.text = c.name + ": " + c.threshold;
                dateText.text = c.date;
            }
        }
    }

    int GetNextThreshold(string key, int current)
    {
        foreach (int threshold in thresholds)
        {
            string reachedKey = key + "_Reached_" + threshold;
            if (PlayerPrefs.GetInt(reachedKey, 0) == 0)
                return threshold;
        }
        return -1; // toate atinse
    }

    int GetPrevThreshold(string key, int nextThreshold)
    {
        int prev = 0;
        foreach (int threshold in thresholds)
        {
            if (threshold == nextThreshold) return prev;
            prev = threshold;
        }
        return 0;
    }

    public void ReportGameResult(int difficulty, bool wonVsAI)
    {
        if (difficulty == 0)
            UpdateCounter("Achievements_EasyPlayed", 1);
        else if (difficulty == 1)
        {
            UpdateCounter("Achievements_MediumPlayed", 1);
            if (wonVsAI)
                UpdateCounter("Achievements_MediumWon", 2);
        }
        else if (difficulty == 2)
        {
            UpdateCounter("Achievements_HardPlayed", 1);
            if (wonVsAI)
                UpdateCounter("Achievements_HardWon", 2);
        }

        PlayerPrefs.Save();
    }

    void UpdateCounter(string key, int hintReward)
    {
        int current = PlayerPrefs.GetInt(key, 0) + 1;
        PlayerPrefs.SetInt(key, current);

        foreach (int threshold in thresholds)
        {
            string reachedKey = key + "_Reached_" + threshold;
            if (current >= threshold && PlayerPrefs.GetInt(reachedKey, 0) == 0)
            {
                PlayerPrefs.SetInt(reachedKey, 1);
                PlayerPrefs.SetString(key + "_Date_" + threshold,
                    DateTime.Now.ToString("dd/MM/yyyy"));

                int hints = PlayerPrefs.GetInt("HintsRemaining", 0);
                hints += hintReward;
                PlayerPrefs.SetInt("HintsRemaining", hints);

                Debug.Log("Achievement unlocked: " + key + " - " + threshold + " | +" + hintReward + " hints");
            }
        }
    }
}