using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class CharacterRecruitBox : MonoBehaviour
    //IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] Image _characterImage;
    [SerializeField] TextMeshProUGUI _positionText;
    [SerializeField] TextMeshProUGUI _gradeText;
    [SerializeField] TextMeshProUGUI _nameText;
    [SerializeField] TextMeshProUGUI _attackText;
    [SerializeField] TextMeshProUGUI _defenseText;    
    [SerializeField] Toggle _toggleButton;
    [SerializeField] Outline _outLine;
    [SerializeField] Button _profileButton;

    private Student _student;
    private bool isSelected = false;
    public bool IsSelected => isSelected;


    private Coroutine _longPressRoutine;
    private float _pressTime = 2.0f;


    private void Start()
    {
        _toggleButton.onValueChanged.AddListener(ChangeToggleState);

        _toggleButton.isOn = false;
        ChangeToggleState(false);
    }

    public void Init(Student target)
    {
        _student = target;
        SetText();
        SetButton();
    }

    public Student GetStudent()
    {
        return _student;
    }

    private void SetText()
    {
        StringManager manager = StringManager.Instance;
        string name = manager.GetString(_student.Name[0]) + manager.GetString(_student.Name[1]) + manager.GetString(_student.Name[2]);

        _positionText.text = _student.Position.ToString();
        _gradeText.text = _student.Grade.ToString() + "학년";
        _nameText.text = name;
        _attackText.text = _student.Attack.ToString();
        _defenseText.text = _student.Defense.ToString();
    }

    private void ChangeToggleState(bool isOn)
    {
        if (isOn)
        {
            isSelected = true;
            _outLine.enabled = true;
        }
        else
        {
            isSelected = false;
            _outLine.enabled = false;
        }
    }

    private void SetButton()
    {
        _profileButton.onClick.RemoveAllListeners();
        _profileButton.onClick.AddListener(() => StudentUIManager.Instance.OpenProfilePopUp(_student));
    }

    //터치 시 프로필 팝업 등장 구현
    public void OnPointerDown(PointerEventData eventData)
    {
        if (_longPressRoutine != null) StopCoroutine(_longPressRoutine);
        _longPressRoutine = StartCoroutine(Co_LongPressTimer());
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // 2초가 되기 전에 손을 떼면 팝업이 뜨지 않도록 타이머만 중단
        if (_longPressRoutine != null)
        {
            StopCoroutine(_longPressRoutine);
            _longPressRoutine = null;
        }
    }

    private IEnumerator Co_LongPressTimer()
    {
        yield return new WaitForSeconds(_pressTime);

        if (_student != null)
        {
            StudentUIManager.Instance.OpenProfilePopUp(_student);// 팝업이 떴으므로 참조를 비워줌 (손을 뗐을 때 StopCoroutine 중복 호출 방지)
            _longPressRoutine = null;
        }
    }

    private void OnDestroy()
    {
        _toggleButton?.onValueChanged?.RemoveAllListeners();
    }

    
}
