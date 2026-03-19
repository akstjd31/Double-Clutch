using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PassiveCheatUI : MonoBehaviour
{
    private Student _student;
    [SerializeField] private TMP_Dropdown _dropdown;
    [SerializeField] private GameObject _passiveObj;
    private TextMeshProUGUI[] _myPassives;


    private void OnEnable()
    {
        if (_dropdown == null) return;
        if (_student == null) return;
        if (_passiveObj == null) return;

        if (_myPassives == null)
        {
            _myPassives = _passiveObj.GetComponentsInChildren<TextMeshProUGUI>();
        }
        
        _dropdown.options.Clear();

        // for (int i = 0; i < _student.Passive.Count; i++)
        // {
        //     var option = new TMP_Dropdown.OptionData();
        //     option.text = StringManager.Instance.GetString(_student.Passive[i].skillName);
        //     _dropdown.options.Add(option);
        // }

        _dropdown.RefreshShownValue();
    }

    public void SetStudent(Student std) => _student = std;
}
