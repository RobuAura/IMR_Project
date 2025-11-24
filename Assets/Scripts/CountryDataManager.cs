using UnityEngine;

public class CountryDataManager : MonoBehaviour
{
    private static CountryDataManager instance;

    public static CountryDataManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject("CountryDataManager");
                instance = go.AddComponent<CountryDataManager>();
                DontDestroyOnLoad(go);
            }
            return instance;
        }
    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void UnlockCountry(string countryName)
    {
        PlayerPrefs.SetInt(countryName + "_Unlocked", 1);
        PlayerPrefs.Save();
        Debug.Log("Tara descoperita: " + countryName);
    }

    public bool IsCountryUnlocked(string countryName)
    {
        return PlayerPrefs.GetInt(countryName + "_Unlocked", 0) == 1;
    }

    public void ResetAllProgress()
    {
        PlayerPrefs.DeleteAll();
        Debug.Log("Progres resetat!");
    }
}