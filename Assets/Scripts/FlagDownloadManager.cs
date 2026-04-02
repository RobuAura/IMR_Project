using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using System.Collections.Generic;

public class FlagDownloadManager : MonoBehaviour
{
    [Header("Panel")]
    public GameObject downloadPanel;    
    public Button closeButton;         
    public Button downloadSelectedButton; 
    public Button resourcesButton;     

    [Header("Flag Items")]
    public List<Toggle> flagToggles;   
    public List<Texture2D> flagTextures; 
    public List<string> flagNames;     

    void Start()
    {
        downloadPanel.SetActive(false);
    }

    public void OpenPanel()
    {
        if (resourcesButton != null) resourcesButton.interactable = false;

        downloadPanel.SetActive(true);
        downloadPanel.transform.localScale = Vector3.one * 0.8f;
        downloadPanel.transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack);
    }

    public void ClosePanel()
    {
        if (resourcesButton != null) resourcesButton.interactable = true;

        downloadPanel.transform.DOScale(Vector3.one * 0.8f, 0.15f)
            .SetEase(Ease.InBack)
            .OnComplete(() => downloadPanel.SetActive(false));
    }

    public void OnDownloadSelectedClicked()
    {
        int downloaded = 0;

        for (int i = 0; i < flagToggles.Count; i++)
        {
            if (flagToggles[i].isOn)
            {
                Texture2D texture = flagTextures[i];
                string name = flagNames[i];

                NativeGallery.SaveImageToGallery(
                    texture, "FlagExplorer", name + ".png",
                    (success, path) => Debug.Log($"Saved {name}: {success} at {path}")
                );

                downloaded++;
            }
        }

        Debug.Log($"Downloaded {downloaded} flags!");
    }
}