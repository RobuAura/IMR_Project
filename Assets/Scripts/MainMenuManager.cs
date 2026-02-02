using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class MainMenuManager : MonoBehaviour
{
    [Header("Menu Buttons")]
    public Button scanFlagButton;
    public Button quizButton;

    [Header("Animation Settings")]
    public float clickScale = 1.08f;
    public float clickDuration = 0.12f;

    public void OnScanFlagButtonClicked()
    {
        Debug.Log("Scan Flag button clicked!");

        if (scanFlagButton != null)
        {
            AnimateButton(scanFlagButton);
        }

        float totalDelay = (clickDuration * 2) + 0.03f;

        DOVirtual.DelayedCall(totalDelay, () => {
            SceneManager.LoadScene("ARScanScene");
        });
    }

    public void OnQuizButtonClicked()
    {
        Debug.Log("Quiz button clicked!");

        if (quizButton != null)
        {
            AnimateButton(quizButton);
        }

        float totalDelay = (clickDuration * 2) + 0.03f;

        DOVirtual.DelayedCall(totalDelay, () => {
            SceneManager.LoadScene("QuizScene");
        });
    }

    void AnimateButton(Button button)
    {
        Vector3 originalScale = button.transform.localScale;

        button.transform.DOKill();

        Sequence clickSeq = DOTween.Sequence();
        clickSeq.Append(button.transform.DOScale(originalScale * clickScale, clickDuration).SetEase(Ease.OutQuad));
        clickSeq.Append(button.transform.DOScale(originalScale, clickDuration).SetEase(Ease.InQuad));

        Debug.Log("Button animation started for: " + button.name);
    }
}
