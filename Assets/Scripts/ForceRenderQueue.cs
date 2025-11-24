using UnityEngine;

public class ForceRenderQueue : MonoBehaviour
{
    [Tooltip("Render Queue value (3000 = deasupra AR background)")]
    public int renderQueue = 3000;

    [Tooltip("Aplica la toate materialele copiilor?")]
    public bool applyToChildren = true;

    void Start()
    {
        ApplyRenderQueue();
    }

    void ApplyRenderQueue()
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            foreach (Material mat in renderer.materials)
            {
                mat.renderQueue = renderQueue;
                Debug.Log("Render Queue setat la " + renderQueue + " pentru " + gameObject.name + " - Material: " + mat.name);
            }
        }

        
        if (applyToChildren)
        {
            Renderer[] childRenderers = GetComponentsInChildren<Renderer>();
            foreach (Renderer childRenderer in childRenderers)
            {
                if (childRenderer.gameObject == gameObject) continue; 

                foreach (Material mat in childRenderer.materials)
                {
                    mat.renderQueue = renderQueue;
                    Debug.Log("Render Queue setat la " + renderQueue + " pentru child: " + childRenderer.gameObject.name + " - Material: " + mat.name);
                }
            }
        }
    }

    
    void OnValidate()
    {
        if (Application.isPlaying)
        {
            ApplyRenderQueue();
        }
    }
}