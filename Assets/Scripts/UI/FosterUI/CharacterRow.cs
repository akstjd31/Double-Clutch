using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

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
    [SerializeField] RectTransform _attackBox;
    [SerializeField] RectTransform _defenceBox;

    private Student _target;
    private void OnEnable()
    {
        StringManager.OnLanguageChanged += Refresh;
    }
    private void OnDisable()
    {
        StringManager.OnLanguageChanged -= Refresh;
    }

    public void Init(Student target)
    {
        _target = target;
        Refresh();
    }

    private string GetStateString(StudentState state)
    {
        if (state == StudentState.Injured) return "UI_Player_부상";
        else if (state == StudentState.OverWorked) return "UI_Player_과로";
        else return ("-");
    }

    private void SetTag(Student target)
    {
        _attackTag1.text = string.Empty;
        _attackTag2.text = string.Empty;
        _guardTag1.text = string.Empty;
        _guardTag2.text = string.Empty;

        if (target.ChangedPotentials.Count == 0)
        {            
            return;
        }
            

        List<string> attackTags = new List<string>();
        List<string> guardTags = new List<string>();


        for (int i = 0; i < target.ChangedPotentials.Count; i++)
        {
            Training_MappingData data = FosterManager.Instance.GetTrainingMapping(target.ChangedPotentials[i]);
            
            switch(data.potential)
            {
                case potential.Stat2pt:
                case potential.Stat3pt:
                case potential.StatPass:
                    if (!attackTags.Contains(data.categorydescKey))
                    {
                        attackTags.Add(data.categorydescKey);
                    }
                    break;
                case potential.StatRebound:
                case potential.StatSteal:
                case potential.StatBlock:
                    if (!guardTags.Contains(data.categorydescKey))
                    {
                        guardTags.Add(data.categorydescKey);
                    }
                    break;
            }             
        }
        if (attackTags.Count > 0)
        {
            _attackTag1.text = StringManager.Instance.GetString(attackTags[0]);
            StringManager.Instance.ApplyFont(_attackTag1);
        }
        if (attackTags.Count > 1)
        {
            _attackTag2.text = StringManager.Instance.GetString(attackTags[1]);
            StringManager.Instance.ApplyFont(_attackTag2);
        }
        if (guardTags.Count > 0)
        {
            _guardTag1.text = StringManager.Instance.GetString(guardTags[0]);
            StringManager.Instance.ApplyFont(_guardTag1);
        }
        if (guardTags.Count > 1)
        {
            _guardTag2.text = StringManager.Instance.GetString(guardTags[1]);
            StringManager.Instance.ApplyFont(_guardTag2);
        }        
    }    
    private void Refresh()
    {
        StringManager manager = StringManager.Instance;
        _name.text = manager.GetString(_target.Name[0]) + manager.GetString(_target.Name[1]) + manager.GetString(_target.Name[2]);
        if(manager.CurrentLanguage == Language.Ko)
        {
            _name.fontSize = 32;
        }
        if (manager.CurrentLanguage == Language.En)
        {
            _name.fontSize = 24;
        }
        if (manager.CurrentLanguage == Language.Ja)
        {
            _name.fontSize = 24;
        }

        manager.ApplyFont(_name);

        _attack.text = _target.AttackChange != 0 ? $"+{_target.AttackChange}" : "-";
        manager.ApplyFont(_attack);

        _guard.text = _target.DefenseChange != 0 ? $"+{_target.DefenseChange}" : "-";
        manager.ApplyFont(_guard);

        if (_target.ConditionChange > 0)
        {
            _condition.text = "+" + _target.ConditionChange.ToString();
        }
        else if (_target.ConditionChange == 0)
        {
            _condition.text = "-";
        }
        else
        {
            _condition.text =_target.ConditionChange.ToString();
        }
        _state.text = manager.GetString(GetStateString(_target.State));
        manager.ApplyFont(_state);
        manager.ApplyFont(_condition);
        SetTag(_target);
        LayoutRebuilder.ForceRebuildLayoutImmediate(_attackBox);
        LayoutRebuilder.ForceRebuildLayoutImmediate(_defenceBox);
    }
}
