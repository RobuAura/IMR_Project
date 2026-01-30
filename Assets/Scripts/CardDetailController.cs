using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

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

    [Header("Hotspot Animation Settings")]
    public Color discoveredColor = new Color(0.4f, 1f, 0.8f, 0.3f);
    public float animationDuration = 0.5f;
    public Ease revealEase = Ease.OutBack;

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

    public void HideCardButton(bool instant = false)
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
            SetHotspotInvisible(countryData.hotspotButton1);
        }

        if (countryData.hotspotButton2 != null)
        {
            countryData.hotspotButton2.SetActive(true);
            SetHotspotInvisible(countryData.hotspotButton2);
        }

        Debug.Log("Loaded card for: " + countryName);
    }

    void SetHotspotInvisible(GameObject hotspot)
    {
        Image img = hotspot.GetComponent<Image>();
        if (img != null)
        {
            Color invisible = img.color;
            invisible.a = 0; 
            img.color = invisible;
        }
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
        StopAllHotspotPulses();
    }

    void StopAllHotspotPulses()
    {
        foreach (var country in countries)
        {
            if (country.hotspotButton1 != null)
            {
                country.hotspotButton1.transform.DOKill();

                Image img1 = country.hotspotButton1.GetComponent<Image>();
                if (img1 != null)
                {
                    img1.DOKill();
                }
            }

            if (country.hotspotButton2 != null)
            {
                country.hotspotButton2.transform.DOKill();

                Image img2 = country.hotspotButton2.GetComponent<Image>();
                if (img2 != null)
                {
                    img2.DOKill();
                }
            }
        }

        Debug.Log("All hotspot pulses stopped!");
    }

    public void OnHotspot1Clicked()
    {
        CountryCardData countryData = GetCurrentCountryData();
        if (countryData != null && descriptionText != null)
        {
            descriptionText.text = countryData.hotspot1Description;
            AnimateHotspotDiscovered(countryData.hotspotButton1);
            Debug.Log("Hotspot 1 clicked for: " + currentCountry);
        }
    }

    public void OnHotspot2Clicked()
    {
        CountryCardData countryData = GetCurrentCountryData();
        if (countryData != null && descriptionText != null)
        {
            descriptionText.text = countryData.hotspot2Description;
            AnimateHotspotDiscovered(countryData.hotspotButton2);
            Debug.Log("Hotspot 2 clicked for: " + currentCountry);
        }
    }

    void AnimateHotspotDiscovered(GameObject hotspot)
    {
        if (hotspot == null) return;

        Image img = hotspot.GetComponent<Image>();
        if (img == null) return;

        if (img.color.a > 0.1f) return;

        Vector3 originalScale = hotspot.transform.localScale;

        hotspot.transform.DOKill();
        img.DOKill();

        Sequence revealSeq = DOTween.Sequence();
        revealSeq.Append(hotspot.transform.DOScale(originalScale * 1.3f, 0.2f).SetEase(Ease.OutQuad));
        revealSeq.Append(hotspot.transform.DOScale(originalScale, 0.3f).SetEase(Ease.InOutQuad));
        revealSeq.OnComplete(() => {
            StartHotspotPulse(hotspot, originalScale);
        });

        img.DOColor(discoveredColor, animationDuration).SetEase(revealEase);

        Debug.Log("Hotspot discovered animation played!");
    }

    void StartHotspotPulse(GameObject hotspot, Vector3 originalScale)
    {
        if (hotspot == null) return;

        Image img = hotspot.GetComponent<Image>();

        Sequence pulseLoop = DOTween.Sequence();
        pulseLoop.Append(hotspot.transform.DOScale(originalScale * 1.1f, 0.8f).SetEase(Ease.InOutQuad));
        pulseLoop.Append(hotspot.transform.DOScale(originalScale, 0.8f).SetEase(Ease.InOutQuad));
        pulseLoop.SetLoops(-1, LoopType.Restart);

        if (img != null)
        {
            Color verde = new Color(0.4f, 1f, 0.8f, 0.35f);
            Color albastru = new Color(0.3f, 0.7f, 1f, 0.35f);
            Color roz = new Color(1f, 0.5f, 0.8f, 0.35f);
            Color portocaliu = new Color(1f, 0.7f, 0.4f, 0.35f);

            Sequence colorLoop = DOTween.Sequence();
            colorLoop.Append(img.DOColor(albastru, 1.2f).SetEase(Ease.InOutSine));
            colorLoop.Append(img.DOColor(roz, 1.2f).SetEase(Ease.InOutSine));
            colorLoop.Append(img.DOColor(portocaliu, 1.2f).SetEase(Ease.InOutSine));
            colorLoop.Append(img.DOColor(verde, 1.2f).SetEase(Ease.InOutSine));
            colorLoop.SetLoops(-1, LoopType.Restart);
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