using UnityEngine;
using UnityEngine.Playables;
using Cinemachine;
using System.Collections;

public class LookAtObjectPrompt : MonoBehaviour
{
    [Header("Raycast Settings")]
    public Transform rayOrigin;
    public float rayDistance = 5f;
    public float sphereRadius = 0.5f;
    public GameObject targetObject;

    [Header("Prompt Settings")]
    public string promptText = "E";
    public int fontSize = 32;
    public Color fontColor = Color.white;
    public Vector3 promptOffset = new Vector3(0f, 2f, 0f);

    [Header("Debug Settings")]
    public bool debugRay = false;
    public Color debugRayColor = Color.red;

    [Header("Animation Settings")]
    public float pressAnimationDuration = 0.5f;
    public float pressScale = 1.5f;
    public Color pressColor = Color.green;

    [Header("Activation Settings")]
    public bool disableAfterPress = true;
    public PlayableDirector timeline;

    private bool showPrompt = false;
    private Vector3 promptWorldPosition;
    private GUIStyle guiStyle;

    private bool isAnimating = false;
    private float animatedFontSize = 0f;
    private bool hasActivated = false;

    private float originalXSpeed;
    private float originalYSpeed;

    void Awake()
    {

    }

    void Start()
    {
        guiStyle = new GUIStyle
        {
            fontSize = fontSize,
            normal = { textColor = fontColor },
            alignment = TextAnchor.MiddleCenter
        };
    }

    void Update()
    {
        Ray ray = new Ray(rayOrigin.position, rayOrigin.forward);

        if (hasActivated)
        {
            if (debugRay)
                Debug.DrawRay(ray.origin, ray.direction * rayDistance, debugRayColor);
            return;
        }

        showPrompt = false;
        if (Physics.SphereCast(ray, sphereRadius, out RaycastHit hit, rayDistance))
        {
            bool isTarget = hit.collider.gameObject == targetObject;

            if (debugRay)
            {
                Color col = isTarget ? Color.green : debugRayColor;
                Debug.DrawRay(ray.origin, ray.direction * rayDistance, col);
            }

            if (isTarget && !isAnimating)
            {
                showPrompt = true;
                promptWorldPosition = hit.point + promptOffset;

                if (Input.GetKeyDown(KeyCode.E))
                {
                    StartCoroutine(PlayPressAnimation());
                    if (timeline != null) timeline.Play();
                    if (disableAfterPress) hasActivated = true;

                    showPrompt = false;
                }
            }
        }
        else if (debugRay)
        {
            Debug.DrawRay(ray.origin, ray.direction * rayDistance, debugRayColor);
        }
    }

    private IEnumerator PlayPressAnimation()
    {
        isAnimating = true;
        float elapsed = 0f;
        float startSize = fontSize * pressScale;

        while (elapsed < pressAnimationDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / pressAnimationDuration;
            animatedFontSize = Mathf.Lerp(startSize, 0f, t);
            yield return null;
        }

        isAnimating = false;
    }

    void OnGUI()
    {
        if (isAnimating)
        {
            Draw(animatedFontSize, pressColor);
            return;
        }
        if (!showPrompt) return;
        Draw(fontSize, fontColor);
    }

    private void Draw(float size, Color color)
    {
        guiStyle.fontSize = Mathf.RoundToInt(size);
        guiStyle.normal.textColor = color;

        Vector3 screenPos = Camera.main.WorldToScreenPoint(promptWorldPosition);
        if (screenPos.z > 0)
        {
            Vector2 dim = guiStyle.CalcSize(new GUIContent(promptText));
            Rect rect = new Rect(
                screenPos.x - dim.x / 2,
                Screen.height - screenPos.y - dim.y / 2,
                dim.x, dim.y
            );
            GUI.Label(rect, promptText, guiStyle);
        }
    }

    // ќтключает вращение камеры мышью
    public void DisableCameraControl(CinemachineFreeLook freeLookCamera)
    {
        originalXSpeed = freeLookCamera.m_XAxis.m_MaxSpeed;
        originalYSpeed = freeLookCamera.m_YAxis.m_MaxSpeed;
        if (freeLookCamera != null)
        {
            freeLookCamera.m_XAxis.m_MaxSpeed = 0f;
            freeLookCamera.m_YAxis.m_MaxSpeed = 0f;
        }
    }

    // ¬ключает вращение камеры мышью обратно
    public void EnableCameraControl(CinemachineFreeLook freeLookCamera)
    {
        if (freeLookCamera != null)
        {
            freeLookCamera.m_XAxis.m_MaxSpeed = originalXSpeed;
            freeLookCamera.m_YAxis.m_MaxSpeed = originalYSpeed;
        }
    }
}