using UnityEngine;
using UnityEngine.SceneManagement;

public class ARSceneManager : MonoBehaviour
{
    [Header("Instructions Panel")]
    public GameObject instructionsPanel;
    public GameObject instructionsButton;

    void Start()
    {
        if (instructionsPanel != null)
        {
            instructionsPanel.SetActive(false);
        }

        if (instructionsButton != null)
        {
            instructionsButton.SetActive(true);
        }
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void OpenCollection()
    {
        Debug.Log("Collection button clicked!");
        SceneManager.LoadScene("CollectionScene");
    }

    public void OpenInstructions()
    {
        if (instructionsPanel != null)
        {
            instructionsPanel.SetActive(true);
            Debug.Log("Instructions panel opened!");
        }

        if (instructionsButton != null)
        {
            instructionsButton.SetActive(false);
        }
    }

    public void CloseInstructions()
    {
        if (instructionsPanel != null)
        {
            instructionsPanel.SetActive(false);
            Debug.Log("Instructions panel closed!");
        }

        if (instructionsButton != null)
        {
            instructionsButton.SetActive(true);
        }
    }
}