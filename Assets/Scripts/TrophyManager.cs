using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using DG.Tweening;

public class TrophyManager : MonoBehaviour
{
    [Header("Trophy")]
    public GameObject trophyPrefab;

    [Header("UI")]
    public TextMeshProUGUI instructionText;

    [Header("Animation Settings")]
    public float spawnDelay = 0.7f;
    public float scaleAnimationDuration = 3.0f;
    public Ease scaleEase = Ease.OutElastic;

    private GameObject spawnedTrophy;
    private bool trophySpawned = false;

    void Start()
    {
        if (instructionText != null)
        {
            instructionText.text = "Trophy will appear in front of you!";
        }

        DOVirtual.DelayedCall(spawnDelay, () => {
            SpawnTrophyInFrontOfCamera();
        });
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

                AnimateTrophyAppearance();

                trophySpawned = true;

                if (instructionText != null)
                {
                    DOVirtual.DelayedCall(scaleAnimationDuration, () =>
                    {
                        if (instructionText != null)
                        {
                            instructionText.text = "Congratulations, Master Explorer!";
                        }
                    });
                }

                Debug.Log("Trophy attached to camera!");
            }
        }
    }

    void AnimateTrophyAppearance()
    {
        Vector3 targetScale = spawnedTrophy.transform.localScale;

        spawnedTrophy.transform.localScale = Vector3.zero;

        spawnedTrophy.transform.DOScale(targetScale, scaleAnimationDuration)
             .SetEase(scaleEase)
             .OnComplete(() =>
             {
                 Debug.Log("Trophy scale animation completed!");
             });
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
            spawnedTrophy.transform.DOScale(Vector3.zero, 0.3f)
                .SetEase(Ease.InBack)
                .OnComplete(() => {
                    Destroy(spawnedTrophy);
                    spawnedTrophy = null;
                    trophySpawned = false;

                    if (instructionText != null)
                    {
                        instructionText.text = "Trophy will appear in front of you!";
                    }

                    SpawnTrophyInFrontOfCamera();
                });
        }
    }

    public void BackToCollection()
    {
        SceneManager.LoadScene("CollectionScene");
    }
}