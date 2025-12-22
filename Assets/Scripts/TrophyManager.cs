using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class TrophyManager : MonoBehaviour
{
    [Header("Trophy")]
    public GameObject trophyPrefab;

    [Header("UI")]
    public TextMeshProUGUI instructionText;

    private GameObject spawnedTrophy;
    private bool trophySpawned = false;

    void Start()
    {
        if (instructionText != null)
        {
            instructionText.text = "Trophy will appear in front of you!";
        }

        SpawnTrophyInFrontOfCamera();
    }

    void SpawnTrophyInFrontOfCamera()
    {
        if (trophyPrefab != null && !trophySpawned)
        {
            Camera mainCamera = Camera.main;

            if (mainCamera != null)
            {
                spawnedTrophy = Instantiate(trophyPrefab);

                spawnedTrophy.transform.SetParent(mainCamera.transform);

                spawnedTrophy.transform.localPosition = new Vector3(0, -1, 2.0f);
                spawnedTrophy.transform.localRotation = Quaternion.Euler(0, 180f, 0);

                trophySpawned = true;

                if (instructionText != null)
                {
                    instructionText.text = "Congratulations, Master Explorer!";
                }

                Debug.Log("Trophy attached to camera!");
            }
        }
    }

    void Update()
    {
        if (spawnedTrophy != null)
        {
            spawnedTrophy.transform.Rotate(Vector3.up, 30f * Time.deltaTime, Space.Self);
        }
    }

    public void ResetTrophy()
    {
        if (spawnedTrophy != null)
        {
            Destroy(spawnedTrophy);
            spawnedTrophy = null;
            trophySpawned = false;

            if (instructionText != null)
            {
                instructionText.text = "Trophy will appear in front of you!";
            }

            SpawnTrophyInFrontOfCamera();
        }
    }

    public void BackToCollection()
    {
        SceneManager.LoadScene("CollectionScene");
    }
}