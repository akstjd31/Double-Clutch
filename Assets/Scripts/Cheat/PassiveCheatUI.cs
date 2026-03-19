using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PassiveCheatUI : MonoBehaviour
{
    private Student _student;
    [SerializeField] private TMP_Dropdown _dropdown;
    [SerializeField] private GameObject _passivesObj;
    private TextMeshProUGUI[] _myPassiveTexts;
    [SerializeField] List<string> skillNameData = new List<string>();
    HashSet<string> nameData = new HashSet<string>();

    public void Init(Student std)
    {
        _student = std;

        if (_myPassiveTexts == null)
        {
            _myPassiveTexts = _passivesObj.GetComponentsInChildren<TextMeshProUGUI>();
        }

        foreach (var t in _myPassiveTexts)
            t.text = "";
        

        for (int i = 0; i < _student.Passive.Count; i++)
        {
            _myPassiveTexts[i].text = StringManager.Instance.GetString(_student.Passive[i].skillName);
        }

        if (_dropdown == null) return;
        if (_student == null) return;
        if (_passivesObj == null) return;

        // 드롭다운 코드 작성 (모든 패시브 가져와서 선택하게끔)
        _dropdown.options.Clear();

        if (StudentManager.Instance == null) return;
        var passiveData = StudentManager.Instance.GetFactory().GetPassiveDataList();
        
        if (skillNameData == null)
        {
            for (int i = 0; i < passiveData.Count; i++)
            {
                skillNameData.Add(passiveData[i].skillName);
            }
        }

        if (nameData == null)
        {
            for (int i = 0; i < skillNameData.Count; i++)
            {
                var passiveName = StringManager.Instance.GetString(skillNameData[i]);
                nameData.Add(passiveName);
            }
        }

        foreach (var n in nameData)
        {
            var option = new TMP_Dropdown.OptionData();
            option.text = n;
            _dropdown.options.Add(option);
        }

        _dropdown.RefreshShownValue();
    }
}
