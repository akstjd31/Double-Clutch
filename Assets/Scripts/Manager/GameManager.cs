using UnityEngine;

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

public class GameManager : Singleton<GameManager>
{

    [Header("GameState")]
    private StateMachine _sm = new StateMachine();

    [Header("Command")]
    private GameContext _ctx;
    private CommandBus _bus;

    protected override void Awake()
    {
        base.Awake();

        InitCommandSystem();
        InitRegister();
    }

    private void InitCommandSystem()
    {  
        var file = "player_save.json";
        // 임시 저장 경로
        if (!SaveLoadManager.Instance.TryLoad(file, out PlayerSaveData save))
            save = new PlayerSaveData();

        _ctx = new GameContext(save, SaveLoadManager.Instance, file);

        _bus = new CommandBus();
        _bus.OnFailed += (_, msg) => Debug.LogWarning($"실패: {msg}");
        _bus.OnExecuted += (cmd) => Debug.Log($"성공: {cmd.GetType().Name}");
    }

    // 객체 미리 등록해놓기
    private void InitRegister()
    {
        _sm.Register(new MainState(this, _sm));
        _sm.Register(new LoadingState(_sm));
        _sm.Register(new LobbyState(this, _sm));
        _sm.Register(new EventState(this, _sm));
        _sm.Register(new MatchPrepState(this, _sm));
        _sm.Register(new MatchSimState(this, _sm));
        _sm.Register(new ResultState(this, _sm));
    }

    private void Start()
    {
        _sm.ChangeState<MainState>();
    }

    // 각 상태 클래스에서 필요시 사용
    public bool Execute(ICommand cmd) => _bus.Execute(_ctx, cmd);
}
