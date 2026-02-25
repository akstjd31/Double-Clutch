using TMPro;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 개인 훈련 박스 프리팹에 할당할 스크립트
/// 클릭 시 FosterManager에 훈련 예약 
/// </summary>
public class TrainingBox : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _trainingName;
    [SerializeField] TextMeshProUGUI _trainingDesc;
    [SerializeField] TextMeshProUGUI _trainingcost;
    [SerializeField] Button _button;
    Student _target;
    ITraining _command;

    public ITraining Command => _command;
    public Student Target => _target;


    public void Init(ITraining command)
    {        
        _command = command;

        _button.onClick.RemoveAllListeners();
        _button.onClick.AddListener(command.IsTeam()? (() => FosterManager.Instance.ReserveTeamTraining(_command)): () => FosterManager.Instance.ReserveIndividualTraining(_command));

        _trainingName.text = StringManager.Instance.GetString(command.GetNameKey());
        _trainingDesc.text = StringManager.Instance.GetString(command.GetDescKey());
        _trainingcost.text = StringManager.Instance.GetString(command.GetCost().ToString() + "G");
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
