using UnityEngine;
using UnityEngine.SceneManagement;

public class ARSceneManager : MonoBehaviour
{
    public void BackToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void OpenCollection()
    {
        Debug.Log("Collection button clicked!");
        SceneManager.LoadScene("CollectionScene");
    }
}