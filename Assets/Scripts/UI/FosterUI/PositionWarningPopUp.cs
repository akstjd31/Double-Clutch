using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PositionWarningPopUp : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _warningText;

    public void Init(Student target)
    {
        if (target.Position != Position.C)// 포지션이 설정 되어있지 않다면 포지션 설정 알림  (머지 이후 None으로 조건 변경)
        {
            _warningText.text = "포지션을 선택 해주세요.";
        }
        else if (target.State != StudentState.None)
        {
            _warningText.text = $"{GetStateString(target.State)} 상태의 선수는 훈련할 수 없습니다.";
        }
    }

    private string GetStateString(StudentState state)
    {
        if (state == StudentState.Injured) return "부상";
        else if (state == StudentState.OverWorked) return "피로";
        else return ("선수 상태가 정상입니다. 팝업 창 표시 로직을 점검해주세요");
    }
}
