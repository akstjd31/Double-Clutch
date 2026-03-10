using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
/// <summary>
/// todo : 1. 개인 훈련, 2. 개인 휴식, 3. 팀 훈련, 4. 팀 휴식
/// </summary>
public class FosterManager : MonoBehaviour
{
    public static FosterManager Instance;
    [Header("<size=18>데이터 리더 SO 모음</size>")]
    [Header("Individual_TrainingDataReader(개인 훈련 데이터)")]
    [SerializeField] Individual_TrainingDataReader _individual_TrainingDB;
    [Header("Individual_RestDataReader(개인 휴식 데이터)")]
    [SerializeField] Individual_RestDataReader _individual_RestDB;
    [Header("Team_TrainingDataReader(팀 훈련 데이터)")]
    [SerializeField] Team_TrainingDataReader _team_TrainingDB;
    [Header("Team_RestDataReader(팀 휴식 데이터)")]
    [SerializeField] Team_RestDataReader _team_RestDB;
    [Header("Team_Position_MappingDataReader(팀 포지션 매핑 데이터)")]
    [SerializeField] Team_Position_MappingDataReader _team_Position_MappingDB;
    [Header("Team_Position_MappingDataReader(훈련 카테고리 매핑 데이터)")]
    [SerializeField] Training_MappingDataReader _training_MappingDB;

    public Individual_TrainingDataReader IndividualTrainingDB => _individual_TrainingDB;
    public Individual_RestDataReader IndividualRestDB => _individual_RestDB;

    public Team_TrainingDataReader Team_TrainingDB => _team_TrainingDB;
    public Team_RestDataReader Team_RestDB => _team_RestDB;
    private int _myGold => GameManager.Instance.SaveData.money;

    Dictionary<Student, ITraining> _schedules = new Dictionary<Student, ITraining>(); //개인 스케줄 예약 목록
    
    ITraining _teamSchedule; //팀 스케줄 예약 목록

    int _scheduleCost = 0;
    int _scheduleCount => (_teamSchedule != null)? StudentManager.Instance.MyStudents.Count : _schedules.Count;

    private void Awake()
    {
        Instance = this;
    }

    public void OnStartFosterClick()
    {
        bool isTeamReserved = (_teamSchedule != null);
        bool isAllIndividualReserved = (_schedules.Count == StudentManager.Instance.MyStudents.Count);

        if (!isTeamReserved && !isAllIndividualReserved)
        {
            //애초에 버튼 활성화가 안되어야 함
            return;
        }

        List<Student> problemStudents = new List<Student>();

        if (_teamSchedule != null) //팀 스케줄이 예약되어 있다면 전체 학생 목록을 사용
        {
            foreach (var student in StudentManager.Instance.MyStudents)
            {
                if (HasProblem(student)) //문제 있는 학생 검사 및 분류
                {
                    problemStudents.Add(student);
                }
            }
        }
        else
        {
            foreach (var student in _schedules.Keys) //개인 스케줄 목록에서 학생 검사
            {
                if (HasProblem(student)) //문제 있는 학생 검사 및 분류
                {
                    problemStudents.Add(student);
                }                
            }
        }
        
        if (problemStudents.Count > 0) //문제 있는 학생이 하나라도 있다면
        {
            StudentUIManager.Instance.OpenConditionWarningPopUp(problemStudents, _scheduleCost); //컨디션 경고 팝업 호출
            Debug.Log("컨디션 경고창 호출");
        }
        else
        {
            StudentUIManager.Instance.OpenTrainingStartConfirmPopUp(_scheduleCost);
            Debug.Log("훈련 확인창 호출");
        }
    }
    private bool HasProblem(Student target)
    {
        return target.Condition <= 0 || target.State != StudentState.None;
    }

    public void UpdateScheduleState()
    {
        if (StudentManager.Instance == null || StudentUIManager.Instance == null) return;

        // 2. 학생 목록 가져오기
        var students = StudentManager.Instance.MyStudents;
        int maxStudentCount = (students != null) ? students.Count : 0;
        int currentReservedCount = 0;
        bool canStart = false;

        // 디버깅: 현재 학생 수가 몇 명으로 찍히는지 확인 (0이 나온다면 데이터 로드 순서 문제)
        Debug.Log($"[FosterManager] 현재 학생 수: {maxStudentCount}, 예약된 수: {_schedules.Count}");

        if (_teamSchedule != null)
        {
            currentReservedCount = maxStudentCount;
            // 학생이 최소 1명은 있어야 팀 훈련 시작 가능
            canStart = (maxStudentCount > 0);
        }
        else
        {
            currentReservedCount = _schedules.Count;
            // 개인 훈련은 예약된 수와 전체 학생 수가 같아야 하며, 학생 수가 0보다 커야 함
            canStart = (maxStudentCount > 0 && currentReservedCount == maxStudentCount);
        }

        // 3. UI 매니저에게 전달
        StudentUIManager.Instance.RefreshStartFosterButton(canStart, currentReservedCount, maxStudentCount);
    }

