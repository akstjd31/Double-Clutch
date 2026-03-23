using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PassiveCheatUI : MonoBehaviour
{
    private Student _student;
    [SerializeField] private TMP_Dropdown _dropdown;
    [SerializeField] private GameObject _passivesObj;
    [SerializeField] private Button _applyButton; // 패시브 변경 확정 버튼

    private Button[] _passiveSlotButtons;
    private TextMeshProUGUI[] _myPassiveTexts;
    private Outline[] _outlines;

    private int _selectedIndex = -1;
    private List<Player_PassiveData> _allPassiveDataList = new List<Player_PassiveData>();

    public void Init(Student std)
    {
        if (std == null) return;
        _student = std;

        // 1. 컴포넌트 캐싱 및 버튼 리스너 등록
        if (_passiveSlotButtons == null || _passiveSlotButtons.Length == 0)
        {
            _passiveSlotButtons = _passivesObj.GetComponentsInChildren<Button>();
            _myPassiveTexts = new TextMeshProUGUI[_passiveSlotButtons.Length];
            _outlines = new Outline[_passiveSlotButtons.Length];

            for (int i = 0; i < _passiveSlotButtons.Length; i++)
            {
                int index = i; // 클로저 문제 방지
                _myPassiveTexts[i] = _passiveSlotButtons[i].GetComponentInChildren<TextMeshProUGUI>();
                _outlines[i] = _passiveSlotButtons[i].GetComponent<Outline>();
                _outlines[i].enabled = false; // 초기엔 끔

                _passiveSlotButtons[i].onClick.RemoveAllListeners();
                _passiveSlotButtons[i].onClick.AddListener(() => OnSelectSlot(index));
            }
        }

        // 2. 변경 버튼 리스너 등록
        _applyButton.onClick.RemoveAllListeners();
        _applyButton.onClick.AddListener(OnClickApplyPassive);

        // 3. 드롭다운 데이터 로드 (전체 패시브)
        SetupDropdown();

        // 4. 현재 학생 데이터로 UI 갱신
        RefreshUI();
    }

    private void SetupDropdown()
    {
        _dropdown.options.Clear();
        _allPassiveDataList = StudentManager.Instance.GetFactory().GetPassiveDataList();

        foreach (var data in _allPassiveDataList)
        {
            var option = new TMP_Dropdown.OptionData();
            option.text = StringManager.Instance.GetString(data.skillName);
            _dropdown.options.Add(option);
        }
        _dropdown.RefreshShownValue();
    }

    private void OnSelectSlot(int index)
    {
        // 이전 아웃라인 끄기
        if (_selectedIndex != -1) _outlines[_selectedIndex].enabled = false;

        // 현재 선택한 슬롯 아웃라인 켜기
        _selectedIndex = index;
        _outlines[_selectedIndex].enabled = true;
    }

    private void OnClickApplyPassive()
    {
        if (_selectedIndex == -1 || _student == null) return;

        // 드롭다운에서 선택된 패시브 데이터 가져오기
        Player_PassiveData selectedData = _allPassiveDataList[_dropdown.value];

        // 학생 데이터 교체 (해당 인덱스의 패시브 변경)
        if (_selectedIndex < _student.Passive.Count)
        {
            _student.Passive[_selectedIndex] = selectedData;
        }     

        RefreshUI();
    }

    private void RefreshUI()
    {
        for (int i = 0; i < _passiveSlotButtons.Length; i++)
        {
            if (i < _student.Passive.Count)
            {
                _myPassiveTexts[i].text = StringManager.Instance.GetString(_student.Passive[i].skillName);
                _passiveSlotButtons[i].interactable = true;
            }
            else
            {
                _myPassiveTexts[i].text = "없음";                
                _passiveSlotButtons[i].interactable = true;
            }
        }
    }
}