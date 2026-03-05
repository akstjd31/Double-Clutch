using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PassiveBox : MonoBehaviour
{
    [SerializeField] private GraduationManager _graduationManager;

    [SerializeField] private TextMeshProUGUI[] _skillName = new TextMeshProUGUI[3];
    [SerializeField] private Image[] _skillImage = new Image[3];
    [SerializeField] private TextMeshProUGUI[] _skillDetail = new TextMeshProUGUI[3];
    [SerializeField] private Button[] _buttons = new Button[3];

    [SerializeField] private Player_PassiveDataReader _passiveDataReader;

    //가져올 스킬 리스트
    private List<Player_PassiveData> _passiveDataList;

    //랜덤으로 띄울 스킬 목록
    private List<Player_PassiveData> _selectSkillList = new List<Player_PassiveData>();

    private Dictionary<int, List<Player_PassiveData>> _selectSkillSave = new Dictionary<int, List<Player_PassiveData>>();

    //선택한 스킬
    private Player_PassiveData _selectSkill;

    public Player_PassiveData SelectSkill => _selectSkill;
    public Dictionary<int, List<Player_PassiveData>> SelectSkillSave => _selectSkillSave;


    private void Awake()
    {
        for (int i = 0; i < 3; i++)
        {
            _skillName[i].text = $"";
            _skillDetail[i].text = $"";
        }
    }

    //private void OnEnable()
    //{
    //    Debug.Log($"버튼 상태 초기화");
    //    for (int i = 0; i < 3; i++)
    //    {
    //        _buttons[i].interactable = true;
    //        _buttons[i].targetGraphic.color = _buttons[i].colors.normalColor;
    //    }
    //}

    private void ButtonInit()
    {
        for (int i = 0; i < 3; i++)
        {
            Debug.Log($"버튼 : {_selectSkillList.Count}개");
            if (i < _selectSkillList.Count)
            {
                _buttons[i].interactable = true;
                _buttons[i].targetGraphic.color = _buttons[i].colors.normalColor;
                Debug.Log($"{i + 1}버튼 활성화");

            }
            else
            {
                _skillName[i].text = "";
                _skillDetail[i].text = "";
                _buttons[i].interactable = false;
                _buttons[i].targetGraphic.color = _buttons[i].colors.disabledColor;
                Debug.Log($"{i + 1}버튼 비활성화");
            }
        }
    }

    public void GetSkillList(Student student)
    {
        //담아놓은 스킬이 있다면
        if (_selectSkillSave.TryGetValue(_graduationManager.PromotionStudentList[_graduationManager.Turn], out var list))
        {
            _selectSkillList = new List<Player_PassiveData>(list);
            for (int i = 0; i < _selectSkillList.Count; i++)
            {
                _skillName[i].text = _selectSkillList[i].skillName;
                _skillDetail[i].text = _selectSkillList[i].passiveDesc;
            }
        }
        else
        {
            _selectSkillList.Clear();

            //패시브 스킬 리스트 가져오기
            _passiveDataList = new List<Player_PassiveData>(student.GetAvailablePassives(_passiveDataReader.DataList));

            for (int i = 0; i < 3; i++)
            {
                if (_passiveDataList.Count == 0)
                {
                    Debug.Log($"새로 가질 수 있는 스킬 : {_passiveDataList.Count}개");
                    ButtonInit();
                    return;
                }

                int randomN = Random.Range(0, _passiveDataList.Count);

                _skillName[i].text = _passiveDataList[randomN].skillName;
                _skillDetail[i].text = _passiveDataList[randomN].passiveDesc;

                _selectSkillList.Add(_passiveDataList[randomN]);
                Debug.Log($"{_passiveDataList[randomN].skillName}추가");

                _passiveDataList.RemoveAt(randomN);
            }
            _selectSkillSave[_graduationManager.PromotionStudentList[_graduationManager.Turn]] = new List<Player_PassiveData>(_selectSkillList);
        }
        ButtonInit();
    }


    public void OnClickSkillBox(Button button)
    {
        //각 버튼을 클릭했을 때 선택되었는지 아닌지 알아야 함.
        //클릭했을 때 스킬이 플레이어 패시브리스트에 들어가도록
        for (int i = 0; i < 3; i++)
        {
            if (button == _buttons[i])
            {
                _buttons[i].interactable = true;
                _buttons[i].targetGraphic.color = _buttons[i].colors.normalColor;
                //선택한 스킬 저장
                _selectSkill = _selectSkillList[i];
                Debug.Log($"{_selectSkill.skillName}");
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
