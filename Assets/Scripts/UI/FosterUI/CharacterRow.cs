using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 개별 결과창 CharacterRow 프리팹에 할당할 스크립트
/// </summary>
public class CharacterRow : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _name;
    [SerializeField] TextMeshProUGUI _attack;
    [SerializeField] TextMeshProUGUI _attackTag1;
    [SerializeField] TextMeshProUGUI _attackTag2;
    [SerializeField] TextMeshProUGUI _guard;
    [SerializeField] TextMeshProUGUI _guardTag1;
    [SerializeField] TextMeshProUGUI _guardTag2;
    [SerializeField] TextMeshProUGUI _condition;
    [SerializeField] TextMeshProUGUI _state;

    public void Init(Student target)
    {
        StringManager manager = StringManager.Instance;
        _name.text = manager.GetString(target.Name[0]) + manager.GetString(target.Name[1]) + manager.GetString(target.Name[2]);
        
        _attack.text = target.AttackChange != 0 ? $"+{target.AttackChange}" : "-";
        _guard.text = target.DefenseChange != 0 ? $"+{target.DefenseChange}" : "-";
        _condition.text = target.ConditionChange.ToString();
        _state.text = GetStateString(target.State);
    }

    private string GetStateString(StudentState state)
    {
        if (state == StudentState.Injured) return "부상";
        else if (state == StudentState.OverWorked) return "과로";
        else return ("-");
    }

    private void SetTag()
    {

    }    
}
