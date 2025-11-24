using UnityEngine;
using TMPro;

public class CountryInfoManager : MonoBehaviour
{
    public GameObject infoPanel;
    public TextMeshProUGUI countryNameText;
    public TextMeshProUGUI capitalText;
    public CountryInfo[] countries;

    void Start()
    {
        if (infoPanel != null)
            infoPanel.SetActive(false);
    }

    public void ShowCountryInfo(string countryName)
    {
        foreach (var country in countries)
        {
            if (country.countryName == countryName)
            {
                if (countryNameText != null)
                    countryNameText.text = country.displayName;

                if (capitalText != null)
                    capitalText.text = "Capital: " + country.capital;

                if (infoPanel != null)
                    infoPanel.SetActive(true);

                Debug.Log("Info afisat pentru: " + countryName);
                break;
            }
        }
    }

    public void HideCountryInfo()
    {
        if (infoPanel != null)
            infoPanel.SetActive(false);
    }
}