using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.InputSystem.EnhancedTouch;
using DG.Tweening;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class ARObjController : MonoBehaviour
{
    private ARTrackedImage trackedImage;
    private bool isSelected = false;
    private Vector2 initialTouchPosition;
    private float initialRotationY;
    private float initialScale;

    public float rotationSpeed = 0.5f;
    public float minScale = 0.2f;
    public float maxScale = 3.0f;
    public float initialYRotation = 0f;

    [Header("Reset Settings")]
    public Vector3 defaultScale = new Vector3(1, 1, 1);

    [Header("Animation Settings")]
    public float spawnDelay = 0.5f;
    public float animationDuration = 3.5f;
    public Ease animationEase = Ease.OutElastic;

    private bool hasInitialRotation = false;
    private bool hasAnimated = false;

    private SimpleFunFact funFactManager;
    private bool hasShownFact = false;

    void Awake()
    {
        defaultScale = transform.localScale;
        Debug.Log($"Awake() {gameObject.name}: Saved defaultScale = {defaultScale}");
    }

    void OnEnable()
    {
        EnhancedTouchSupport.Enable();
        ResetToInitialState();
    }

    void OnDisable()
    {
        EnhancedTouchSupport.Disable();
        hasInitialRotation = false;
        hasShownFact = false;
        hasAnimated = false;

        transform.DOKill();

        CardDetailController cardController = FindObjectOfType<CardDetailController>();
        if (cardController != null && trackedImage != null)
        {
            string myCountry = trackedImage.referenceImage.name;
            string currentCountry = cardController.GetCurrentCountry();

            if (currentCountry == myCountry)
            {
                cardController.HideCardButton(instant: true);
                Debug.Log("Hiding card for: " + myCountry);
            }
        }
    }

    void Start()
    {
        trackedImage = GetComponentInParent<ARTrackedImage>();
        initialScale = defaultScale.x; 

        funFactManager = FindObjectOfType<SimpleFunFact>();

        transform.localScale = Vector3.zero;

        Debug.Log($"Start() {gameObject.name}: Will animate to defaultScale = {defaultScale}");
    }

    void Update()
    {
        if (trackedImage == null)
        {
            trackedImage = GetComponentInParent<ARTrackedImage>();
            return;
        }

        if (trackedImage.trackingState != UnityEngine.XR.ARSubsystems.TrackingState.Tracking)
        {
            if (hasAnimated)
            {
                ResetToInitialState();
            }
            return;
        }

        if (!hasAnimated && gameObject.activeSelf)
        {
            DOVirtual.DelayedCall(spawnDelay, () => {
                if (gameObject.activeSelf)
                {
                    AnimateScaleUp();
                }
            });
            hasAnimated = true;
        }

        if (!hasInitialRotation)
        {
            SetInitialOrientation();
            hasInitialRotation = true;
        }

        HandleTouchInput();

        if (!hasShownFact)
        {
            ShowFunFactForTargetCountry();
        }
    }

    void AnimateScaleUp()
    {
        transform.DOScale(defaultScale, animationDuration)
             .SetEase(animationEase)
             .OnComplete(() =>
             {
                 Debug.Log($"Animation completed for {gameObject.name} at scale {transform.localScale}");
             });
    }

    void SetInitialOrientation()
    {
        if (Camera.main != null)
        {
            Vector3 directionToCamera = Camera.main.transform.position - transform.position;
            directionToCamera.y = 0;

            if (directionToCamera != Vector3.zero)
            {
                float yRotation = Quaternion.LookRotation(directionToCamera).eulerAngles.y;
                transform.rotation = Quaternion.Euler(0, yRotation + initialYRotation, 0);
            }
        }
    }

    void ResetToInitialState()
    {
        transform.localScale = Vector3.zero;
        hasInitialRotation = false;
        hasShownFact = false;
        hasAnimated = false;

        Debug.Log("Reset to initial state for: " + gameObject.name);
    }

    void HandleTouchInput()
    {
        var touches = Touch.activeTouches;

        if (touches.Count == 0) return;

        var touch = touches[0];

        if (touches.Count == 1)
        {
            if (touch.phase == UnityEngine.InputSystem.TouchPhase.Began)
            {
                Ray ray = Camera.main.ScreenPointToRay(touch.screenPosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.transform == transform || hit.transform.IsChildOf(transform) || transform.IsChildOf(hit.transform.root))
                    {
                        isSelected = true;
                        initialTouchPosition = touch.screenPosition;
                        initialRotationY = transform.rotation.eulerAngles.y;
                    }
                }
            }

            if (isSelected && touch.phase == UnityEngine.InputSystem.TouchPhase.Moved)
            {
                float deltaX = touch.screenPosition.x - initialTouchPosition.x;
                float newRotationY = initialRotationY + deltaX * rotationSpeed;
                transform.rotation = Quaternion.Euler(0, newRotationY, 0);
            }

            if (touch.phase == UnityEngine.InputSystem.TouchPhase.Ended ||
                touch.phase == UnityEngine.InputSystem.TouchPhase.Canceled)
            {
                isSelected = false;
            }
        }

        if (touches.Count == 2)
        {
            var touch1 = touches[0];
            var touch2 = touches[1];

            Vector2 touch1PrevPos = touch1.screenPosition - touch1.delta;
            Vector2 touch2PrevPos = touch2.screenPosition - touch2.delta;

            float prevDistance = Vector2.Distance(touch1PrevPos, touch2PrevPos);
            float currentDistance = Vector2.Distance(touch1.screenPosition, touch2.screenPosition);

            if (prevDistance > 0)
            {
                float scaleFactor = currentDistance / prevDistance;
                float newScale = transform.localScale.x * scaleFactor;

                newScale = Mathf.Clamp(newScale, minScale, maxScale);
                transform.localScale = Vector3.one * newScale;
            }
        }
    }

    void ShowFunFactForTargetCountry()
    {
        if (funFactManager != null && trackedImage != null)
        {
            string countryName = trackedImage.referenceImage.name;
            funFactManager.OnImageDetected(countryName);
            hasShownFact = true;

            CardDetailController cardController = FindObjectOfType<CardDetailController>();
            if (cardController != null)
            {
                cardController.ShowCardButton(countryName);
            }
        }
    }
}