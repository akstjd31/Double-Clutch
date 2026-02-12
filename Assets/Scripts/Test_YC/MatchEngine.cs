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


            //  2쿼터 종료 후 하프타임 이벤트 로직

            if (_matchState.CurrentQuarter == 3)
            {
                _matchState.AddLog("--- Half Time (Event Phase) ---");

                if (_uiManager != null)
                {
                    // 이벤트 패널 띄우기
                    _uiManager.ShowHalfTimeEvent();

                    // 유저가 버튼을 누를 때까지 시뮬레이션 무한 대기
                    yield return new WaitUntil(() => _uiManager.IsEventSelected);

                    // 선택된 효과 적용 (껍데기 함수 호출)
                    _matchState.ApplyHalfTimeEffect(_uiManager.SelectedEventIndex);
                }

                // 이벤트 후 잠시 대기 후 3쿼터 시작
                yield return new WaitForSeconds(1.0f);
            }
        }

        _isSimulating = false;
        _matchState.AddLog("=== Simulation Complete ===");
        // 경기 끝! 1초 대기
        yield return new WaitForSeconds(1.0f);

        // 결과 팝업 띄우기
        if (_uiManager != null)
        {
            _uiManager.ShowResultPopup(_matchState.HomeTeam.Score, _matchState.AwayTeam.Score);
        }
    }

    private IEnumerator SimulateActionRoutine(MatchTeam attackTeam, MatchTeam defendTeam, MatchPlayer attacker, MatchPlayer defender)
    {
        // 공 잡음 (순간이동)
        _ballTransform.position = attacker.VisualObject.transform.position;

        TeamTactics tactics = MatchDataProxy.GetTactics(attackTeam.TeamColorId);
        // 전술 가중치 기반 행동 결정 로직
        // 가중치 계산

        float weightShoot = tactics.w2PT + tactics.w3PT;
        float weightPass = tactics.wPass;
        float weightDribble = 1.0f; // (기획서 테이블에 드리블 가중치가 없으므로 기본값 1.0 고정)

        float totalWeight = weightShoot + weightPass + weightDribble;
        float randomPoint = Random.Range(0f, totalWeight);

        // 가중치 주사위 판정
        // 슛 구간
        if (randomPoint <= weightShoot)
        {
            yield return StartCoroutine(ProcessShoot(attacker, defender, attackTeam, defendTeam));
        }
        // 드리블 구간 (슛 구간 다음)
        else if (randomPoint <= weightShoot + weightDribble)
        {
            _matchState.AddLog($"{attacker.PlayerName} attempts to Drive!");

            bool isSuccess = MatchCalculator.CalculateDribbleSuccess(attacker, defender);

            // [연출] 돌파 모션
            Vector3 targetPos = defender.VisualObject.transform.position + (Vector3.up * 1.5f);
            yield return _ballTransform.DOMove(targetPos, 0.3f).SetEase(Ease.OutCubic).WaitForCompletion();

            if (isSuccess)
            {
                _matchState.AddLog("Breakthrough Successful!");
                yield return StartCoroutine(ProcessShoot(attacker, null, attackTeam, defendTeam));
            }
            else
            {
                _matchState.AddLog("Blocked by Defender!");
                _matchState.SwitchPossession();
            }
            _matchState.DecreaseTime(5f);
        }
        // 패스 구간 (나머지)
        else
        {
            yield return StartCoroutine(ProcessPass(attacker, defender, attackTeam));
        }

        yield return new WaitForSeconds(0.2f);
    }
    //  슛 거리도 전술(3점 선호도)에 따라 결정하도록 변경
    private IEnumerator ProcessShoot(MatchPlayer attacker, MatchPlayer defender, MatchTeam attackTeam, MatchTeam defendTeam)
    {
        _matchState.IsBallInAir = true;

        TeamTactics tactics = MatchDataProxy.GetTactics(attackTeam.TeamColorId);

        // 3점슛 선호 확률 계산: w3PT / (w2PT + w3PT)
        float threePointChance = 0f;
        if (tactics.w2PT + tactics.w3PT > 0)
            threePointChance = tactics.w3PT / (tactics.w2PT + tactics.w3PT);

        float distance = 0f;

        // 확률에 따라 거리 설정 (전술 반영)
        if (Random.value < threePointChance)
        {
            // 3점슛 시도 (3점 라인 0.35 ~ 1.0 사이 랜덤)
            distance = Random.Range(MatchDataProxy.GetBalance("DIST_3POINT_LINE"), 1.0f);
        }
        else
        {
            // 2점슛 시도 (골밑 0.0 ~ 3점 라인 0.35 사이 랜덤)
            distance = Random.Range(0.0f, MatchDataProxy.GetBalance("DIST_3POINT_LINE") - 0.01f);
        }

        // 거리 기준값 가져오기
        float threePointLine = MatchDataProxy.GetBalance("DIST_3POINT_LINE"); // 0.35f
        float dunkRange = MatchDataProxy.GetBalance("DIST_DUNK_RANGE");       // 0.05f

        // 슛 타입 판별
        bool isThreePoint = distance >= threePointLine;
        bool isDunk = distance <= dunkRange;
        int scoreToAdd = isThreePoint ? 3 : 2; // 3점슛이면 3점, 아니면 2점

        // 성공 여부 계산
        bool success = MatchCalculator.CalculateShootSuccess(attacker, defender, distance);

        // 시도 로그 출력 (슛 종류에 따라 다르게)
        if (isDunk)
        {
            _matchState.AddLog($"<color=red><b>{attacker.PlayerName} attempts a SLAM DUNK!</b></color>"); // 강조
        }
        else if (isThreePoint)
        {
            _matchState.AddLog($"{attacker.PlayerName} shoots from 3-Point line!");
        }
        else
        {
            _matchState.AddLog($"{attacker.PlayerName} shoots a Mid-range jumper.");
        }

        // 연출 (DOTween)
        if (isDunk)
        {
            // 덩크는 골대로 아주 빠르게 돌진!
            yield return _ballTransform.DOMove(_hoopTransform.position, 0.3f).SetEase(Ease.InExpo).WaitForCompletion();
        }
        else
        {
            // 일반 슛은 포물선
            yield return _ballTransform.DOJump(_hoopTransform.position, 2.0f, 1, 1.0f).WaitForCompletion();
        }
        _matchState.IsBallInAir = false;

        // 결과 처리
        if (success)
        {
            _matchState.AddLog("Goal!!");
            attackTeam.AddScore(scoreToAdd); // [핵심] 3점 혹은 2점 추가
            _matchState.SwitchPossession();
            // 버저비터
            if (_matchState.RemainTime <= 0)
            {
                _matchState.AddLog("<color=red><b>BUZZER BEATER!!!</b></color>");
                // (기획서에 버저비터 전용 연출/컷신이 있다고 했으니 나중에 여기에 연결)
            }
        }
        else
        {
            _matchState.AddLog("Miss...");

            // 덩크 실패는 튕겨나가는 연출을 다르게 할 수도 있음 (지금은 공통 처리)
            ProcessRebound(attackTeam, defendTeam);
        }
        _matchState.DecreaseTime(15f);
    }

    private IEnumerator ProcessPass(MatchPlayer attacker, MatchPlayer defender, MatchTeam attackTeam)
    {
        bool success = MatchCalculator.CalculatePassSuccess(attacker, defender);
        MatchPlayer receiver = attackTeam.GetPlayerByPosition(Position.C); // 임시 타겟
        if (receiver == null || receiver == attacker) receiver = attackTeam.Roster[Random.Range(0, attackTeam.Roster.Count)];

        _matchState.AddLog($"{attacker.PlayerName} Passes to {receiver.PlayerName}");

        // 패스 연출 (이동)
        yield return _ballTransform.DOMove(receiver.VisualObject.transform.position, 0.5f).SetEase(Ease.OutQuad).WaitForCompletion();

        if (success)
        {
            // 패스 성공 (다음 턴에 받은 사람이 공격)
        }
        else
        {
            _matchState.AddLog("Intercepted!");
            _matchState.SwitchPossession();
        }
        _matchState.DecreaseTime(3f);
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
