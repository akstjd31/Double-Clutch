using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class PrefKeys
{
    public const string KEY_FIRST_RUN_DONE = "FIRST_RUN_DONE";          // 튜토리얼 여부 결정
    public const string MATCH_PREP_UI_INDEX = "MATCH_PREP_UI_INDEX";    // 경기 준비 단계 UI 인덱스
}

public static class SceneName
{
    public const string MAIN = "Test_Main";
    public const string LOBBY = "Test_Lobby";
    public const string LOADING = "Test_Loading";
    public const string EVENT = "Test_Event";
    public const string GRADUATION = "Test_Graduation";
}

public static class FilePath
{
    public const string PLAYER_PATH = "PlayerSaveData.json";
    public const string MY_STUDENT_MATCHING_PATH = "MyStudentMatchingSaveData.json";
    public const string RIVAL_STUDENT_MATCHING_PATH = "RivalStudentMatchingSaveData.json";
    public const string INFRA_PATH = "InfraSaveData.json";
}

public class GameManager : Singleton<GameManager>
{
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

        InitCommandSystem();
        InitRegister();

        // 로딩화면에서 시작하여 메인으로 넘어가기
        SetNextFlow(SceneName.MAIN, _sm.Get<MainState>());
    }

    private void InitCommandSystem()
    {
        // 임시 저장 경로 (데이터 존재 여부에 따라 처음인지 아닌지를 판별)
        if (SaveLoadManager.Instance.TryLoad(FilePath.PLAYER_PATH, out _saveData))
        {
            PlayerPrefs.SetInt(PrefKeys.KEY_FIRST_RUN_DONE, 1);
        }
        else
        {
            PlayerPrefs.SetInt(PrefKeys.KEY_FIRST_RUN_DONE, 0);
        }

        // _ctx = new GameContext(save, SaveLoadManager.Instance, FilePath.PLAYER_PATH);

        // _bus = new CommandBus();
        // _bus.OnFailed += (_, msg) => Debug.LogWarning($"실패: {msg}");
        // _bus.OnExecuted += (cmd) => Debug.Log($"성공: {cmd.GetType().Name}");
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
    private void NotifyLoadingDone()
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
}
