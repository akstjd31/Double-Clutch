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

    public void Init(string name, StudentState state)
    {
        string word = GetStateString(state);
        _warningText.text = $"{name} (상태 : {word})\n{word} 상태로 인해 훈련에서 제외됩니다.";
    }

    private string GetStateString(StudentState state)
    {
        if (state == StudentState.Injured) return "부상";
        else if (state == StudentState.OverWorked) return "과로";
        else return ("선수 상태가 정상입니다. 팝업 창 표시 로직을 점검해주세요");
    }
}
