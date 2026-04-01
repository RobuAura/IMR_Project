using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class Card : MonoBehaviour
{
    [Header("References")]
    public GameObject frontFace;
    public TextMeshProUGUI cardNumberText;
    public Image flagImage;

    public int CardId { get; private set; } = -1;
    public bool IsFlipped { get; private set; } = false;
    public bool IsMatched { get; private set; } = false;
    private bool isFlipping = false;

    private Color originalColor;

    public string CountryName { get; private set; } = "";

    void Awake()
    {
        originalColor = GetComponent<Image>().color;
    }

    public void SetHighlight(bool highlighted, Color highlightColor)
    {
        GetComponent<Image>().color = highlighted ? highlightColor : originalColor;
    }

    public void SetCard(int id, Sprite flagSprite = null, string countryName = "")
    {
        CardId = id;
        CountryName = countryName;

        if (flagSprite != null)
        {
            flagImage.sprite = flagSprite;
            flagImage.gameObject.SetActive(true);
            cardNumberText.gameObject.SetActive(false);
        }
        else
        {
            flagImage.gameObject.SetActive(false);
            cardNumberText.gameObject.SetActive(true);
            cardNumberText.text = id.ToString();
        }
    }

    public void OnCardClicked()
    {
        if (IsFlipped || isFlipping || IsMatched) return;

        MemoryGameManager.Instance.OnCardSelected(this);
    }

    public void FlipToFront()
    {
        isFlipping = true;

        Sequence flipSeq = DOTween.Sequence();
        flipSeq.Append(transform.DOScaleX(0f, 0.15f).SetEase(Ease.InQuad));
        flipSeq.AppendCallback(() => frontFace.SetActive(true));
        flipSeq.Append(transform.DOScaleX(1f, 0.15f).SetEase(Ease.OutQuad));
        flipSeq.OnComplete(() =>
        {
            IsFlipped = true;
            isFlipping = false;
        });
    }

    public void FlipToBack()
    {
        isFlipping = true;

        Sequence flipSeq = DOTween.Sequence();
        flipSeq.Append(transform.DOScaleX(0f, 0.15f).SetEase(Ease.InQuad));
        flipSeq.AppendCallback(() => frontFace.SetActive(false));
        flipSeq.Append(transform.DOScaleX(1f, 0.15f).SetEase(Ease.OutQuad));
        flipSeq.OnComplete(() =>
        {
            IsFlipped = false;
            isFlipping = false;
        });
    }

    public void SetMatched()
    {
        IsMatched = true;

        Sequence matchSeq = DOTween.Sequence();
        matchSeq.Append(transform.DOScale(Vector3.one * 1.15f, 0.15f).SetEase(Ease.OutQuad));
        matchSeq.Append(transform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InQuad));
        matchSeq.OnComplete(() =>
        {
            GetComponent<Image>().color = new Color(0, 0, 0, 0);
            frontFace.SetActive(false);
        });
    }
}