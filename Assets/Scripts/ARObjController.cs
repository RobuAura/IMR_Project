using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ARObjController : MonoBehaviour
{
    private ARTrackedImage trackedImage;
    private bool isSelected = false;
    private Vector2 initialTouchPosition;
    private float initialRotationY;
    private float initialScale;

    public float rotationSpeed = 2f;
    public float minScale = 0.2f;
    public float maxScale = 3.0f;
    public bool faceUser = true;

    private SimpleFunFact funFactManager;
    private bool hasShownFact = false;

    void Start()
    {
        trackedImage = GetComponentInParent<ARTrackedImage>();
        if (trackedImage == null)
        {
            Debug.LogError("ARTrackedImage nu a fost gasit!");
        }
        initialScale = transform.localScale.x;

        NotifyButtonManager();

        funFactManager = FindObjectOfType<SimpleFunFact>();
    }

    void OnEnable()
    {
        NotifyButtonManager();
    }

    void Update()
    {
        if (trackedImage == null)
        {
            trackedImage = GetComponentInParent<ARTrackedImage>();
            return;
        }

        if (trackedImage.trackingState != UnityEngine.XR.ARSubsystems.TrackingState.Tracking)
            return;

     
        if (faceUser)
        {
            FaceCamera();
        }

        HandleTouchInput();

        if (trackedImage != null &&
        trackedImage.trackingState == UnityEngine.XR.ARSubsystems.TrackingState.Tracking &&
        !hasShownFact)
        {
            ShowFunFactForTargetCountry();
        }
    }

    void FaceCamera()
    {
        if (Camera.main != null)
        {
            Vector3 directionToCamera = Camera.main.transform.position - transform.position;
            directionToCamera.y = 0;

            if (directionToCamera != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(directionToCamera);
            }
        }
    }

    void HandleTouchInput()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                Ray ray = Camera.main.ScreenPointToRay(touch.position);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit) && hit.transform == transform)
                {
                    isSelected = true;
                    initialTouchPosition = touch.position;
                    initialRotationY = transform.rotation.eulerAngles.y;
                    initialScale = transform.localScale.x;
                }
            }

            if (isSelected)
            {
                if (touch.phase == TouchPhase.Moved)
                {
                    if (Input.touchCount == 1)
                    {
                        float deltaX = touch.position.x - initialTouchPosition.x;
                        float newRotationY = initialRotationY + deltaX * rotationSpeed;
                        faceUser = false;
                        transform.rotation = Quaternion.Euler(0, newRotationY, 0);
                    }
                    else if (Input.touchCount == 2)
                    {
                        Touch touch1 = Input.GetTouch(0);
                        Touch touch2 = Input.GetTouch(1);

                        Vector2 touch1PrevPos = touch1.position - touch1.deltaPosition;
                        Vector2 touch2PrevPos = touch2.position - touch2.deltaPosition;

                        float prevDistance = Vector2.Distance(touch1PrevPos, touch2PrevPos);
                        float currentDistance = Vector2.Distance(touch1.position, touch2.position);

                        float scaleFactor = currentDistance / prevDistance;
                        float newScale = initialScale * scaleFactor;

                        newScale = Mathf.Clamp(newScale, minScale, maxScale);
                        transform.localScale = Vector3.one * newScale;
                    }
                }

                if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                {
                    isSelected = false;
                }
            }
        }
    }

   
    public void RotateLeft()
    {
        faceUser = false;
        transform.Rotate(0, -45, 0);
        Debug.Log("Rotire stanga aplicata");
    }

    public void RotateRight()
    {
        faceUser = false;
        transform.Rotate(0, 45, 0);
        Debug.Log("Rotire dreapta aplicata");
    }

    public void ScaleUp()
    {
        float newScale = Mathf.Clamp(transform.localScale.x * 1.3f, minScale, maxScale);
        transform.localScale = Vector3.one * newScale;
        Debug.Log("Marire aplicata. Noua scala: " + newScale);
    }

    public void ScaleDown()
    {
        float newScale = Mathf.Clamp(transform.localScale.x * 0.7f, minScale, maxScale);
        transform.localScale = Vector3.one * newScale;
        Debug.Log("Micsorare aplicata. Noua scala: " + newScale);
    }

    public void ResetOrientation()
    {
        faceUser = true;
        Debug.Log("Orientare resetata");
    }

    void NotifyButtonManager()
    {
        ARButtonManager buttonManager = FindObjectOfType<ARButtonManager>();
        if (buttonManager != null)
        {
            buttonManager.SetCurrentARObject(this);
        }
    }

    void ShowFunFactForTargetCountry()
    {
        if (funFactManager != null && trackedImage != null)
        {
            
            funFactManager.OnImageDetected(trackedImage.referenceImage.name);
            hasShownFact = true;
        }
    }
}