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

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("GameState")]
    private StateMachine _sm = new StateMachine();
    

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(this.gameObject);

        InitRegister();
    }

    // 객체 미리 등록해놓기
    private void InitRegister()
    {
        _sm.Register(new MainState(this, _sm));
        _sm.Register(new LoadingState(_sm));
        _sm.Register(new LobbyState(this, _sm));
    }


    private void Start()
    {
        _sm.ChangeState<MainState>();
    }
}
