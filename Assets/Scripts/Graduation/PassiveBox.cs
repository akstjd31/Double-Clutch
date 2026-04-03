using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Game.Constants;

public class PassiveBox : MonoBehaviour
{
    [SerializeField] private GraduationManager _graduationManager;

    [SerializeField] private TextMeshProUGUI[] _skillName = new TextMeshProUGUI[3];
    [SerializeField] private Image[] _skillGradeImage = new Image[3];
    [SerializeField] private Image[] _skillImage = new Image[3];
    [SerializeField] private TextMeshProUGUI[] _skillDetail = new TextMeshProUGUI[3];
    [SerializeField] private Button[] _buttons = new Button[3];
    [SerializeField] private Outline[] _outlines = new Outline[3];

    [SerializeField] private Player_PassiveDataReader _passiveDataReader;
    [SerializeField] private Player_PassiveGradeDataReader _passiveGradeDataReader;

    private PassiveBoxSaveData _saveData;

    // 현재 학생이 가질 수 있는 전체 후보
    private List<Player_PassiveData> _passiveDataList = new List<Player_PassiveData>();

    // 현재 화면에 표시 중인 후보 3개
    private List<Player_PassiveData> _selectSkillList = new List<Player_PassiveData>();

    // skillId -> passive data lookup
    private Dictionary<string, Player_PassiveData> _passiveLookup = new Dictionary<string, Player_PassiveData>();

    // 현재 선택된 스킬
    private Player_PassiveData? _selectSkill;

    // 현재 표시 중인 학생 key
    private int _currentStudentKey = -1;

    private string[] _detailText = new string[3];
    private bool _needGuideRefresh = false;

    public Player_PassiveData? SelectSkill => _selectSkill;

    private void Awake()
    {
        InitUI();
        BuildPassiveLookup();
        LoadSaveData();
    }

    private void OnEnable()
    {
        StringManager.OnLanguageChanged += RefreshUI;
    }

    private void OnDisable()
    {
        StringManager.OnLanguageChanged -= RefreshUI;
    }

    private void LateUpdate()
    {
        if (!_needGuideRefresh) return;

        for (int i = 0; i < _selectSkillList.Count && i < _skillDetail.Length; i++)
        {
            _skillDetail[i].text = _detailText[i];
            StringManager.Instance.ApplyFont(_skillDetail[i]);
        }

        _needGuideRefresh = false;
    }

    private void InitUI()
    {
        for (int i = 0; i < 3; i++)
        {
            _skillName[i].text = "";
            _skillDetail[i].text = "";
            _detailText[i] = "";
            _skillGradeImage[i].sprite = null;
            _skillImage[i].sprite = null;
            _outlines[i].enabled = false;
        }
    }

    private void LoadSaveData()
    {
        if (!SaveLoadManager.Instance.TryLoad(FilePath.PASSIVE_BOX_SAVE_PATH, out _saveData) || _saveData == null)
        {
            _saveData = new PassiveBoxSaveData();
        }

        if (_saveData.entries == null)
            _saveData.entries = new List<PassiveBoxSkillEntry>();
    }

    private void BuildPassiveLookup()
    {
        _passiveLookup.Clear();

        if (_passiveDataReader == null || _passiveDataReader.DataList == null)
            return;

        foreach (var passive in _passiveDataReader.DataList)
        {
            if (string.IsNullOrEmpty(passive.skillId)) continue;

            if (!_passiveLookup.ContainsKey(passive.skillId))
                _passiveLookup.Add(passive.skillId, passive);
        }
    }

    private PassiveBoxSkillEntry GetEntry(int studentKey)
    {
        if (_saveData == null || _saveData.entries == null)
            return null;

        return _saveData.entries.Find(x => x.studentKey == studentKey);
    }

    private PassiveBoxSkillEntry GetOrCreateEntry(int studentKey)
    {
        var entry = GetEntry(studentKey);
        if (entry != null) return entry;

        entry = new PassiveBoxSkillEntry
        {
            studentKey = studentKey,
            passiveIds = new List<string>(),
            selectedPassiveId = string.Empty
        };

        _saveData.entries.Add(entry);
        return entry;
    }

