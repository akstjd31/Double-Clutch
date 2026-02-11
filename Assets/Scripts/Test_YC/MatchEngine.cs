using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using AYellowpaper.SerializedCollections;

public class MatchEngine : MonoBehaviour
{
    [Header("Visual Objects")]
    [SerializeField] private Transform _ballTransform; // 공 오브젝트 연결
    [SerializeField] private Transform _hoopTransform; // 골대 위치 (슛할 때 목표)

    [SerializeField] private MatchState _matchState; // 인스펙터 연결 혹은 Find
    [SerializeField] private MatchUIManager _uiManager;
    [SerializedDictionary("Position", "Prefab")]
    public SerializedDictionary<Position, GameObject> PlayerPrefabs;

    // 시뮬레이션 속도 (로그 출력용)
    private bool _isSimulating = false;

    // 외부(UI 버튼 등)에서 호출하여 경기 시작
    public void StartSimulation()
    {
        if (_isSimulating) return;
        StartCoroutine(ProcessMatchRoutine());
    }

    private IEnumerator ProcessMatchRoutine()
    {
        _isSimulating = true;
        _matchState.AddLog("=== Simulation Start ===");

        // 4쿼터까지 반복
        while (_matchState.CurrentQuarter <= MatchState.MAX_QUARTER)
        {
            // 쿼터 시작 로그
            _matchState.AddLog($"--- Quarter {_matchState.CurrentQuarter} Start ---");

            // 한 쿼터의 시간이 다 될 때까지 루프
            while (_matchState.RemainTime > 0)
            {
                // 매 프레임(또는 턴마다) 시간과 점수 갱신
                _uiManager.UpdateScoreBoard(_matchState);

                // 공을 가진 팀과 선수 가져오기
                MatchTeam attackTeam = (_matchState.BallPossession == TeamSide.Home)
                                       ? _matchState.HomeTeam : _matchState.AwayTeam;
                MatchTeam defendTeam = (_matchState.BallPossession == TeamSide.Home)
                                       ? _matchState.AwayTeam : _matchState.HomeTeam;

                // 현재 공을 가진 선수 선정 로직 (일단 임의로 PG가 가졌다고 가정)
                MatchPlayer attacker = attackTeam.GetPlayerByPosition(Position.PG);
                MatchPlayer defender = defendTeam.GetPlayerByPosition(Position.PG); // 매치업 상대

                // 로그가 너무 빨리 지나가지 않게 딜레이 (선택 사항)
                yield return null;

                // [임시] 테스트를 위해 비주얼 오브젝트가 없으면 에러 안 나게 방어
                if (attacker.VisualObject == null || defender.VisualObject == null)
                {
                    Debug.LogWarning("선수 비주얼이 연결되지 않았습니다! (테스트 중)");
                    yield break; // 멈춤
                }

                //  함수 호출 앞에 'yield return StartCoroutine' 붙이기
                yield return StartCoroutine(SimulateActionRoutine(attackTeam, defendTeam, attacker, defender));
            }

            // 시간이 다 되면 쿼터 종료 처리 (State 내부에서 카운트 업)
            // 시간 강제로 0 이하로 만들어서 루프 탈출 확인
        }

        _isSimulating = false;
        _matchState.AddLog("=== Simulation Complete ===");
    }

    private IEnumerator SimulateActionRoutine(MatchTeam attackTeam, MatchTeam defendTeam, MatchPlayer attacker, MatchPlayer defender)
    {
        // 공을 공격수 위치로 순간이동 (잡고 시작)
        _ballTransform.position = attacker.VisualObject.transform.position;

        // 행동 (Logic)
        bool tryShoot = Random.value > 0.5f; // 나중에 전술 코드로 교체

        if (tryShoot)
        {

            float distance = Random.Range(0.0f, 1.0f);
            bool success = MatchCalculator.CalculateShootSuccess(attacker, defender, distance);

            _matchState.AddLog($"{attacker.PlayerName} Shoots!");

            // [연출] 공이 골대로 포물선을 그리며 날아감 (1.0초)
            // DOTween: Jump(목표점, 점프파워, 점프횟수, 시간)
            yield return _ballTransform.DOJump(_hoopTransform.position, 2.0f, 1, 1.0f)
                                       .WaitForCompletion(); // 도착할 때까지 대기

            if (success)
            {
                // 골인 연출 (예: 공이 림에서 튕기는 효과 등 추가 가능)
                _matchState.AddLog("Goal!!");
                attackTeam.AddScore(2); // 점수
                _matchState.SwitchPossession();
            }
            else
            {
                // 노골 (리바운드)
                _matchState.AddLog("Miss...");
                // 여기서 리바운드 연출 추가 가능
                ProcessRebound(attackTeam, defendTeam);
            }
            _matchState.DecreaseTime(15f);
        }
        else
        {
  
            bool success = MatchCalculator.CalculatePassSuccess(attacker, defender);

            // 패스 받을 동료 찾기 (임시로 센터가 받는다고 가정)
            MatchPlayer receiver = attackTeam.GetPlayerByPosition(Position.C);
            if (receiver == null) receiver = attacker; // 없으면 본인 (에러 방지)

            _matchState.AddLog($"{attacker.PlayerName} Passes to {receiver.PlayerName}");

            // [연출] 동료에게 직선으로 날아감 (0.5초)
            // DOTween: Move(목표점, 시간).SetEase(곡선타입)
            yield return _ballTransform.DOMove(receiver.VisualObject.transform.position, 0.5f)
                                       .SetEase(Ease.OutQuad)
                                       .WaitForCompletion();

            if (success)
            {
                // 패스 성공
            }
            else
            {
                // 스틸 당함
                _matchState.AddLog("Intercepted!");
                _matchState.SwitchPossession();
            }
            _matchState.DecreaseTime(3f);
        }

        // 행동 종료 후 잠시 대기 (너무 빠르면 정신없음)
        yield return new WaitForSeconds(0.2f);
    }

    private void ProcessRebound(MatchTeam attackTeam, MatchTeam defendTeam)
    {
        // 양팀 센터, 파워포워드들이 경합한다고 가정
        List<MatchPlayer> rebounders = new List<MatchPlayer>();

        // 공격팀 리바운더
        var attC = attackTeam.GetPlayerByPosition(Position.C);
        if (attC != null) rebounders.Add(attC);

        // 수비팀 리바운더
        var defC = defendTeam.GetPlayerByPosition(Position.C);
        if (defC != null) rebounders.Add(defC);

        // 승자 결정
        MatchPlayer winner = MatchCalculator.CalculateReboundWinner(rebounders);

        if (winner != null)
        {
            _matchState.AddLog($"Rebound! {winner.PlayerName} grabs the ball.");

            // 리바운드 잡은 팀이 공격권 가져감
            // 수비팀이 잡았으면 공수 교대, 공격팀이 잡았으면 공격권 유지
            bool isAttackerRebound = attackTeam.Roster.Contains(winner);

            if (!isAttackerRebound)
            {
                _matchState.SwitchPossession();
            }
        }
    }
}