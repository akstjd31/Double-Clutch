using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LayoutRefreshHelper : MonoBehaviour
{
    [SerializeField] private RectTransform target;

    private void OnEnable()
    {
        if (target == null)
            target = GetComponent<RectTransform>();

        StartCoroutine(RefreshLayout());
    }

    private IEnumerator RefreshLayout()
    {
        yield return null; // 한 프레임 대기
        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(target);
    }
}