    private void SaveSkillRoll(int studentKey, List<Player_PassiveData> skillList)
    {
        if (_saveData == null) return;

        var entry = GetOrCreateEntry(studentKey);

        if (entry.passiveIds == null)
            entry.passiveIds = new List<string>();
        else
            entry.passiveIds.Clear();

        if (skillList != null)
        {
            foreach (var skill in skillList)
            {
                if (string.IsNullOrEmpty(skill.skillId)) continue;

                entry.passiveIds.Add(skill.skillId);
            }
        }

        SaveLoadManager.Instance.Save(FilePath.PASSIVE_BOX_SAVE_PATH, _saveData);
    }

    private void SaveSelectedSkill(int studentKey, Player_PassiveData selectedSkill)
    {
        if (_saveData == null) return;

        var entry = GetOrCreateEntry(studentKey);
        entry.selectedPassiveId = selectedSkill.skillId;

        SaveLoadManager.Instance.Save(FilePath.PASSIVE_BOX_SAVE_PATH, _saveData);
    }

    private List<Player_PassiveData> LoadSavedSkillList(int studentKey)
    {
        var result = new List<Player_PassiveData>();

        var entry = GetEntry(studentKey);
        if (entry == null || entry.passiveIds == null || entry.passiveIds.Count == 0)
            return result;

        foreach (var passiveId in entry.passiveIds)
        {
            if (string.IsNullOrEmpty(passiveId)) continue;

            if (_passiveLookup.TryGetValue(passiveId, out var passive))
                result.Add(passive);
        }

        return result;
    }

    private Player_PassiveData? LoadSavedSelectedSkill(int studentKey)
    {
        var entry = GetEntry(studentKey);
        if (entry == null || string.IsNullOrEmpty(entry.selectedPassiveId))
            return null;

        if (_passiveLookup.TryGetValue(entry.selectedPassiveId, out var passive))
            return passive;

        return null;
    }

    private void ClearSlot(int index)
    {
        _skillName[index].text = "";
        _skillDetail[index].text = "";
        _detailText[index] = "";
        _skillGradeImage[index].sprite = null;
        _skillImage[index].sprite = null;

        _buttons[index].interactable = false;
        _buttons[index].targetGraphic.color = _buttons[index].colors.disabledColor;

        _buttons[index].GetComponent<Image>().color = Color.white;
        _skillName[index].color = Color.white;
        _skillGradeImage[index].color = Color.white;
        _skillImage[index].color = Color.white;
        _outlines[index].enabled = false;
    }

    private void ResetAllSlots()
    {
        for (int i = 0; i < 3; i++)
        {
            ClearSlot(i);
        }
    }

    private void ButtonInit()
    {
        for (int i = 0; i < 3; i++)
        {
            if (i < _selectSkillList.Count)
            {
                _buttons[i].interactable = true;
                _buttons[i].targetGraphic.color = _buttons[i].colors.normalColor;

                _buttons[i].GetComponent<Image>().color = Color.white;
                _skillName[i].color = Color.white;
                _skillGradeImage[i].color = Color.white;
                _skillImage[i].color = Color.white;
                _outlines[i].enabled = false;
            }
            else
            {
                ClearSlot(i);
            }
        }
    }

    private string GetPassiveFrameResourceId(int grade)
    {
        if (_passiveGradeDataReader == null || _passiveGradeDataReader.DataList == null)
            return null;

        foreach (var data in _passiveGradeDataReader.DataList)
        {
            if (data.gradeId == grade)
                return data.passiveFrameResource;
        }

        return null;
    }

