using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HeightLine : MonoBehaviour
{
    [SerializeField] private RectTransform _topLine;
    [SerializeField] private RectTransform _bottomLine;

    [SerializeField] private RectTransform _height;


    private void OnEnable()
    {
        StartCoroutine(Co_Line());
    }

    IEnumerator Co_Line()
    {
        _height.gameObject.SetActive(false);
        yield return null;
        yield return null;
        Line();
        _height.gameObject.SetActive(true);
    }

    private void Line()
    {
        Canvas.ForceUpdateCanvases();

        float topY = _topLine.anchoredPosition.y;
        float bottomY = _bottomLine.anchoredPosition.y;

        float distance = Mathf.Abs(topY - bottomY);
        distance = Mathf.Round(distance);

        _height.sizeDelta = new Vector2(4f, distance);

        _height.anchoredPosition = new Vector2(_height.anchoredPosition.x,
                                                Mathf.Round(_height.anchoredPosition.y)
);
    }
}
