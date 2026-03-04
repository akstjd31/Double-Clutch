using TMPro;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 육성 선수 목록의 각 선수 버튼에 할당할 스크립트
/// 선수 상태 혹은 예약한 훈련 표시
/// 클릭 시 훈련 목록이 등장
/// </summary>

public class TrainingCharacterBox : MonoBehaviour
{
    [SerializeField] Button _button;
    //[SerializeField] Image _studentImage;
    [SerializeField] Image _stateBackGround;
    [SerializeField] TextMeshProUGUI _nameText;
    [SerializeField] TextMeshProUGUI _stateText;


    Student _student;
    

    public void Init(Student student)
    {
        _student = student;
        _button.onClick.RemoveAllListeners();
        _button.onClick.AddListener(()=> StudentUIManager.Instance.OnTrainingCharacterBoxClick(_student));
        _nameText.text = _student.Name;
        SetStudentState();
    }

    public Button GetSelectButton() => _button;    

    //private void SetStudentImage(Student target)
    //{
    //    _studentImage에 타겟의 비주얼 넣기
    //}

    public void SetStudentState()
    {
        var cv = _stateBackGround.GetComponent<CanvasGroup>();
        cv.alpha = 0f;
        _stateText.text = "";
        if (_student.CurrentTraining != null)
        {
            cv.alpha = 1f;
            _stateBackGround.color = Color.green;
            _stateText.text = StringManager.Instance.GetString(_student.CurrentTraining.GetNameKey());
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
