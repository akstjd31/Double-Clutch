using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

// 게임의 전반적인 상태
// public enum GameState
// {
//     Main,       // 메인 화면
//     Loding,
//     Lobby,      // 로비
//     Event,      // 이벤트 발생 시점
//     MatchPrep,  // 농구 시합 전 단계 (준비)
//     MatchSim,   // 농구 시합
//     Result      // 결과 (보상 지급)
// }
public static class PrefKeys
{
    public const string KEY_FIRST_RUN_DONE = "FIRST_RUN_DONE";
    public const string CURRENCY_SUBSIDY = "CURRENCY_SUBSIDY";
}

public static class SceneName
{
    public const string MAIN = "Test_Main";
    public const string LOBBY = "Test_Lobby";
    public const string LOADING = "Test_Loading";
    public const string EVENT = "Test_Event";
}

public static class FilePath
{
    public const string PLAYER_PATH = "Player_Data.json";
}

public class GameManager : Singleton<GameManager>
{
    [Header("Data")]
    [SerializeField] private PlayerSaveData saveData;
    public PlayerSaveData SaveData => saveData;

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
        if (SaveLoadManager.Instance.TryLoad(FilePath.PLAYER_PATH, out saveData))
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
    }

    public void InitData(PlayerSaveData data)
    {
        saveData = data;
        Debug.Log($"{saveData.schoolName} 학교 {saveData.coachName} 감독님 환영합니다!");
        SavePlayerData();
    }

    private void SavePlayerData() => SaveLoadManager.Instance.Save(FilePath.PLAYER_PATH, saveData);

    private void Start()
    {
        _sm.ChangeState<LoadingState>();
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

    private IEnumerator LoadNextScene_Coroutine()
    {
        var target = NextSceneName;

        var op = SceneManager.LoadSceneAsync(target, LoadSceneMode.Single);
        op.allowSceneActivation = true;

        yield return op; // 씬 로드 완료까지 대기

        NotifyLoadingDone(); // 로드 끝난 뒤 상태 전환
    }

    public void SetMoney(int money)
    {
        saveData.money = money;
        OnDataChanged?.Invoke();
    }

    public void SetWeekId(int weekId)
    {
        saveData.weekId = weekId;
        OnDataChanged?.Invoke();
    }

    public void SetHonor(int honor)
    {
        saveData.honor = honor;
        OnDataChanged?.Invoke();
    }
}
