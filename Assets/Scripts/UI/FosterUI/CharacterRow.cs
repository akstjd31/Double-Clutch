using System.Collections.Generic;
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
        if (target.ConditionChange > 0)
        {
            _condition.text = "+"+target.ConditionChange.ToString();
        }
        else
        {
            _condition.text = target.ConditionChange.ToString();
        }            
        _state.text = GetStateString(target.State);
        SetTag(target);
    }

    private string GetStateString(StudentState state)
    {
        if (state == StudentState.Injured) return "부상";
        else if (state == StudentState.OverWorked) return "과로";
        else return ("-");
    }

    private void SetTag(Student target)
    {
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
        }
        if (attackTags.Count > 1)
        {
            _attackTag2.text = StringManager.Instance.GetString(attackTags[1]);
        }
        if (guardTags.Count > 0)
        {
            _guardTag1.text = StringManager.Instance.GetString(guardTags[0]);
        }
        if (guardTags.Count > 1)
        {
            _guardTag2.text = StringManager.Instance.GetString(guardTags[1]);
        }        
    }    
}