    private void ApplySkillListToUI(List<Player_PassiveData> skillList)
    {
        _selectSkillList = skillList ?? new List<Player_PassiveData>();

        for (int i = 0; i < 3; i++)
        {
            if (i >= _selectSkillList.Count)
            {
                ClearSlot(i);
                continue;
            }

            var skill = _selectSkillList[i];

            _skillName[i].text = StringManager.Instance.GetString(skill.skillName);
            StringManager.Instance.ApplyFont(_skillName[i]);

            _detailText[i] = StringManager.Instance.GetString(skill.passiveDesc);
            _detailText[i] = _detailText[i].Replace("{effectValue}", GetValueText(skill));

            _skillDetail[i].text = _detailText[i];
            StringManager.Instance.ApplyFont(_skillDetail[i]);

            _skillImage[i].sprite = SpriteManager.Instance.GetSprite(skill.passiveResource);

            string frameResourceId = GetPassiveFrameResourceId(skill.grade);
            _skillGradeImage[i].sprite = string.IsNullOrEmpty(frameResourceId)
                ? null
                : SpriteManager.Instance.GetSprite(frameResourceId);
        }

        ButtonInit();
        _needGuideRefresh = true;
    }

    private void ApplySelectedVisual(Player_PassiveData selectedSkill)
    {
        for (int i = 0; i < 3; i++)
        {
            if (i >= _selectSkillList.Count)
            {
                _outlines[i].enabled = false;
                continue;
            }

            bool isSelected = _selectSkillList[i].skillId == selectedSkill.skillId;

            if (isSelected)
            {
                _buttons[i].GetComponent<Image>().color = new Color(1f, 1f, 1f);
                _skillName[i].color = new Color(1f, 1f, 1f);
                _skillGradeImage[i].color = new Color(1f, 1f, 1f);
                _skillImage[i].color = new Color(1f, 1f, 1f);
                _outlines[i].enabled = true;
            }
            else
            {
                _buttons[i].GetComponent<Image>().color = new Color(0.4f, 0.4f, 0.4f);
                _skillName[i].color = new Color(0.4f, 0.4f, 0.4f);
                _skillGradeImage[i].color = new Color(0.4f, 0.4f, 0.4f);
                _skillImage[i].color = new Color(0.4f, 0.4f, 0.4f);
                _outlines[i].enabled = false;
            }
        }
    }

    public void ClearSavedSkill()
    {
        // 메모리 데이터 초기화
        if (_saveData != null && _saveData.entries != null)
        {
            _saveData.entries.Clear();
        }

        // 파일 자체를 초기화 (빈 데이터로 덮어쓰기)
        _saveData = new PassiveBoxSaveData();
        SaveLoadManager.Instance.Save(FilePath.PASSIVE_BOX_SAVE_PATH, _saveData);

        // 현재 UI 상태 초기화
        _currentStudentKey = -1;
        _selectSkill = null;
        _selectSkillList.Clear();
        ResetAllSlots();

        if (_graduationManager != null && _graduationManager.PromotionPanel != null)
        {
            _graduationManager.PromotionPanel.IsSkillChoise = false;
        }
    }

    public void GetSkillList(Student student, int studentKey)
    {
        if (student == null) return;
        if (_passiveDataReader == null || _passiveDataReader.DataList == null) return;

        _currentStudentKey = studentKey;

        var savedSkillList = LoadSavedSkillList(studentKey);

        // 이미 저장된 스킬이 있으면 그대로 사용
        if (savedSkillList.Count > 0)
        {
            ApplySkillListToUI(savedSkillList);

            _selectSkill = LoadSavedSelectedSkill(studentKey).Value;
            if (_selectSkill != null)
            {
                ApplySelectedVisual(_selectSkill.Value);
                _graduationManager.PromotionPanel.IsSkillChoise = true;
            }
            else
            {
                _graduationManager.PromotionPanel.IsSkillChoise = false;
            }

            return;
        }

        // 저장된 값이 없으면 새로 뽑기
        _selectSkill = null;
        _graduationManager.PromotionPanel.IsSkillChoise = false;
        _selectSkillList.Clear();
        ResetAllSlots();

        _passiveDataList = new List<Player_PassiveData>(student.GetAvailablePassives(_passiveDataReader.DataList));

        Dictionary<int, List<Player_PassiveData>> gradePool = new Dictionary<int, List<Player_PassiveData>>();
        foreach (var passive in _passiveDataList)
        {
            if (!gradePool.ContainsKey(passive.grade))
                gradePool[passive.grade] = new List<Player_PassiveData>();

            gradePool[passive.grade].Add(passive);
        }

        for (int i = 0; i < 3; i++)
        {
            if (_passiveDataList.Count == 0)
                break;

            int weightedRandom = GetWeightedRandomGrade();
            Player_PassiveData selectedSkill;

            if (gradePool.ContainsKey(weightedRandom) && gradePool[weightedRandom].Count > 0)
            {
                int randomIndex = Random.Range(0, gradePool[weightedRandom].Count);
                selectedSkill = gradePool[weightedRandom][randomIndex];
            }
            else
            {
                int randomIndex = Random.Range(0, _passiveDataList.Count);
                selectedSkill = _passiveDataList[randomIndex];
            }

            _selectSkillList.Add(selectedSkill);

            effectType currentType = selectedSkill.effectType;

            _passiveDataList.RemoveAll(p => p.effectType == currentType);

            foreach (var key in gradePool.Keys)
            {
                gradePool[key].RemoveAll(p => p.effectType == currentType);
            }
        }

        ApplySkillListToUI(new List<Player_PassiveData>(_selectSkillList));
        SaveSkillRoll(studentKey, _selectSkillList);
    }

