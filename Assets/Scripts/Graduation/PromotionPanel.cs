using System.Collections.Generic;
using Game.Constants;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PromotionPanel : MonoBehaviour
{
    [SerializeField] private GraduationManager _graduationManager;
    [SerializeField] private GameObject _passiveSkillSelectPanel;
    [SerializeField] private PassiveBox _passiveBox;

    [SerializeField] private GameObject _beforeGuideBox;
    [SerializeField] private GameObject _afterChoice;

    [SerializeField] private TextMeshProUGUI _guideBoxName;

    [SerializeField] private TextMeshProUGUI _name;
    [SerializeField] private TextMeshProUGUI _gradeUp;
    [SerializeField] private Image _image;

    [SerializeField] private TextMeshProUGUI[] _passiveNameText = new TextMeshProUGUI[3];

    private List<int> _promotionStudentList;

    private bool _isSkillChoise = false;
    Student _currentStudent;
    string _getPromotionName;
    bool _needGuideRefresh = false;

    public bool IsSkillChoise { get { return _isSkillChoise; } set { _isSkillChoise = value; } }

    private void Start()
    {
        UpdateProfile();
        _passiveBox.GetSkillList(_currentStudent);
    }

    private void LateUpdate()
    {
        if (_needGuideRefresh)
        {
            _guideBoxName.text = _getPromotionName;
            _needGuideRefresh = false;
        }
    }

    public void GetList()
    {
        _promotionStudentList = _graduationManager.PromotionStudentList;
    }

    public void UpdateProfile()
    {
        StringManager manager = StringManager.Instance;
        

        if (_graduationManager.Turn >= _promotionStudentList.Count)
        {
            return;
        }

        int studentNum = _promotionStudentList[_graduationManager.Turn];

        for (int i = 0; i < _graduationManager.MyStudents.Count; i++)
        {
            if(studentNum == _graduationManager.MyStudents[i].StudentId)
            {
                _currentStudent = _graduationManager.MyStudents[i];
            }
        }
        string name = manager.GetString(_currentStudent.Name[0]) + manager.GetString(_currentStudent.Name[1]) + manager.GetString(_currentStudent.Name[2]);
        Debug.Log($"순서: {name} 학생");

        if (_isSkillChoise == false)
        {
            _getPromotionName = StringManager.Instance.GetString("UI_Promotion_진급팝업");
            var keys = TextParser.GetKeys(_getPromotionName);
            _getPromotionName = _getPromotionName.Replace("{" + keys[0] +"}", name);
            _guideBoxName.text = _getPromotionName;
            _needGuideRefresh = true;
        }
        else if (_isSkillChoise == true)
        {
            _afterChoice.SetActive(true);
        }

        _name.text = name;
        _image.sprite = SpriteManager.Instance.GetSprite(_currentStudent.VisualData.playerImageResource);
        string gradeText = StringManager.Instance.GetString("UI_Promotion_진급");
        var key =  TextParser.GetKeys(gradeText);
        _gradeUp.text = gradeText.Replace("{"+key[0]+"}", (_currentStudent.Grade - 1).ToString()).Replace("{" + key[1] + "}", (_currentStudent.Grade).ToString());
        StringManager.Instance.ApplyFont(_gradeUp);

        for (int i = 0; i < 3; i++)
        {
            if(i < _currentStudent.PassiveId.Count)
            {
                _passiveNameText[i].text = StringManager.Instance.GetString(_currentStudent.Passive[i].skillName);
                StringManager.Instance.ApplyFont(_passiveNameText[i]);
            }
            else
            {
                _passiveNameText[i].text = "";
            }
        }

        _isSkillChoise = false;
        Debug.Log($"스킬 선택 상태{_isSkillChoise}/{IsSkillChoise}");
    }

    public void OnClickNextButton()
    {
        PlayConfirmSound();
        if (_promotionStudentList.Count == 0)
        {
            Debug.Log("진급 학생 없음");
            _graduationManager.NextScene();
            return;
        }

        Debug.Log($"{_graduationManager.Turn}=={_promotionStudentList.Count}");
        if (_graduationManager.Turn == _promotionStudentList.Count)
        {
            Debug.Log("메인 씬으로 넘어가야 함.");
            _graduationManager.NextScene();
            return;
        }

        var student = _promotionStudentList[_graduationManager.Turn];
        _passiveSkillSelectPanel.SetActive(true);
        _passiveBox.GetSkillList(_currentStudent);
        //스킬 선택 상태 초기화
        _isSkillChoise = false;
        Debug.Log($"스킬 선택 상태{_isSkillChoise}/{IsSkillChoise}");

        Debug.Log($"남은 학생 수 {_graduationManager.Turn + 1}/{_promotionStudentList.Count}");
    }

    public void OnClickAfterChoice()
    {
        PlayConfirmSound();
        _afterChoice.SetActive(false);
    }

    public void OnClickNextStudent()
    {
        PlayConfirmSound();
        _afterChoice.SetActive(false);

        if (_graduationManager.Turn == _promotionStudentList.Count)
        {
            _beforeGuideBox.SetActive(false);
        }
        UpdateProfile();
    }

    private void PlayConfirmSound()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySoundOneShot(SoundName.SE_BUTTON_SELECT);
    }
}
