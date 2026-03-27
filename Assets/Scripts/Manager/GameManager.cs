using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class PrefKeys
{
    public const string MATCH_PREP_UI_INDEX = "MATCH_PREP_UI_INDEX";    // 경기 준비 단계 UI 인덱스
    public const string PLAYER_TEAM_ID = "Player_Team";
}

public static class SceneName
{
    public const string MAIN = "Main";
    public const string EVENT = "Event";
    public const string LOBBY = "Lobby";
    public const string LOADING = "Loading";
    public const string TUTORIAL = "Tutorial";
    public const string GRADUATION = "Graduation";
    public const string SIMULATOR = "Simulator";
}

public static class FilePath
{
    public const string TEAM_PATH = "TeamSaveData.json";
    public const string INFRA_PATH = "InfraSaveData.json";
    public const string PLAYER_PATH = "PlayerSaveData.json";
    public const string LEAGUE_PATH = "LeagueSaveData.json";
    public const string STUDENT_PATH = "StudentSaveData.json";
    public const string SETTING_PATH = "SettingSaveData.json";
    public const string RANDOM_EVENT_PATH = "RandomEventSaveData.json";
    public const string GRADUATION_PATH = "GraduationAlbumSaveData.json";
    public const string LEAGUE_RECORD_PATH = "LeagueRecordSaveData.json";
    public const string MY_STUDENT_MATCHING_PATH = "MyStudentMatchingSaveData.json";
    public const string RIVAL_STUDENT_MATCHING_PATH = "RivalStudentMatchingSaveData.json";
}

public class GameManager : Singleton<GameManager>
{
    public const string CHEAT_CODE = "0123";        // 치트 코드
    public const int MAX_MONEY = 999999;

    [Header("Data")]
    [SerializeField] private PlayerSaveData _saveData;
    public PlayerSaveData SaveData => _saveData;

    public event Action OnDataChanged;

    [Header("GameState")]
    private StateMachine _sm = new StateMachine();
    public string NextSceneName { get; private set; }           // 로딩 이후 전환될 씬 이름
    public IState NextStateAfterLoading { get; private set; }   // 로딩 이후에 적용될 상태 정보를 담고 있음.

    [Header("Command")]
    private GameContext _ctx;
    private CommandBus _bus;

    protected override void Awake()
    {
        base.Awake();

        SaveLoadManager.Instance.TryLoad<PlayerSaveData>(FilePath.PLAYER_PATH, out _saveData);
        InitRegister();

        // 로딩화면에서 시작하여 메인으로 넘어가기
        SetNextFlow(SceneName.MAIN, _sm.Get<MainState>());
    }

    // 객체 미리 등록해놓기
    private void InitRegister()
    {
        _sm.Register(new MainState(this, _sm));
        _sm.Register(new LoadingState(this, _sm));
        _sm.Register(new LobbyState(this, _sm));
        _sm.Register(new EventState(this, _sm));
        _sm.Register(new MatchPrepState(this, _sm));
        _sm.Register(new MatchSimState(this, _sm));
        _sm.Register(new ResultState(this, _sm));
        _sm.Register(new TutorialState(this, _sm));
        _sm.Register(new GraduationState(this, _sm));
    }

    public void InitData(PlayerSaveData data)
    {
        _saveData = data;
        Debug.Log($"{_saveData.schoolName} 학교 {_saveData.coachName} 감독님 환영합니다!");
        SavePlayerData();
    }

    public bool HasData() => _saveData != null;

    private void SavePlayerData() => SaveLoadManager.Instance.Save(FilePath.PLAYER_PATH, _saveData);

    private void Start()
    {
        _sm.ChangeState<LoadingState>();

        OnDataChanged += SavePlayerData;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        
        OnDataChanged -= SavePlayerData;
    }

    // 각 상태 클래스에서 필요시 사용
    public bool Execute(ICommand cmd) => _bus.Execute(_ctx, cmd);

    public void Dispatch(UIAction action)
    {
        if (_sm.CurrentState is IUIActionHandler h)
            h.Handle(action);
    }

    // 다음 씬, 상태 정보를 임시 저장하기 위한 메서드
    public void SetNextFlow(string sceneName, IState nextState)
    {
        NextSceneName = sceneName;
        NextStateAfterLoading = nextState;
    }

