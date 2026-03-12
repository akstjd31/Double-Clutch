using UnityEngine;
using UnityEngine.UI;

public class StatTriangle : MaskableGraphic
{
    [Header("Stat Ratios (0 to 1)")]
    [Range(0, 200)] public float Scoring = 100f;    // 상단 (2pt + 3pt)
    [Range(0, 200)] public float Support = 100f;    // 좌하단 (Pass + Rebound)
    [Range(0, 200)] public float Disruption = 100f; // 우하단 (Block + Steal)

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();

        // 너비와 높이 중 작은 값을 기준으로 반지름(size) 결정
        float size = Mathf.Min(rectTransform.rect.width, rectTransform.rect.height) * 0.5f;
        Color32 color32 = color;

        // 최대 스탯값이 200이라면 200으로 나누어 0~1 사이 비율로 만듭니다.
        float maxStat = 65;

        // 각 꼭짓점 좌표 계산
        Vector2 posA = new Vector2(0, size * Scoring / maxStat);
        Vector2 posB = Quaternion.Euler(0, 0, 120) * new Vector2(0, size * Support / maxStat);
        Vector2 posC = Quaternion.Euler(0, 0, 240) * new Vector2(0, size * Disruption / maxStat);

        // 버텍스 추가
        vh.AddVert(posA, color32, Vector2.zero);
        vh.AddVert(posB, color32, Vector2.zero);
        vh.AddVert(posC, color32, Vector2.zero);

        // 삼각형 면 생성
        vh.AddTriangle(0, 1, 2);
    }

    // 인스펙터에서 값이 바뀔 때 즉시 반영
    protected override void OnValidate()
    {
        base.OnValidate();
        SetVerticesDirty();
    }

    // UI 크기가 바뀔 때 다시 그리기
    protected override void OnRectTransformDimensionsChange()
    {
        base.OnRectTransformDimensionsChange();
        SetVerticesDirty();
    }
}