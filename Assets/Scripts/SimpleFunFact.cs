using UnityEngine;
using TMPro;

[System.Serializable]
public class CountryFunFact
{
    public string countryName;
    public string funFact;
}

public class SimpleFunFact : MonoBehaviour
{
    public GameObject funFactPanel;
    public TextMeshProUGUI funFactText;
    public CountryFunFact[] countries;

    private string currentCountry = "";

    void Start()
    {
        if (funFactPanel != null)
            funFactPanel.SetActive(false);
    }

    public void OnImageDetected(string imageName)
    {
        if (currentCountry == imageName) return;

        foreach (var country in countries)
        {
            if (country.countryName == imageName)
            {
                ShowFunFact(country.funFact, imageName);

                CountryDataManager.Instance.UnlockCountry(imageName);
                break;
            }
        }
    }

    void ShowFunFact(string fact, string countryName)
    {
        currentCountry = countryName;

        if (funFactText != null)
            funFactText.text = fact;
        if (funFactPanel != null)
            funFactPanel.SetActive(true);

        SoundManager soundManager = FindObjectOfType<SoundManager>();
        if (soundManager != null)
        {
            soundManager.ShowSoundButton(countryName);
        }

        CountryInfoManager infoManager = FindObjectOfType<CountryInfoManager>();
        if (infoManager != null)
        {
            infoManager.ShowCountryInfo(countryName);
        }
    }

    public void HideFunFact()
    {
        currentCountry = "";

        if (funFactPanel != null)
            funFactPanel.SetActive(false);

        SoundManager soundManager = FindObjectOfType<SoundManager>();
        if (soundManager != null)
        {
            soundManager.HideSoundButton();
        }

        CountryInfoManager infoManager = FindObjectOfType<CountryInfoManager>();
        if (infoManager != null)
        {
            infoManager.HideCountryInfo();
        }
    }
}