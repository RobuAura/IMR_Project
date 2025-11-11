using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class CollectionManager : MonoBehaviour
{
    [System.Serializable]
    public class CountryCard
    {
        public string countryName;
        public GameObject cardPanel;
        public Image flagImage;
        public TextMeshProUGUI statusText;
        public GameObject lockedOverlay;
    }

    public CountryCard[] countries;

    void Start()
    {
        UpdateAllCards();
    }

    void UpdateAllCards()
    {
        foreach (CountryCard country in countries)
        {
            bool isUnlocked = CountryDataManager.Instance.IsCountryUnlocked(country.countryName);

            if (isUnlocked)
            {
                // Tara descoperita
                country.statusText.text = "Discovered";
                country.statusText.color = Color.green;

                if (country.lockedOverlay != null)
                    country.lockedOverlay.SetActive(false);

                country.flagImage.color = Color.white; 
            }
            else
            {
                // Tara blocata
                country.statusText.text = "Locked";
                country.statusText.color = Color.grey;

                if (country.lockedOverlay != null)
                    country.lockedOverlay.SetActive(true);

                country.flagImage.color = new Color(0.3f, 0.3f, 0.3f); 
            }
        }
    }

    public void BackToARScan()
    {
        SceneManager.LoadScene("ARScanScene");
    }
}