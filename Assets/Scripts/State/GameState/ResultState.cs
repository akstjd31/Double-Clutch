using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class ResultState : IState
{
    private readonly StateMachine _sm;
    private readonly GameManager _gm;

    public ResultState(GameManager gm, StateMachine sm)
    {
        _sm = sm;
        _gm = gm;
    }

    public void Enter()
    {
        // 씬에 있는 데이터(MatchState)와 UI 관리자(MatchUIManager)를 찾습니다.
        MatchState matchState = Object.FindFirstObjectByType<MatchState>();
        MatchUIManager uiManager = Object.FindFirstObjectByType<MatchUIManager>();
        MatchEngine matchEngine = Object.FindFirstObjectByType<MatchEngine>();

        if (matchState == null || uiManager == null || matchEngine == null)
        {
            Debug.LogError("[ResultState] MatchState, matchEngine 또는 MatchUIManager를 찾을 수 없습니다.");
            return;
        }
        int currentMatchId = 1; // 임시 매치 ID (라운드 번호)
        // 리그 보관소에 현재 경기 기록 저장 
        if (LeagueRecordManager.Instance != null)
        {
            LeagueRecordManager.Instance.SaveMatchRecord(currentMatchId, matchState, matchEngine.FullMatchLogs);
        }


        // 경기 결과 및 기본 보상 산정
        bool isWin = matchState.HomeTeam.Score > matchState.AwayTeam.Score;

        // 추후 LeagueManager 구현 시 League_RewardDataReader에서 받아올 값들 (임시로 Swiss_01 값 적용)
        int rewardGoldEach = 100;           // 승리 시 경기당 지원금
        float rewardGoldMultiplier = 0.3f;  // 패배 시 지원금 배율
        
        // 아직 리그 미구현으로 주석처리
        //int rewardFameWin = 20;             // 리그 최종 우승 시 명성

        // 승패에 따른 기본 지급금 계산
        int baseGold = isWin ? rewardGoldEach : Mathf.RoundToInt(rewardGoldEach * rewardGoldMultiplier);

        // 인프라 보너스 산정
        float infraBonusPercent = 0f;
        // 추후 InfraManager 구현 시 프런트(RewardGoldBonus)의 infraEffectValue 값을 가져와 적용
        // 예: infraBonusPercent = InfraManager.Instance.GetEffectValue(infraEffectType.RewardGoldBonus) / 100f;


        // 현재 경기 뛰었던 팀(HomeTeam)의 선수 득점 기록을 List로 만들기
        List<MatchPlayerData> matchPlayers = new List<MatchPlayerData>();
        float passiveBonusPercent = 0f;
        // 홈 팀의 Roster 리스트를 순회하며 데이터 변환
        if (matchState.HomeTeam != null && matchState.HomeTeam.Roster != null)
        {
            foreach (var matchPlayer in matchState.HomeTeam.Roster)
            {
                matchPlayers.Add(new MatchPlayerData
                {
                    Position = matchPlayer.MainPosition.ToString(), // 선수의 주 포지션
                    Name = matchPlayer.PlayerName,                  // 선수 이름
                    Score = matchPlayer.Score                       // 선수의 득점
                });
                if (matchPlayer.PlayerId < 10000)
                {
                    Student realStudent = StudentManager.Instance.FindStudentById(matchPlayer.PlayerId);
                    if (realStudent != null)
                    {
                        // 경기 종료 후 컨디션 -30 차감
                        realStudent.ChangeCondition(-30);

                        // 경기 지원금 상승 패시브 (MatchGoldUp) 합산
                        foreach (var passive in realStudent.Passive)
                        {
                            // 현재 effectType Enum에 MatchGoldUp이 아직 없다면 문자열로 임시 체크
                            if (passive.effectType.ToString() == "MatchGoldUp")
                            {
                                passiveBonusPercent += passive.effectValue; // 예: 0.01 (1%), 0.04 (4%)
                            }
                        }
                    }
                }
            }
            // 변경된 컨디션 상태를 저장
            StudentManager.Instance.SaveGame();
        }
        // 최종 지원금 & 명성 지급
        // 계산식: 기본 지급금 * (1 + 인프라 프런트 % + 선수 패시브 %)
        float totalMultiplier = 1f + infraBonusPercent + passiveBonusPercent;
        int finalRewardAmount = Mathf.RoundToInt(baseGold * totalMultiplier);

        // 지원금 획득 (GameManager 안에서 Save까지 자동 진행됨)
        _gm.SetMoney(_gm.SaveData.money + finalRewardAmount);

        // 명성 획득 (기획서 상 매 경기가 아니라 리그 우승 시 지급)
        // 추후 리그 결승전 로직이 추가되면 아래 주석을 해제하여 사용
        /*
        bool isLeagueFinalMatch = false; // 리그 매니저에서 판별
        if (isWin && isLeagueFinalMatch)
        {
            _gm.SetHonor(_gm.SaveData.honor + rewardFameWin);
        }
        */

        // 결과창 UI 호출
        uiManager.ShowResultPopup(
            matchState.HomeTeam.TeamName,
            matchState.HomeTeam.Score,
            matchState.AwayTeam.TeamName,
            matchState.AwayTeam.Score,
            finalRewardAmount,
            matchPlayers,
            () =>
            {
                // MVP를 위해 ReturnToLobby() 대신 리그 결산 패널을 띄웁니다.
                uiManager.ShowLeagueCalculatePanel(currentMatchId, () => GoToLobby());
            }
         );
    }

    public void Exit()
    {
        // 상태를 빠져나갈 때 결과창을 닫거나 정리할 내용이 있다면 여기에 작성
    }

    public void Update() { }
    public void GoToLobby()
    {
        // 껍데기 데이터 저장
        var data = new StudentSaveData();
        SaveLoadManager.Instance.Save<StudentSaveData>(FilePath.MY_STUDENT_MATCHING_PATH, data);

        CalendarManager.Instance.NextTurn();
        
        // GameManager에 다음 씬(LOBBY)과 다음 상태(LobbyState)를 세팅
        _gm.SetNextFlow(SceneName.LOBBY, _sm.Get<LobbyState>());

        // 로딩 상태로 전환하여 자연스럽게 씬 이동 처리
        _sm.ChangeState<LoadingState>();
    }
}
