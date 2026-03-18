using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TrainingCharacterBox : MonoBehaviour
{
    [SerializeField] Image _studentImage;
    [SerializeField] Image _positionImage;
    [SerializeField] Image _traitImage;
    [SerializeField] Button _button;
    [SerializeField] Image _stateBackGround;
    [SerializeField] TextMeshProUGUI _attackPointText;
    [SerializeField] TextMeshProUGUI _guardPointText;
    
    [SerializeField] TextMeshProUGUI _nameText;
    [SerializeField] TextMeshProUGUI _stateText;

    [SerializeField] Slider _conditionSlider;

    [SerializeField] Sprite _c;
    [SerializeField] Sprite _sf;
    [SerializeField] Sprite _sg;
    [SerializeField] Sprite _pf;
    [SerializeField] Sprite _pg;
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

        _studentImage.sprite = SpriteManager.Instance.GetSprite(_student.VisualData.portraitResource);
        _positionImage.sprite = GetPositionImage(_student.Position);
        _positionImage.sprite = SpriteManager.Instance.GetSprite(_student.TraitData.traitResource);


        StringManager manager = StringManager.Instance;
        string name = manager.GetString(_student.Name[0]) + manager.GetString(_student.Name[1]) + manager.GetString(_student.Name[2]);

        _button.onClick.RemoveAllListeners();
        _button.onClick.AddListener(() => StudentUIManager.Instance.OnTrainingCharacterBoxClick(_student));
        _nameText.text = name;
        _attackPointText.text = _student.Attack.ToString();
        _guardPointText.text = _student.Defense.ToString();
        manager.ApplyFont(_nameText);
        SetStudentState();

        _conditionSlider.value = NormalizeConditionValue(_student.Condition);
    }

    public void SetStudentState()
    {
        StringManager manager = StringManager.Instance;
        if (_student == null) return;
        var cv = _stateBackGround.GetComponent<CanvasGroup>();
        cv.alpha = 0f;
        _stateText.text = "";
        if (_student.CurrentTraining != null)
        {
            cv.alpha = 1f;
            manager.GetString(_student.CurrentTraining.GetNameKey(), _stateText);

            if (_student.CurrentTraining is IndividualTraining || _student.CurrentTraining is TeamTraining)
            {
                _stateText.color = Color.blue;
            }
            else
            {
                _stateText.color= Color.green;
            }

            return;
        }
        if (_student.State == StudentState.OverWorked)
        {
            cv.alpha = 1f;
            _stateText.text = manager.GetString("UI_Player_과로");            
            _stateText.color = Color.yellow;
            return;
        }
        if (_student.State == StudentState.Injured)
        {
            cv.alpha = 1f;
            _stateText.text = manager.GetString("UI_Player_부상");            
            _stateText.color = Color.red;
            return;
        }
        manager.ApplyFont(_stateText);
    }

    public Sprite GetPositionImage(Position position)
    {
        Sprite sprite = null;
        switch(position)
        {
            case Position.C: sprite = _c; break;
            case Position.SF: sprite = _sf; break;
            case Position.SG: sprite = _sg; break;
            case Position.PF: sprite = _pf; break;
            case Position.PG: sprite = _pg; break;
        }
        return sprite;
    }
    public float NormalizeConditionValue(int condition)
    {
        return (float)condition / 100;
    }
}
