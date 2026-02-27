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
    [SerializeField] TextMeshProUGUI _guard;
    [SerializeField] TextMeshProUGUI _condition;
    [SerializeField] TextMeshProUGUI _state;

    public void Init(Student target)
    {
        _name.text = target.Name;
        _attack.text = $"공격 : + {target.AttackChange}";
        _guard.text = $"공격 : + {target.DefenseChange}";
        _condition.text = target.Condition.ToString();
        _state.text = GetStateString(target.State);
    }

    private string GetStateString(StudentState state)
    {
        if (state == StudentState.Injured) return "부상";
        else if (state == StudentState.OverWorked) return "피로";
        else return ("건강");
    }
}