    // LoadingScene에서 호출
    public void NotifyLoadingDone()
    {
        if (NextStateAfterLoading != null)
            _sm.ChangeState(NextStateAfterLoading);
    }

    public void LoadNextScene()
    {
        StartCoroutine(LoadNextScene_Coroutine());
    }

    public void GoToGraduation()
    {
        SetNextFlow(SceneName.GRADUATION, _sm.Get<GraduationState>());

        _sm.ChangeState<LoadingState>();
    }

    public void GoToLobby()
    {
        SetNextFlow(SceneName.LOBBY, _sm.Get<LobbyState>());

        _sm.ChangeState<LoadingState>();
    }

    private IEnumerator LoadNextScene_Coroutine()
    {
        var target = NextSceneName;

        var op = SceneManager.LoadSceneAsync(target, LoadSceneMode.Single);
        op.allowSceneActivation = true;

        yield return op; // 씬 로드 완료까지 대기

        NotifyLoadingDone(); // 로드 끝난 뒤 상태 전환
    }

    public void SetTutorialCompleted(int index, bool flag)
    {
        _saveData.tutorialCompleted[index] = flag;
        OnDataChanged?.Invoke();
    }

    public void SetCoachName(string name)
    {
        _saveData.coachName = name;
        OnDataChanged?.Invoke();
    }
    
    public void SetSchoolName(string name)
    {
        _saveData.schoolName = name;
        OnDataChanged?.Invoke();
    }

    public void SetMoney(int money)
    {
        _saveData.money = HasMaximumMoney() ? MAX_MONEY : money;
        OnDataChanged?.Invoke();
    }

    public bool HasMaximumMoney() => _saveData.money > MAX_MONEY;

    public void SetWeekId(int weekId)
    {
        _saveData.weekId = weekId;
        OnDataChanged?.Invoke();
    }

    public void SetHonor(int honor)
    {
        _saveData.honor = honor;
        OnDataChanged?.Invoke();
    }

    public void SetTotalWinHonor(int honor)
    {
        _saveData.totalWinHonor = honor;
        OnDataChanged?.Invoke();
    }

    public void ClearTotalWinHonorData()
    {
        _saveData.totalWinHonor = 0;
        OnDataChanged?.Invoke();
    }

    public void ClearLeagueWinData()
    {
        _saveData.leagueWinRecord.Clear();
        OnDataChanged?.Invoke();
    }

    public void SetYear(int year)
    {
        _saveData.year = year;
        OnDataChanged?.Invoke();
    }
    
    
    public void ChangeState<T>() where T : class, IState
    {
        _sm.ChangeState<T>();
    }
    
    public void LoadMatchSceneWithData(string sceneName, List<Student> homeRoster, List<Student> awayRoster)
    {
        // 상태 머신에서 MatchSimState를 미리 꺼내서 데이터를 주입
        var matchState = _sm.Get<MatchSimState>();
        matchState.SetRosters(homeRoster, awayRoster);

        // 다음 넘어갈 씬과 상태를 세팅하고 로딩 씬으로 이동
        SetNextFlow(sceneName, matchState);
        _sm.ChangeState<LoadingState>();
    }

    
    public int GetGraduationCount(string visualId)//프로필 해금을 위한 졸업생 종족(비주얼) 카운트.
    {
        // 리스트에서 ID가 일치하는 첫 번째 요소를 찾고, 없으면 null 반환
        var record = SaveData.graduationRecord.FirstOrDefault(x => x.visualId == visualId);

        // 찾았다면 count를, 못 찾았다면 0을 반환
        return record != null ? record.count : 0;
    }

    public void AddGraduationCount(string visualId)
    {
        var record = SaveData.graduationRecord.FirstOrDefault(x => x.visualId == visualId);

        if (record != null)
        {
            record.count++; // 이미 있으면 1 증가
        }
        else
        {
            // 없으면 새로 만들어서 리스트에 추가
            SaveData.graduationRecord.Add(new GraduationRecord { visualId = visualId, count = 1 });
        }

        OnDataChanged?.Invoke();
    }

    public void SetCurrentProfileIcon(string imageKey)
    {
        SaveData.currentProfileImage = imageKey;
        OnDataChanged?.Invoke();
    }

    public void SetGraduationPending(bool value)
    {
        _saveData.isGraduationPending = value;
        OnDataChanged?.Invoke();
    }
}
