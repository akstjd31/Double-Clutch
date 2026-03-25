using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TraitCheatUI : MonoBehaviour
{
    private Student _student;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI _currentTraitText; // 현재 특성 표시 텍스트
    [SerializeField] private TMP_Dropdown _dropdown;           // 특성 선택 드롭다운
    [SerializeField] private Button _applyButton;            // 변경 확정 버튼

    private List<Player_TraitData> _allTraitDataList = new List<Player_TraitData>();

    public void Init(Student std)
    {
        if (std == null) return;
        _student = std;

        // 1. 적용 버튼 리스너 등록
        _applyButton.onClick.RemoveAllListeners();
        _applyButton.onClick.AddListener(OnClickApplyTrait);

        // 2. 드롭다운 데이터 로드 (전체 특성 목록)
        SetupDropdown();

        // 3. 현재 정보 갱신
        RefreshUI();
    }

    private void SetupDropdown()
    {
        _dropdown.options.Clear();

        
        _allTraitDataList = StudentManager.Instance.GetFactory().GetTraitDataList();

        foreach (var data in _allTraitDataList)
        {
            var option = new TMP_Dropdown.OptionData();
        
            option.text = StringManager.Instance.GetString(data.traitName);
            _dropdown.options.Add(option);
        }

        _dropdown.RefreshShownValue();
    }

    private void OnClickApplyTrait()
    {
        if (_student == null || _dropdown.value < 0) return;

        // 드롭다운에서 선택된 특성 데이터 가져오기
        Player_TraitData selectedData = _allTraitDataList[_dropdown.value];

        // 학생의 특성 데이터 직접 교체
        _student.SetTrait(selectedData);

        // 변경 후 UI 갱신
        RefreshUI();

        Debug.Log($"[Cheat] {_student.Name}의 특성이 {selectedData.traitName}로 변경되었습니다.");
    }

    private void RefreshUI()
    {
        if (_student == null || _student.TraitId == null)
        {
            _currentTraitText.text = "특성 없음";
            return;
        }

        // 현재 학생이 보유한 특성 이름을 텍스트에 표시
        _currentTraitText.text = $"현재 특성: {StringManager.Instance.GetString(_student.TraitData.traitName)}";
    }
}