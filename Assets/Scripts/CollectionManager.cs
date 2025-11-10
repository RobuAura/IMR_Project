using UnityEngine;
using UnityEngine.SceneManagement;

public class CollectionManager : MonoBehaviour
{
    public void BackToARScan()
    {
        SceneManager.LoadScene("ARScanScene"); 
    }

    void Start()
    {
        Debug.Log("Collection Scene Loaded!");
    }
}
