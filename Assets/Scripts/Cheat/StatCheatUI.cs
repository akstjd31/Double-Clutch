using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatCheatUI : MonoBehaviour
{
    [SerializeField] private GameObject _studentListObj;
    [SerializeField] private Student _student;
    [SerializeField] private TMP_InputField[] _2ptFields;
    [SerializeField] private TMP_InputField[] _3ptFields;
    [SerializeField] private TMP_InputField _conditionField;
    [SerializeField] private Button _confirmButton;

    public void SetStudent(Student std)
    {
        _student = std;
    }

    private void OnEnable()
    {
        if (_confirmButton == null) return;

        Init();
        
        _confirmButton.onClick.AddListener(OnClickConfirmButton);
    }

    private void OnDisable()
    {
        if (_confirmButton != null)
        {
            _confirmButton.onClick.RemoveAllListeners();
        }
    }

    public void OnClickConfirmButton()
    {
        if (_student == null) return;
        if (_2ptFields == null || _2ptFields.Length < 2) return;
        if (_3ptFields == null || _3ptFields.Length < 2) return;
        if (_conditionField == null) return;

        if (string.IsNullOrWhiteSpace(_2ptFields[0].text) || string.IsNullOrWhiteSpace(_2ptFields[1].text)) return;
        
        int num1 = 0, num2 = 0;
        Stat stat = null;

        // 2점슛 현재 잠재력, 최대 잠재력 넣어주기
        if (!int.TryParse(_2ptFields[0].text, out num1) || !int.TryParse(_2ptFields[1].text, out num2)) return;
        stat = _student.GetStat(potential.Stat2pt);
        stat.SetCurrent(num1);
        stat.SetLimit(num2);

        // 3점슛 현재 잠재력, 최대 잠재력 넣어주기
        if (!int.TryParse(_3ptFields[0].text, out num1) || !int.TryParse(_3ptFields[1].text, out num2)) return;
        stat = _student.GetStat(potential.Stat3pt);
        stat.SetCurrent(num1);
        stat.SetLimit(num2);

        // 컨디션 넣어주기
        if (!int.TryParse(_conditionField.text, out num1)) return;
        _student.SetCondition(num1);

        Debug.Log("능력치 변경 완료!");

        if (_studentListObj == null) return;
        _studentListObj.SetActive(true);
        this.gameObject.SetActive(false);
    }

    private void Init()
    {
        if (_2ptFields == null) return;
        foreach (var field in _2ptFields)
        {
            field.text = "";
        }

        if (_3ptFields == null) return;
        foreach (var field in _3ptFields)
        {
            field.text = "";
        }

        if (_conditionField == null) return;
        _conditionField.text = "";
    }
}
