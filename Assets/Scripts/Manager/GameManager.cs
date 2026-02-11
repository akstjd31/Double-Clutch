using UnityEngine;

// 게임의 전반적인 상태
// public enum GameState
// {
//     Main,       // 메인 화면
//     Lobby,      // 로비
//     Event,      // 이벤트 발생 시점
//     MatchPrep,  // 농구 시합 전 단계 (준비)
//     MatchSim,   // 농구 시합
//     Result      // 결과 (보상 지급)
// }

public class GameManager : MonoBehaviour
{
    [Header("GameState")]
    private StateMachine _stateMachine = new StateMachine();
    

    private void Start()
    {
    }
}
