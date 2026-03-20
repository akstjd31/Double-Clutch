using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class SafeAreaApplier : MonoBehaviour
{
    private RectTransform _panel;

    void Awake()
    {
        _panel = GetComponent<RectTransform>();
        ApplySafeArea();
    }

    void ApplySafeArea()
    {
        Rect safeArea = Screen.safeArea;

        Vector2 anchorMin = safeArea.position;
        Vector2 anchorMax = safeArea.position + safeArea.size;

        anchorMin.x /= Screen.width;
        anchorMin.y /= Screen.height;
        anchorMax.x /= Screen.width;
        anchorMax.y /= Screen.height;

        _panel.anchorMin = anchorMin;
        _panel.anchorMax = anchorMax;
    }
}