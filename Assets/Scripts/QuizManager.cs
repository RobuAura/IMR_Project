using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class QuizManager : MonoBehaviour
{
    [Header("UI References")]
    public Image flagImage;
    public TextMeshProUGUI questionText;
    public Button[] answerButtons;
    public Button nextButton;
    public TextMeshProUGUI scoreText;
    public GameObject funFactPanel;
    public TextMeshProUGUI funFactText;

    [Header("Final Score UI")]
    public GameObject finalScorePanel;
    public TextMeshProUGUI finalScoreText;
    public Button backToMenuButton;

    [Header("Quiz Data")]
    public Question[] questions;

    private int currentQuestionIndex = 0;
    private int score = 0;
    private bool hasAnswered = false;

    void Start()
    {
        nextButton.interactable = false;
        finalScorePanel.SetActive(false);
        funFactPanel.SetActive(false);
        LoadQuestion();
        UpdateScoreUI();
    }

    void LoadQuestion()
    {
        if (currentQuestionIndex >= questions.Length)
        {
            ShowFinalScore();
            return;
        }

        hasAnswered = false;
        nextButton.interactable = false;
        funFactPanel.SetActive(false);

        Question currentQuestion = questions[currentQuestionIndex];

        flagImage.sprite = currentQuestion.flagImage;
        questionText.text = currentQuestion.questionText;

        for (int i = 0; i < answerButtons.Length; i++)
        {
            TextMeshProUGUI buttonText = answerButtons[i].GetComponentInChildren<TextMeshProUGUI>();
            buttonText.text = currentQuestion.answers[i];

            answerButtons[i].GetComponent<Image>().color = new Color(0.29f, 0.56f, 0.89f);
            answerButtons[i].interactable = true;
        }
    }

    public void OnAnswerSelected(int answerIndex)
    {
        if (hasAnswered) return;

        hasAnswered = true;
        Question currentQuestion = questions[currentQuestionIndex];

        if (answerIndex == currentQuestion.correctAnswerIndex)
        {
            answerButtons[answerIndex].GetComponent<Image>().color = Color.green;
            score++;
        }
        else
        {
            answerButtons[answerIndex].GetComponent<Image>().color = Color.red;
            answerButtons[currentQuestion.correctAnswerIndex].GetComponent<Image>().color = Color.green;
        }

        foreach (Button btn in answerButtons)
        {
            btn.interactable = false;
        }

        funFactPanel.SetActive(true);
        funFactText.text = currentQuestion.funFact;
        nextButton.interactable = true;
        UpdateScoreUI();
    }

    public void OnNextButtonClicked()
    {
        currentQuestionIndex++;
        LoadQuestion();
    }

    void UpdateScoreUI()
    {
        scoreText.text = "Score: " + score + "/" + questions.Length;
    }

    void ShowFinalScore()
    {
        // Ascunde tot UI-ul de quiz
        flagImage.gameObject.SetActive(false);
        questionText.gameObject.SetActive(false);
        nextButton.gameObject.SetActive(false);
        scoreText.gameObject.SetActive(false);
        funFactPanel.SetActive(false);

        foreach (Button btn in answerButtons)
        {
            btn.gameObject.SetActive(false);
        }

        // Aratã panoul cu scorul final
        finalScorePanel.SetActive(true);
        finalScoreText.text = "Quiz Completed!\n\nYour Score:\n" + score + "/" + questions.Length;
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}