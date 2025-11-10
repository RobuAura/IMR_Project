using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public void OnScanFlagButtonClicked()
    {
        Debug.Log("Scan Flag button clicked!");
        SceneManager.LoadScene("ARScanScene");
    }

    public void OnQuizButtonClicked()
    {
        Debug.Log("Quiz button clicked!");
        SceneManager.LoadScene("QuizScene");
    }
}