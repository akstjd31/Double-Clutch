using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PassiveBox : MonoBehaviour
{
    [SerializeField] private GraduationManager _graduationManager;
    [SerializeField] private Transform _parent;

    [SerializeField] private TextMeshProUGUI _skillName;
    [SerializeField] private Image _skillImage;
    [SerializeField] private TextMeshProUGUI _skillDetail;

    private Button[] _buttons = new Button[3];

    //private List<Player_PassiveData> _passiveDataList; //패시브 스킬
    //private Player_PassiveData _selectedSkill; 
    private Button _button; 


    private void Awake()
    {
        _skillName.text = $"";
        _skillDetail.text = $"";
        _button = GetComponent<Button>();
        _buttons = _parent.GetComponentsInChildren<Button>();
    }

    private void OnEnable()
    {
        for (int i = 0; i < _buttons.Length; i++)
        {
            _buttons[i].interactable = true;
                _buttons[i].targetGraphic.color = _buttons[i].colors.normalColor;
                Debug.Log($"버튼 상태 초기화");
            }
        }

    private void Start()
    {
        //패시브 스킬 리스트 가져오기
        //_passiveDataList = _graduationManager.PromotionStudentList[_graduationManager.Turn].Passive;

        //System.Random random = new System.Random();
        
        //while(true)
        //{
        //    int skillN = random.Next(0, _passiveDataList.Count);

        //    if(skillN)
        //}

        //스킬 랜덤으로 띄워주는건가
        _skillName.text = $"스킬{transform.GetSiblingIndex()}";
        _skillDetail.text = $"스킬설명{transform.GetSiblingIndex()}";
    }

    public void OnClickSkillBox()
    {
        Button clickButton = _button;

        //각 버튼을 클릭했을 때 선택되었는지 아닌지 알아야 함.
        //클릭했을 때 스킬이 플레이어 패시브리스트에 들어가도록
        for (int i = 0; i < _buttons.Length; i++)
        {
            _buttons[i].interactable = true;
            if (clickButton == _buttons[i])
            {
                _buttons[i].targetGraphic.color = _buttons[i].colors.normalColor;
                Debug.Log($"{gameObject.name} 버튼 활성화 색상");
            }
            else
            {
                _buttons[i].targetGraphic.color = _buttons[i].colors.disabledColor;
                Debug.Log($"{gameObject.name} 버튼 비활성화 색상");
            }
        }

        _graduationManager.PromotionPanel.IsSkillChoise = true;
        Debug.Log($"스킬 선택 상태{_graduationManager.PromotionPanel.IsSkillChoise}");
    }
}
