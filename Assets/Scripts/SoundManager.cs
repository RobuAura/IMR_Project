using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class CountrySound
{
    public string countryName;
    public AudioClip audioClip;
}

public class SoundManager : MonoBehaviour
{
    public CountrySound[] countrySounds;
    public Button soundButton;
    public GameObject soundButtonObject;

    private AudioSource audioSource;
    private string currentCountry = "";

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();

        if (soundButtonObject != null)
            soundButtonObject.SetActive(false);
    }

    public void ShowSoundButton(string countryName)
    {
        if (currentCountry != countryName)
        {
            StopSound();
        }

        currentCountry = countryName;

        if (soundButtonObject != null)
            soundButtonObject.SetActive(true);
    }

    public void HideSoundButton()
    {
        currentCountry = "";

        if (soundButtonObject != null)
            soundButtonObject.SetActive(false);

        StopSound();
    }

    public void PlaySound()
    {
        AudioClip clip = GetAudioClipForCountry(currentCountry);

        if (audioSource != null && clip != null)
        {
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }

            audioSource.PlayOneShot(clip);
            Debug.Log("Sunet redat pentru: " + currentCountry);
        }
        else
        {
            Debug.LogWarning("AudioClip nu este setat pentru: " + currentCountry);
        }
    }

    public void StopSound()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }

    AudioClip GetAudioClipForCountry(string countryName)
    {
        foreach (var country in countrySounds)
        {
            if (country.countryName == countryName)
            {
                return country.audioClip;
            }
        }
        return null;
    }
}