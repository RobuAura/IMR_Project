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

    [Header("Trophy Card")]
    public GameObject trophyCardPanel;
    public TextMeshProUGUI trophyStatusText;
    public GameObject trophyLockedOverlay;

    void Start()
    {
        UpdateAllCards();
        UpdateTrophy();
    }

    void UpdateAllCards()
    {
        foreach (CountryCard country in countries)
        {
            bool isUnlocked = CountryDataManager.Instance.IsCountryUnlocked(country.countryName);

            if (isUnlocked)
            {
                country.statusText.text = "Discovered";
                country.statusText.color = Color.green;

                if (country.lockedOverlay != null)
                    country.lockedOverlay.SetActive(false);

                country.flagImage.color = Color.white;
            }
            else
            {
                country.statusText.text = "Locked";
                country.statusText.color = Color.grey;

                if (country.lockedOverlay != null)
                    country.lockedOverlay.SetActive(true);

                country.flagImage.color = new Color(0.3f, 0.3f, 0.3f);
            }
        }
    }

    void UpdateTrophy()
    {
        bool allUnlocked = AreAllCountriesUnlocked();

        if (allUnlocked)
        {
            if (trophyStatusText != null)
            {
                trophyStatusText.text = "Master Explorer!";
                trophyStatusText.color = new Color(1f, 0.84f, 0f);
            }

            if (trophyLockedOverlay != null)
                trophyLockedOverlay.SetActive(false);

            CountryDataManager.Instance.UnlockCountry("Trophy");

            Debug.Log("TROFEU DEBLOCAT!");
        }
        else
        {
            if (trophyStatusText != null)
            {
                int unlocked = CountUnlockedCountries();
                trophyStatusText.text = unlocked + "/" + countries.Length + " countries";
                trophyStatusText.color = Color.grey;
            }

            if (trophyLockedOverlay != null)
                trophyLockedOverlay.SetActive(true);
        }
    }

    bool AreAllCountriesUnlocked()
    {
        foreach (CountryCard country in countries)
        {
            if (!CountryDataManager.Instance.IsCountryUnlocked(country.countryName))
            {
                return false;
            }
        }
        return true;
    }

    int CountUnlockedCountries()
    {
        int count = 0;
        foreach (CountryCard country in countries)
        {
            if (CountryDataManager.Instance.IsCountryUnlocked(country.countryName))
            {
                count++;
            }
        }
        return count;
    }

    public void UnlockAll()
    {
        foreach (CountryCard country in countries)
        {
            CountryDataManager.Instance.UnlockCountry(country.countryName);
        }
        UpdateAllCards();
        UpdateTrophy();
    }

    public void ResetProgress()
    {
        CountryDataManager.Instance.ResetAllProgress();
        UpdateAllCards();
        UpdateTrophy();
    }

    public void BackToARScan()
    {
        SceneManager.LoadScene("ARScanScene");
    }
}