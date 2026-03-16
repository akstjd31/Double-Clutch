using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TrainingCharacterBox : MonoBehaviour
{
    [SerializeField] Button _button;
    [SerializeField] Image _stateBackGround;
    [SerializeField] TextMeshProUGUI _nameText;
    [SerializeField] TextMeshProUGUI _stateText;

    Student _student;

    public Button GetSelectButton() => _button;

    private void OnEnable()
    {
        StringManager.OnLanguageChanged += SetStudentState;
    }

    private void OnDisable()
    {
        StringManager.OnLanguageChanged -= SetStudentState;
    }

    public void Init(Student student)
    {
        _student = student;

        StringManager manager = StringManager.Instance;
        string name = manager.GetString(_student.Name[0]) + manager.GetString(_student.Name[1]) + manager.GetString(_student.Name[2]);

        _button.onClick.RemoveAllListeners();
        _button.onClick.AddListener(() => StudentUIManager.Instance.OnTrainingCharacterBoxClick(_student));
        _nameText.text = name;
        manager.ApplyFont(_nameText);
        SetStudentState();
    }

    public void SetStudentState()
    {
        if (_student == null) return;
        var cv = _stateBackGround.GetComponent<CanvasGroup>();
        cv.alpha = 0f;
        _stateText.text = "";
        if (_student.CurrentTraining != null)
        {
            cv.alpha = 1f;
            _stateBackGround.color = Color.green;
            StringManager.Instance.GetString(_student.CurrentTraining.GetNameKey(), _stateText);
            return;
        }
        if (_student.State == StudentState.OverWorked)
        {
            cv.alpha = 1f;
            _stateText.text = "과로";
            _stateBackGround.color = Color.yellow;
            return;
        }
        if (_student.State == StudentState.Injured)
        {
            cv.alpha = 1f;
            _stateBackGround.color = Color.red;
            _stateText.text = "부상";
            return;
        }
    }
}
