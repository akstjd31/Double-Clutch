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
        StringManager manager = StringManager.Instance;
        _name.text = manager.GetString(target.Name[0]) + manager.GetString(target.Name[1]) + manager.GetString(target.Name[2]);
        _attack.text = $"+{target.AttackChange}";
        _guard.text = $"+{target.DefenseChange}";
        _condition.text = target.Condition.ToString();
        _state.text = GetStateString(target.State);
    }

    private string GetStateString(StudentState state)
    {
        if (state == StudentState.Injured) return "부상";
        else if (state == StudentState.OverWorked) return "과로";
        else return ("정상");
    }
}
