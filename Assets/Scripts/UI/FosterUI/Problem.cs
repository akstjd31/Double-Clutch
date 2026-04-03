using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 개별 Problem(경고창 프리팹)에 할당할 스크립트
/// 경고 표시기능만 탑재
/// </summary>
public class Problem : MonoBehaviour
{    
    [SerializeField] TextMeshProUGUI _warning1;
    [SerializeField] TextMeshProUGUI _warning2;
    private string _name;
    private StudentState _studentState;
    private void OnEnable()
    {
        StringManager.OnLanguageChanged += Refresh;
    }
    private void OnDisable()
    {
        StringManager.OnLanguageChanged -=Refresh;
    }
    private void Refresh()
    {
        Init(_name);
    }
    public void Init(string name) //개인훈련용
    {
        _name = name;
        StringManager manager = StringManager.Instance;
        _warning1.text = name +  " ("+ manager.GetString("UI_Development_컨디션")+" : 0"+")";
        _warning2.text = manager.GetString("UI_Development_컨디션부족");
        manager.ApplyFont(_warning1);
        manager.ApplyFont(_warning2);
    }

    public void Init(string name, StudentState state) //팀훈련용
    {
        _name = name;
        _studentState = state;
        StringManager manager = StringManager.Instance;
        string stateWord = manager.GetString(GetStateString(state));

        _warning1.text = name + $"({stateWord})";        
        _warning2.text = manager.GetString("UI_Development_팀훈련부족팝업").Replace("{}", stateWord);
        manager.ApplyFont(_warning1);
        manager.ApplyFont(_warning2);
    }

    private string GetStateString(StudentState state)
    {
        if (state == StudentState.Injured) return "UI_Player_부상";
        else if (state == StudentState.OverWorked) return "UI_Player_과로";
        else return ("선수 상태가 정상입니다. 팝업 창 표시 로직을 점검해주세요");
    }
}
