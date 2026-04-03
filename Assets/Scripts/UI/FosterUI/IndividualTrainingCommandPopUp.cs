using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.UI;

public class IndividualTrainingCommandPopUp : MonoBehaviour
{
    [SerializeField] Transform _trainingListParent;
    [SerializeField] TrainingBox _trainingBoxPrefab;
    [SerializeField] TextMeshProUGUI _nameText;
    [SerializeField] TextMeshProUGUI _attackPoint;
    [SerializeField] TextMeshProUGUI _defensePoint;
    [SerializeField] Toggle _pg;
    [SerializeField] Toggle _pf;
    [SerializeField] Toggle _sg;
    [SerializeField] Toggle _sf;
    [SerializeField] Toggle _c;
    
    private GenericObjectPool<TrainingBox> _pool;
    private List<TrainingBox> _boxList = new List<TrainingBox>();
    private Student _selectedStudent;
    private TrainingPanel _trainingPanel;

    private void Awake()
    {        
        _pool = new GenericObjectPool<TrainingBox>(_trainingBoxPrefab, _trainingListParent);
        _trainingPanel = GameObject.FindAnyObjectByType<TrainingPanel>();
    }

    private void OnEnable()
    {
        StringManager.OnLanguageChanged += Refresh;
    }
    private void OnDisable()
    {
        StringManager.OnLanguageChanged -= Refresh;
    }

    public void Init(Student student)
    {
        _selectedStudent = student;
        SetPositionToggle(student.Position);
        // 기존 사용하던 박스 반납
        foreach (var box in _boxList)
        {
            _pool.Release(box);
        }

        _boxList.Clear();

        // 개인 훈련 데이터 생성 및 배치
        var trainingDB = FosterManager.Instance.IndividualTrainingDB.DataList;
        for (int i = 0; i < trainingDB.Count; i++)
        {
            CreateBox(new IndividualTraining(trainingDB[i]));
        }

        // 개인 휴식 데이터 생성 및 배치
        var restDB = FosterManager.Instance.IndividualRestDB.DataList;
        for (int i = 0; i < restDB.Count; i++)
        {
            CreateBox(new IndividualRest(restDB[i]));
        }

        StringManager manager = StringManager.Instance;
        string name = manager.GetString(_selectedStudent.Name[0]) + manager.GetString(_selectedStudent.Name[1]) + manager.GetString(_selectedStudent.Name[2]);
        _nameText.text = name +" "+manager.GetString("UI_Popup_육성커맨드");
        if (StringManager.Instance.CurrentLanguage == Language.Ko)
        {
            _nameText.fontSize = 56;
        }
        if (StringManager.Instance.CurrentLanguage == Language.En)
        {
            _nameText.text = name + "\n" + manager.GetString("UI_Popup_육성커맨드");
            _nameText.fontSize = 48;
        }
        if (StringManager.Instance.CurrentLanguage == Language.Ja)
        {
            _nameText.text = name + "\n" + manager.GetString("UI_Popup_육성커맨드");
            _nameText.fontSize = 48;
        }

        _attackPoint.text = _selectedStudent.Attack.ToString();
        _defensePoint.text = _selectedStudent.Defense.ToString();

        manager.ApplyFont(_nameText);
    }

    private void SetPositionToggle(Position position)
    {
        // 1. 기존 리스너 제거 (무한 루프 및 중복 방지)
        RemoveAllToggleListeners();

        // 2. 모든 토글 끄기 (초기화)
        _pg.isOn = _pf.isOn = _sg.isOn = _sf.isOn = _c.isOn = false;

        // 3. 현재 포지션 설정 (시각적 갱신을 위해 isOn 사용)
        switch (position)
        {
            case Position.PG: _pg.isOn = true; break;
            case Position.PF: _pf.isOn = true; break;
            case Position.SG: _sg.isOn = true; break;
            case Position.SF: _sf.isOn = true; break;
            case Position.C: _c.isOn = true; break;
        }

        // 4. 리스너 다시 등록
        AddAllToggleListeners();
    }

    private void RemoveAllToggleListeners()
    {
        _pg.onValueChanged.RemoveAllListeners();
        _pf.onValueChanged.RemoveAllListeners();
        _sg.onValueChanged.RemoveAllListeners();
        _sf.onValueChanged.RemoveAllListeners();
        _c.onValueChanged.RemoveAllListeners();
    }

    private void AddAllToggleListeners()
    {
        _pg.onValueChanged.AddListener(isOn => { if (isOn) ChangePosition(true, Position.PG); });
        _pf.onValueChanged.AddListener(isOn => { if (isOn) ChangePosition(true, Position.PF); });
        _sg.onValueChanged.AddListener(isOn => { if (isOn) ChangePosition(true, Position.SG); });
        _sf.onValueChanged.AddListener(isOn => { if (isOn) ChangePosition(true, Position.SF); });
        _c.onValueChanged.AddListener(isOn => { if (isOn) ChangePosition(true, Position.C); });
    }

    private void Refresh()
    {
        StringManager manager = StringManager.Instance;
        string name = manager.GetString(_selectedStudent.Name[0]) + manager.GetString(_selectedStudent.Name[1]) + manager.GetString(_selectedStudent.Name[2]);
        _nameText.text = name+" "+manager.GetString("UI_Popup_육성커맨드");
        manager.ApplyFont(_nameText);
        if (StringManager.Instance.CurrentLanguage == Language.Ko)
        {
            _nameText.fontSize = 56;
        }
        if (StringManager.Instance.CurrentLanguage == Language.En)
        {
            _nameText.text = name + "\n" + manager.GetString("UI_Popup_육성커맨드");
            _nameText.fontSize = 48;
        }
        if (StringManager.Instance.CurrentLanguage == Language.Ja)
        {
            _nameText.text = name + "\n" + manager.GetString("UI_Popup_육성커맨드");
            _nameText.fontSize = 48;
        }

    }

    private void CreateBox(ITraining command)
    {
        // 풀에서 박스 생성
        TrainingBox box = _pool.Get();
        box.transform.SetAsLastSibling();

        // 데이터 주입 (학생 설정 후 Init 호출)
        box.Init(command, _trainingPanel);
        box.SetStudent(_selectedStudent);        

        _boxList.Add(box);
    }
        
    //선수 개인 훈련 시 포지션 변경 버튼에 각각 연결
    public void OnCClick(bool isOn) => ChangePosition(isOn, Position.C);
    public void OnPFClick(bool isOn) => ChangePosition(isOn, Position.PF);
    public void OnPGClick(bool isOn) => ChangePosition(isOn, Position.PG);
    public void OnSFClick(bool isOn) => ChangePosition(isOn, Position.SF);
    public void OnSGClick(bool isOn) => ChangePosition(isOn, Position.SG);
    private void ChangePosition(bool isOn, Position position)
    {
        if (isOn && _selectedStudent != null)
        {
            _selectedStudent.SetPosition(position);
            _trainingPanel.RefreshPositionMark();            
        }
    }
}