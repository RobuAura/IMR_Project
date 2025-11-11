using UnityEngine;
using UnityEngine.UI;

public class ARButtonManager : MonoBehaviour
{
    private ARObjController currentARObject;

    // Metode pentru butoane
    public void SetCurrentARObject(ARObjController obj)
    {
        currentARObject = obj;
        Debug.Log("Obiect AR setat pentru control: " + (obj != null ? obj.name : "null"));
    }

    public void RotateLeft()
    {
        if (currentARObject != null)
        {
            currentARObject.RotateLeft();
            Debug.Log("Rotire stanga");
        }
        else
        {
            Debug.LogWarning("Niciun obiect AR selectat pentru rotire!");
        }
    }

    public void RotateRight()
    {
        if (currentARObject != null)
        {
            currentARObject.RotateRight();
            Debug.Log("Rotire dreapta");
        }
        else
        {
            Debug.LogWarning("Niciun obiect AR selectat pentru rotire!");
        }
    }

    public void ScaleUp()
    {
        if (currentARObject != null)
        {
            currentARObject.ScaleUp();
            Debug.Log("Marire");
        }
        else
        {
            Debug.LogWarning("Niciun obiect AR selectat pentru scalare!");
        }
    }

    public void ScaleDown()
    {
        if (currentARObject != null)
        {
            currentARObject.ScaleDown();
            Debug.Log("Micsorare");
        }
        else
        {
            Debug.LogWarning("Niciun obiect AR selectat pentru scalare!");
        }
    }

    public void ResetOrientation()
    {
        if (currentARObject != null)
        {
            currentARObject.ResetOrientation();
            Debug.Log("Resetare orientare");
        }
    }
}