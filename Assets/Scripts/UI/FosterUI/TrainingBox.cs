using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TrainingBox : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _trainingName;
    [SerializeField] TextMeshProUGUI _trainingDesc;
    [SerializeField] TextMeshProUGUI _trainingcost;
    [SerializeField] Sprite _trainingButton;
    [SerializeField] Sprite _restButton;
    [SerializeField] Image _buttonImage;
    [SerializeField] Button _button;
    Student _target;
    ITraining _command;

    public ITraining Command => _command;
    public Student Target => _target;

    private void OnEnable()
    {
        StringManager.OnLanguageChanged += Refresh;
    }

    private void OnDisable()
    {
        StringManager.OnLanguageChanged -= Refresh;
    }

    public void Init(ITraining command)
    {
        _command = command;

        _button.onClick.RemoveAllListeners();
        _button.onClick.AddListener(command.IsTeam() ? (() => FosterManager.Instance.ReserveTeamTraining(_command)) : () => FosterManager.Instance.ReserveIndividualTraining(_command));
        _button.onClick.AddListener(() => StudentUIManager.Instance.OnTrainingBoxClick());

        _buttonImage.sprite = command is IndividualTraining || command is TeamTraining ? _trainingButton : _restButton;

        Refresh();
    }

    private void Refresh()
    {
        if (_command == null) return;
        StringManager.Instance.GetString(_command.GetNameKey(), _trainingName);
        StringManager.Instance.GetString(_command.GetDescKey(), _trainingDesc);
        StringManager.Instance.GetString(_command.GetCost().ToString() + "G", _trainingcost);
    }

    public void SetStudent(Student target)
    {
        _target = target;
        _command.SetTarget(_target);
    }

    private void OnDestroy()
    {
        _button?.onClick?.RemoveAllListeners();
    }
}