    private int GetWeightedRandomGrade()
    {
        float randomPoint = Random.value;
        float cumulative = 0f;
        var gradeData = _passiveGradeDataReader.DataList;

        for (int i = 0; i < gradeData.Count; i++)
        {
            cumulative += gradeData[i].spawnRate;
            if (randomPoint <= cumulative)
                return gradeData[i].gradeId;
        }

        return 1;
    }

    public void OnClickSkillBox(Button button)
    {
        if (_currentStudentKey < 0) return;

        for (int i = 0; i < 3; i++)
        {
            if (i >= _selectSkillList.Count)
                continue;

            if (button == _buttons[i])
            {
                _buttons[i].GetComponent<Image>().color = new Color(1f, 1f, 1f);
                _skillName[i].color = new Color(1f, 1f, 1f);
                _skillGradeImage[i].color = new Color(1f, 1f, 1f);
                _skillImage[i].color = new Color(1f, 1f, 1f);
                _outlines[i].enabled = true;

                _selectSkill = _selectSkillList[i];
                SaveSelectedSkill(_currentStudentKey, _selectSkill.Value);
            }
            else
            {
                _buttons[i].GetComponent<Image>().color = new Color(0.4f, 0.4f, 0.4f);
                _skillName[i].color = new Color(0.4f, 0.4f, 0.4f);
                _skillGradeImage[i].color = new Color(0.4f, 0.4f, 0.4f);
                _skillImage[i].color = new Color(0.4f, 0.4f, 0.4f);
                _outlines[i].enabled = false;
            }
        }

        _graduationManager.PromotionPanel.IsSkillChoise = true;
    }

    private void RefreshUI()
    {
        if (_currentStudentKey < 0) return;

        var savedSkillList = LoadSavedSkillList(_currentStudentKey);
        if (savedSkillList.Count == 0) return;

        ApplySkillListToUI(savedSkillList);

        _selectSkill = LoadSavedSelectedSkill(_currentStudentKey);
        if (_selectSkill != null)
        {
            ApplySelectedVisual(_selectSkill.Value);
            _graduationManager.PromotionPanel.IsSkillChoise = true;
        }
        else
        {
            _graduationManager.PromotionPanel.IsSkillChoise = false;
        }
    }

    private string GetValueText(Player_PassiveData? data)
    {
        if (!data.HasValue) return string.Empty;

        switch (data.Value.effectType)
        {
            case effectType.None:
                return string.Empty;

            case effectType.Rate2pt:
            case effectType.Rate3pt:
            case effectType.RateBlock:
            case effectType.RatePass:
            case effectType.RateSteal:
            case effectType.RateRebound:
            case effectType.MonthGoldUp:
            case effectType.MatchGoldUp:
            case effectType.ReputationUp:
                return (data.Value.effectValue * 100).ToString();

            default:
                return data.Value.effectValue.ToString();
        }
    }
}