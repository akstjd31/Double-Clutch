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

    private Color _restColor = new Color32(0, 223, 112, 255);
    private Color _trainingColor = new Color32(0, 93, 232, 255);
    private Color _overworkColor = new Color32(255, 174, 0, 255);
    private Color _injuredColor = new Color32(204, 0, 0, 255);

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

    public void RefreshPositionMark()
    {
        _positionImage.sprite = SpriteManager.Instance.GetPositionSprite(_student.Position);
    }

    public void Init(Student student)
    {
        _student = student;

        SpriteManager spriteManager = SpriteManager.Instance;
        _studentImage.sprite = spriteManager.GetSprite(_student.VisualData.portraitResource);
        _positionImage.sprite = spriteManager.GetPositionSprite(_student.Position);
        _traitImage.sprite = SpriteManager.Instance.GetSprite(_student.TraitData.traitResource);


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
        string name = manager.GetString(_student.Name[0]) + manager.GetString(_student.Name[1]) + manager.GetString(_student.Name[2]);
        _nameText.text = name;
        manager.ApplyFont(_nameText);
        if (_student == null) return;
        var cv = _stateBackGround.GetComponent<CanvasGroup>();
        cv.alpha = 0f;
        _stateText.text = "";
        _stateText.color = Color.white;
        if (_student.CurrentTraining != null)
        {
            cv.alpha = 1f;
            manager.GetString(_student.CurrentTraining.GetNameKey(), _stateText);

            if (_student.CurrentTraining is IndividualTraining || _student.CurrentTraining is TeamTraining)
            {
                if (StringManager.Instance.CurrentLanguage == Language.Ko)
                {
                    _stateText.fontSize = 32;
                }
                if (StringManager.Instance.CurrentLanguage == Language.En)
                {
                    _stateText.fontSize = 32;
                }
                if (StringManager.Instance.CurrentLanguage == Language.Ja)
                {
                    _stateText.fontSize = 24;
                }
                _stateText.color = _trainingColor;
                manager.ApplyFont(_stateText);
            }
            else
            {
                if (StringManager.Instance.CurrentLanguage == Language.Ko)
                {
                    _stateText.fontSize = 32;
                }
                if (StringManager.Instance.CurrentLanguage == Language.En)
                {
                    _stateText.fontSize = 32;
                }
                if (StringManager.Instance.CurrentLanguage == Language.Ja)
                {
                    _stateText.fontSize = 24;
                }
                _stateText.color= _restColor;
                manager.ApplyFont(_stateText);
            }

            return;
        }
        if (_student.State == StudentState.OverWorked)
        {
            cv.alpha = 1f;
            _stateText.text = manager.GetString("UI_Player_과로");            
            _stateText.color = _overworkColor;
            manager.ApplyFont(_stateText);
            return;
        }
        if (_student.State == StudentState.Injured)
        {
            cv.alpha = 1f;
            _stateText.text = manager.GetString("UI_Player_부상");            
            manager.ApplyFont(_stateText);
            _stateText.color = _injuredColor;
            return;
        }
    }
    
    public float NormalizeConditionValue(int condition)
    {
        return (float)condition / 100;
    }
}
