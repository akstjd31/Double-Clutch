using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 개별 Problem(경고창 프리팹)에 할당할 스크립트
/// 경고 표시기능만 탑재
/// </summary>
public class Problem : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _warningText;

    public void Init(string name)
    {
        _warningText.text = $"{name} (컨디션 : 0)\n선수의 컨디션이 낮습니다!";
    }
}
