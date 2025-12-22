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

    [Header("Final Score UI")]
    public GameObject finalScorePanel;
    public TextMeshProUGUI finalScoreText;
    public TextMeshProUGUI perfectScoresText;
    public Button backToMenuButton;
    public Button retryButton;

    [Header("Quiz Data")]
    public Question[] allQuestions;
    public int questionsPerQuiz = 10;

    private Question[] currentQuestions;
    private int currentQuestionIndex = 0;
    private int score = 0;
    private bool hasAnswered = false;

    void Start()
    {
        currentQuestions = GetRandomQuestions(questionsPerQuiz);
        nextButton.interactable = false;
        finalScorePanel.SetActive(false);
        LoadQuestion();
        UpdateScoreUI();
    }

    Question[] GetRandomQuestions(int count)
    {
        if (count > allQuestions.Length)
        {
            count = allQuestions.Length;
        }

        Question[] shuffled = new Question[allQuestions.Length];
        System.Array.Copy(allQuestions, shuffled, allQuestions.Length);

        for (int i = 0; i < shuffled.Length; i++)
        {
            int randomIndex = Random.Range(i, shuffled.Length);
            Question temp = shuffled[i];
            shuffled[i] = shuffled[randomIndex];
            shuffled[randomIndex] = temp;
        }

        Question[] selected = new Question[count];
        System.Array.Copy(shuffled, selected, count);

        return selected;
    }

    void LoadQuestion()
    {
        if (currentQuestionIndex >= currentQuestions.Length)
        {
            ShowFinalScore();
            return;
        }

        hasAnswered = false;
        nextButton.interactable = false;

        Question currentQuestion = currentQuestions[currentQuestionIndex];

        if (currentQuestion.flagImage != null)
        {
            flagImage.gameObject.SetActive(true);
            flagImage.sprite = currentQuestion.flagImage;
        }
        else
        {
            flagImage.gameObject.SetActive(false);
        }


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
        Question currentQuestion = currentQuestions[currentQuestionIndex];

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
        scoreText.text = "Score: " + score + "/" + currentQuestions.Length;
    }

    void ShowFinalScore()
    {
        flagImage.gameObject.SetActive(false);
        questionText.gameObject.SetActive(false);
        nextButton.gameObject.SetActive(false);
        scoreText.gameObject.SetActive(false);

        foreach (Button btn in answerButtons)
        {
            btn.gameObject.SetActive(false);
        }

        finalScorePanel.SetActive(true);

        int perfectScores = PlayerPrefs.GetInt("PerfectScores", 0);

        if (score == currentQuestions.Length)
        {
            perfectScores++;
            PlayerPrefs.SetInt("PerfectScores", perfectScores);
        }

        PlayerPrefs.Save();
        finalScoreText.text = "Quiz Completed!\n\nYour Score: " + score + "/" + currentQuestions.Length;

        if (perfectScores == 0)
        {
            perfectScoresText.text = "No perfect scores yet. Keep trying!";
        }
        else if (perfectScores == 1)
        {
            perfectScoresText.text = "First perfect score achieved!";
        }
        else
        {
            perfectScoresText.text = "Perfect scores: " + perfectScores + " times!";
        }
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void RetryQuiz()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}