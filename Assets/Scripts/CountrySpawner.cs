using UnityEngine;
using UnityEngine.XR.ARFoundation;
using System.Collections.Generic;

public class CountrySpawner : MonoBehaviour
{
    [System.Serializable]
    public class CountryData
    {
        public string countryName;
        public GameObject prefab;
    }

    public CountryData[] countries;
    private ARTrackedImageManager trackedImageManager;
    private Dictionary<string, GameObject> spawnedObjects = new Dictionary<string, GameObject>();
    private string currentActiveCountry = "";

    void OnEnable()
    {
        trackedImageManager = GetComponent<ARTrackedImageManager>();

        if (trackedImageManager == null)
        {
            Debug.LogError("AR Tracked Image Manager NU GASIT!");
            return;
        }

        trackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
    }

    void OnDisable()
    {
        if (trackedImageManager != null)
            trackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
    }

    void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (var trackedImage in eventArgs.added)
        {
            SpawnContent(trackedImage);
        }

        foreach (var trackedImage in eventArgs.updated)
        {
            UpdateTrackedImage(trackedImage);
        }

        foreach (var trackedImage in eventArgs.removed)
        {
            RemoveContent(trackedImage);
        }
    }

    void SpawnContent(ARTrackedImage trackedImage)
    {
        string imageName = trackedImage.referenceImage.name;

        if (spawnedObjects.ContainsKey(imageName))
        {
            ActivateCountry(imageName);
            return;
        }

        foreach (var country in countries)
        {
            if (country.countryName == imageName)
            {
                GameObject obj = Instantiate(country.prefab, trackedImage.transform);
                obj.transform.localPosition = Vector3.zero;
                obj.transform.localRotation = Quaternion.identity;

                spawnedObjects[imageName] = obj;

                Debug.Log("Spawnat: " + imageName);

                ActivateCountry(imageName);
                break;
            }
        }
    }

    void UpdateTrackedImage(ARTrackedImage trackedImage)
    {
        string imageName = trackedImage.referenceImage.name;

        if (spawnedObjects.ContainsKey(imageName))
        {
            bool isTracking = trackedImage.trackingState == UnityEngine.XR.ARSubsystems.TrackingState.Tracking;

            if (isTracking)
            {
                ActivateCountry(imageName);
            }
            else
            {
                DeactivateCountry(imageName);
            }
        }
    }

    void RemoveContent(ARTrackedImage trackedImage)
    {
        string imageName = trackedImage.referenceImage.name;
        DeactivateCountry(imageName);
    }

    void ActivateCountry(string countryName)
    {
        if (currentActiveCountry == countryName)
            return;

        if (!string.IsNullOrEmpty(currentActiveCountry) && currentActiveCountry != countryName)
        {
            DeactivateCountry(currentActiveCountry);
        }

        currentActiveCountry = countryName;

        if (spawnedObjects.ContainsKey(countryName))
        {
            spawnedObjects[countryName].SetActive(true);
        }

        NotifyUIManagers(countryName, true);
    }

    void DeactivateCountry(string countryName)
    {
        if (spawnedObjects.ContainsKey(countryName))
        {
            spawnedObjects[countryName].SetActive(false);
        }

        if (currentActiveCountry == countryName)
        {
            currentActiveCountry = "";
            HideAllUI();
        }
    }

    void NotifyUIManagers(string countryName, bool isVisible)
    {
        if (isVisible)
        {
            SimpleFunFact funFactManager = FindObjectOfType<SimpleFunFact>();
            if (funFactManager != null)
            {
                funFactManager.OnImageDetected(countryName);
            }
        }
    }

    void HideAllUI()
    {
        SimpleFunFact funFactManager = FindObjectOfType<SimpleFunFact>();
        if (funFactManager != null)
        {
            funFactManager.HideFunFact();
        }

        CountryInfoManager infoManager = FindObjectOfType<CountryInfoManager>();
        if (infoManager != null)
        {
            infoManager.HideCountryInfo();
        }

        SoundManager soundManager = FindObjectOfType<SoundManager>();
        if (soundManager != null)
        {
            soundManager.HideSoundButton();
        }
    }
}