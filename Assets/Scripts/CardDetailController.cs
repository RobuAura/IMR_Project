using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class CountryCardData
{
    public string countryName;
    public Sprite cardSprite;
    public string initialMessage = "Some objects around the flag hide interesting information. Try to discover them!";
    public GameObject hotspotButton1;
    public string hotspot1Description;
    public GameObject hotspotButton2;
    public string hotspot2Description;
}

public class CardDetailController : MonoBehaviour
{
    public GameObject cardDetailPanel;
    public GameObject cardButton;
    public Image cardButtonImage;
    public Image largeCardImage;
    public TextMeshProUGUI descriptionText;
    public CountryCardData[] countries;

    private string currentCountry = "";

    void Start()
    {
        if (cardDetailPanel != null)
        {
            cardDetailPanel.SetActive(false);
        }

        if (cardButton != null)
        {
            cardButton.SetActive(false);
        }
    }

    public void ShowCardButton()
    {
        if (cardButton != null)
        {
            cardButton.SetActive(true);
            Debug.Log("Card button shown!");
        }
    }

    public void HideCardButton()
    {
        if (cardButton != null)
        {
            cardButton.SetActive(false);
            Debug.Log("Card button hidden!");
        }

        currentCountry = "";
    }

    public void ShowCardButton(string countryName)
    {
        currentCountry = countryName;
        SetCardButtonImage(countryName);

        if (cardButton != null)
        {
            cardButton.SetActive(true);
            Debug.Log("Card button shown for: " + countryName);
        }
    }

    void SetCardButtonImage(string countryName)
    {
        CountryCardData countryData = null;
        foreach (var country in countries)
        {
            if (country.countryName == countryName)
            {
                countryData = country;
                break;
            }
        }

        if (cardButtonImage != null && countryData != null && countryData.cardSprite != null)
        {
            cardButtonImage.sprite = countryData.cardSprite;
            Debug.Log("Button image set to: " + countryName);
        }
    }

    public void ShowCardDetail()
    {
        if (cardDetailPanel != null)
        {
            cardDetailPanel.SetActive(true);
            LoadCountryData(currentCountry);
        }
    }

    void LoadCountryData(string countryName)
    {
        CountryCardData countryData = null;
        foreach (var country in countries)
        {
            if (country.countryName == countryName)
            {
                countryData = country;
                break;
            }
        }

        if (countryData == null)
        {
            Debug.LogWarning("Country data not found: " + countryName);
            return;
        }

        if (largeCardImage != null && countryData.cardSprite != null)
        {
            largeCardImage.sprite = countryData.cardSprite;
        }

        if (descriptionText != null)
        {
            descriptionText.text = countryData.initialMessage;
        }

        HideAllHotspotButtons();

        if (countryData.hotspotButton1 != null)
        {
            countryData.hotspotButton1.SetActive(true);
        }

        if (countryData.hotspotButton2 != null)
        {
            countryData.hotspotButton2.SetActive(true);
        }

        Debug.Log("Loaded card for: " + countryName);
    }

    void HideAllHotspotButtons()
    {
        foreach (var country in countries)
        {
            if (country.hotspotButton1 != null)
                country.hotspotButton1.SetActive(false);

            if (country.hotspotButton2 != null)
                country.hotspotButton2.SetActive(false);
        }
    }

    public void CloseCardDetail()
    {
        if (cardDetailPanel != null)
        {
            cardDetailPanel.SetActive(false);
        }
    }

    public void OnHotspot1Clicked()
    {
        CountryCardData countryData = GetCurrentCountryData();
        if (countryData != null && descriptionText != null)
        {
            descriptionText.text = countryData.hotspot1Description;
            Debug.Log("Hotspot 1 clicked for: " + currentCountry);
        }
    }

    public void OnHotspot2Clicked()
    {
        CountryCardData countryData = GetCurrentCountryData();
        if (countryData != null && descriptionText != null)
        {
            descriptionText.text = countryData.hotspot2Description;
            Debug.Log("Hotspot 2 clicked for: " + currentCountry);
        }
    }

    CountryCardData GetCurrentCountryData()
    {
        foreach (var country in countries)
        {
            if (country.countryName == currentCountry)
            {
                return country;
            }
        }
        return null;
    }
}