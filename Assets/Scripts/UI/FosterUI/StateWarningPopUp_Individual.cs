using TMPro;
using UnityEngine;


/// <summary>
/// 부상 선수에게 개인 훈련을 예약하려고 시도할 때 나오는 경고 알림창. 팀 훈련시 경고창은 별도 스크립트 없음. 
/// </summary>
public class StateWarningPopUp_Individual : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _warningText;

    public void Init(Student target)
    {
        _warningText.text = $"{GetStateString(target.State)} 상태의 선수는 훈련할 수 없습니다.";        
    }

    private string GetStateString(StudentState state)
    {
        if (state == StudentState.Injured) return "부상";
        else if (state == StudentState.OverWorked) return "과로";
        else return ("선수 상태가 정상입니다. 팝업 창 표시 로직을 점검해주세요");
    }
}
