using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class StatTriangle : MaskableGraphic
{
    [Header("Stat Settings")]
    [Range(0, 200)] public float Scoring = 100f;
    [Range(0, 200)] public float Support = 100f;
    [Range(0, 200)] public float Disruption = 100f;
    public float MaxStatValue = 200f;

    [Header("Visual Options")]
    public bool ShowAxes = true; // 이 값이 true일 때만 선을 그립니다.
    public float LineThickness = 2f;
    public Color LineColor = Color.white;

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();

        float size = Mathf.Min(rectTransform.rect.width, rectTransform.rect.height) * 0.5f;

        // 1. 각 꼭짓점 좌표 계산
        Vector2 posA = new Vector2(0, size * Scoring / MaxStatValue);
        Vector2 posB = (Vector2)(Quaternion.Euler(0, 0, 120) * new Vector3(0, size * Support / MaxStatValue, 0));
        Vector2 posC = (Vector2)(Quaternion.Euler(0, 0, 240) * new Vector3(0, size * Disruption / MaxStatValue, 0));

        // 2. 배경 삼각형 면 그리기 (기본 색상)
        AddTriangleMesh(vh, posA, posB, posC, color);

        // 3. 조건부 선 그리기 (ShowAxes가 true일 때만 실행)
        if (ShowAxes)
        {
            AddLine(vh, Vector2.zero, posA, LineThickness, LineColor);
            AddLine(vh, Vector2.zero, posB, LineThickness, LineColor);
            AddLine(vh, Vector2.zero, posC, LineThickness, LineColor);
        }
    }

    private void AddTriangleMesh(VertexHelper vh, Vector2 v1, Vector2 v2, Vector2 v3, Color32 col)
    {
        int startIndex = vh.currentVertCount;
        vh.AddVert(v1, col, Vector2.zero);
        vh.AddVert(v2, col, Vector2.zero);
        vh.AddVert(v3, col, Vector2.zero);
        vh.AddTriangle(startIndex, startIndex + 1, startIndex + 2);
    }

    private void AddLine(VertexHelper vh, Vector2 start, Vector2 end, float thickness, Color32 col)
    {
        if (start == end) return; // 길이가 0인 경우 방지

        Vector2 direction = (end - start).normalized;
        Vector2 normal = new Vector2(-direction.y, direction.x) * (thickness * 0.5f);

        int startIndex = vh.currentVertCount;

        vh.AddVert(start - normal, col, Vector2.zero);
        vh.AddVert(start + normal, col, Vector2.zero);
        vh.AddVert(end + normal, col, Vector2.zero);
        vh.AddVert(end - normal, col, Vector2.zero);

        vh.AddTriangle(startIndex, startIndex + 1, startIndex + 2);
        vh.AddTriangle(startIndex + 2, startIndex + 3, startIndex);
    }

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();
        SetVerticesDirty();
    }
#endif

}