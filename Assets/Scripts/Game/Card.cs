using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Card : MonoBehaviour
{
    [Header("References")]
    public GameObject frontFace;

    public bool IsFlipped { get; private set; } = false;
    public bool IsMatched { get; private set; } = false;
    private bool isFlipping = false;

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
    }
}