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

    private List<Player_PassiveData> _passiveDataList;
    private List<Player_PassiveData> _selectSkillList = new List<Player_PassiveData>();
    private Player_PassiveData _selectSkill;

    public Player_PassiveData SelectSkill => _selectSkill;


    private void Awake()
    {
        for (int i = 0; i < 3; i++)
        {
            _skillName[i].text = $"";
            _skillDetail[i].text = $"";
        }
    }

    private void OnEnable()
    {
        for (int i = 0; i < 3; i++)
        {
            _buttons[i].interactable = true;
            _buttons[i].targetGraphic.color = _buttons[i].colors.normalColor;
            Debug.Log($"버튼 상태 초기화");
        }
    }

    private void Start()
    {
    }

    public void GetSkillList(Student student)
    {
        //패시브 스킬 리스트 가져오기
        _passiveDataList = student.GetAvailablePassives(_passiveDataReader.DataList);

        for (int i = 0; i < 3; i++)
        {
            int randomN = Random.Range(0, _passiveDataList.Count);
            _skillName[i].text = _passiveDataList[randomN].skillName;
            _skillDetail[i].text = _passiveDataList[randomN].passiveDesc;

            _selectSkillList.Add(_passiveDataList[randomN]);
            Debug.Log($"{_passiveDataList[randomN].skillName}추가");

            _passiveDataList.RemoveAt(randomN);
        }
    }


    public void OnClickSkillBox(Button button)
    {
        //각 버튼을 클릭했을 때 선택되었는지 아닌지 알아야 함.
        //클릭했을 때 스킬이 플레이어 패시브리스트에 들어가도록
        for (int i = 0; i < 3; i++)
        {
            _buttons[i].interactable = true;
            if (button == _buttons[i])
            {
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
