using UnityEngine;
using TMPro;

public class SimpleFunFact : MonoBehaviour
{
    public GameObject funFactPanel;
    public TextMeshProUGUI funFactText;

    public string targetCountry = "Egypt";

    public string funFact = " The Pyramid of Khufu at Giza is the largest Egyptian pyramid. This incredible structure weighs as much as 16 Empire State buildings!";

    private bool hasUnlocked = false;

    void Start()
    {
        
        if (funFactPanel != null)
            funFactPanel.SetActive(false);
    }

    // Metoda apelata cand o imagine este detectata
    public void OnImageDetected(string imageName)
    {
       
        if (imageName == targetCountry)
        {
            ShowFunFact();

            if (!hasUnlocked)
            {
                CountryDataManager.Instance.UnlockCountry(targetCountry);
                hasUnlocked = true;
            }
        }
    }

    void ShowFunFact()
    {
        if (funFactText != null)
            funFactText.text = funFact;

        if (funFactPanel != null)
            funFactPanel.SetActive(true);

        Debug.Log("Fun fact afisat pentru: " + targetCountry);
    }

   
    public void HideFunFact()
    {
        if (funFactPanel != null)
            funFactPanel.SetActive(false);
    }
}