    public void StartFoster()
    {
        Debug.Log("StartFoster 진입");
        foreach (var student in StudentManager.Instance.MyStudents)
        {
            student.PrepareStatChange();
        }
        if (_teamSchedule != null)
        {
            Debug.Log("팀 스케줄 실행 시도");
            _teamSchedule.StartAction();
            Debug.Log("팀 스케줄 실행 완료");
        }
        else
        {
            Debug.Log($"개인 스케줄 실행 시도 (개수: {_schedules.Count})");
            foreach (var training in _schedules.Values)
            {
                training.StartAction();                
            }
            Debug.Log("개인 스케줄 실행 완료");
        }        
        
        GameManager.Instance.SetMoney(_myGold - _scheduleCost);

        _schedules.Clear(); //내부 카운트 및 UI 상태 초기화
        _scheduleCost = 0;
        _teamSchedule = null;
        UpdateScheduleState();         
        StudentUIManager.Instance.OpenWeeklyTrainingReportPopUp(StudentManager.Instance.MyStudents);
    }
    

    public void ReserveIndividualTraining(ITraining command)
    {
        Student target = command.GetTarget();
        if (_teamSchedule != null) //개인 스케줄 예약시 팀 스케줄 예약은 삭제
        {
            _schedules.Clear();
            _scheduleCost = 0;
            _teamSchedule = null;
        }

        int oldCost = _schedules.ContainsKey(target) ? _schedules[target].GetCost() : 0;
        int nextTotalCost = _scheduleCost - oldCost + command.GetCost();
        
        if (nextTotalCost > _myGold) // 돈 체크
        {
            StudentUIManager.Instance.OpenCostWarningPopUp();
            return;
        }

        if (target.State != StudentState.None && command is IndividualTraining) //학생에게 부상 및 과로가 있는데 개인 훈련을 할당하려고 하면 여기서 팝업띄우고 할당 못하게 함.
        {
            StudentUIManager.Instance.OpenStateWarningPopUp_Individual(target);
            return;
        }

        if (_schedules.ContainsKey(command.GetTarget())) // 해당 학생 개인 예약이 이미 존재하면 커맨드만 교체
        {
            _schedules[target] = command;
        }
        else
        {
            _schedules.Add(target, command); // 아니라면 딕셔너리에 학생 / 커맨드 추가
        }
        target.SetCurrentTraining(command);
        _scheduleCost = nextTotalCost; // 최종 비용 업데이트
        UpdateScheduleState(); //버튼 표시 및 활성화 여부 갱신
        StudentUIManager.Instance.OnTrainingReserved();
    }

    public void ReserveTeamTraining(ITraining command)
    {
        int nextTotalCost = command.GetCost();

        if (nextTotalCost > _myGold) //돈 체크
        {
            StudentUIManager.Instance.OpenCostWarningPopUp();
            return;
        }

        foreach (var target in StudentManager.Instance.MyStudents)
        {
            //if (target.State != StudentState.None && command is TeamTraining) //선수 한 명이라도 부상 및 과로가 있는데 팀 훈련을 예약하려고 하면 여기서 팝업띄우고 경고만 해주기(예약은 됨)
            //{
            //    StudentUIManager.Instance.OpenStateWarningPopUp_Team();                ==>>> 최종 훈련 버튼 클릭 시 개별 맞춤형 경고하는 것으로 수정(기획 변경)
            //}
            target.SetCurrentTraining(command);
        }

        _schedules.Clear(); //팀 스케줄 예약 시 개인 스케줄 예약 목록 삭제.
        _teamSchedule = command;        
        _scheduleCost = nextTotalCost;

        UpdateScheduleState(); //버튼 표시 및 활성화 여부 갱신
        StudentUIManager.Instance.OnTrainingReserved();
    }


    public Team_Position_MappingData GetPositionMapping(Student student)
    {
        foreach (var data in _team_Position_MappingDB.DataList)
        {
            if (data.position == student.Position)
            {
                return data;
            }
        }
        Debug.LogWarning($"{student.Position} 포지션에 대한 매핑 데이터를 찾을 수 없습니다.");
        return default;
    }

    public Training_MappingData GetTrainingMapping(potential potential)
    {
        foreach(var data in _training_MappingDB.DataList)
        {
            if (data.potential == potential)
            {
                return data;
            }
        }
        Debug.LogWarning($"{potential} 잠재력에 대한 매핑 데이터를 찾을 수 없습니다.");
        return default;        
    }
}