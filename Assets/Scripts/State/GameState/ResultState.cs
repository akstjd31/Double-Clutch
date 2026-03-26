using System;
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
        MatchState matchState = UnityEngine.Object.FindFirstObjectByType<MatchState>();
        MatchUIManager uiManager = UnityEngine.Object.FindFirstObjectByType<MatchUIManager>();
        MatchEngine matchEngine = UnityEngine.Object.FindFirstObjectByType<MatchEngine>();
        HeadlessMatchSimulator headlessSim = UnityEngine.Object.FindFirstObjectByType<HeadlessMatchSimulator>();

        if (matchState == null || uiManager == null || matchEngine == null)
        {
            Debug.LogError("[ResultState] MatchState, matchEngine 또는 MatchUIManager를 찾을 수 없습니다.");
            return;
        }
        int currentMatchId = 1; // 기본값 (리그가 진행 중이 아닐 경우 대비)
        var currentLeague = LeagueManager.Instance.CurrentLeague;
        string myTeamId = StudentManager.TEAM_ID;

        // 리그 마스터 및 보상 데이터 보관용 변수
        League_MasterData? masterData = null;
        League_RewardData? rewardData = null;

        if (currentLeague != null && !currentLeague.isFinished)
        {
            // 인덱스는 0부터 시작하므로 +1 처리 (1라운드, 2라운드...)
            currentMatchId = currentLeague.currentRoundIndex + 1;
            masterData = LeagueDataManager.Instance.GetMasterDataById(currentLeague.leagueId);

            if (masterData != null)
            {
                // 리그 보상 데이터 가져오기
                rewardData = LeagueDataManager.Instance.GetLeagueRewardDataById(masterData.Value.leagueRewardId);
            }

            // 유저 경기 결과 기록
            var myMatch = currentLeague.matchRecords.Find(m =>
                m.roundIndex == currentLeague.currentRoundIndex &&
                (m.homeTeamId == myTeamId || m.awayTeamId == myTeamId));

            if (myMatch != null && !myMatch.isPlayed)
            {
                // 대진표 상에서 내가 홈인지 어웨이인지 판별
                bool amIHomeInLeague = (myMatch.homeTeamId == myTeamId);

                // 시뮬레이터(유저가 Home)의 점수를 대진표 위치에 맞게 재배치
                int reportHomeScore = amIHomeInLeague ? matchState.HomeTeam.Score : matchState.AwayTeam.Score;
                int reportAwayScore = amIHomeInLeague ? matchState.AwayTeam.Score : matchState.HomeTeam.Score;

                LeagueManager.Instance.CompleteMatch(myMatch, reportHomeScore, reportAwayScore);
            }

            // NPC 경기 백그라운드 연산 진행
            if (headlessSim != null && masterData != null)
            {
                var unplayedMatches = currentLeague.matchRecords.FindAll(m =>
                    m.roundIndex == currentLeague.currentRoundIndex && !m.isPlayed);

                string levelId = masterData.Value.leagueLevelId;

                foreach (var match in unplayedMatches)
                {
                    if (match.specialNote == "BYE")
                    {
                        LeagueManager.Instance.CompleteMatch(match, 1, 0, "BYE");
                        continue;
                    }

                    // NPC 팀 생성
                    Team homeData = LeagueTeamManager.Instance.GetTeamById(match.homeTeamId);
                    Team awayData = LeagueTeamManager.Instance.GetTeamById(match.awayTeamId);

                    MatchTeam npcHome = EnemyTeamFactory.Instance.ConvertToTeam(TeamSide.Home, homeData);
                    MatchTeam npcAway = EnemyTeamFactory.Instance.ConvertToTeam(TeamSide.Away, awayData);

                    // 시뮬레이터로 결과 도출 후 기록
                    if (npcHome != null && npcAway != null)
                    {
                        var result = headlessSim.SimulateNPCMatch(npcHome, npcAway);
                        LeagueManager.Instance.CompleteMatch(match, result.homeScore, result.awayScore);
                    }
                }
            }
            else
            {
                Debug.LogError("[ResultState] HeadlessMatchSimulator를 찾을 수 없어 NPC 경기를 진행할 수 없습니다.");
            }

            // 라운드 종료 처리 (다음 라운드 대진표 생성 또는 리그 종료 처리 됨)
            LeagueManager.Instance.EndRound();
        }

        // 리그 보관소에 현재 경기 기록 저장 
        if (LeagueRecordManager.Instance != null)
        {
            LeagueRecordManager.Instance.SaveMatchRecord(currentMatchId, matchState, matchEngine.FullMatchLogs);
        }


        // 경기 결과 및 기본 보상 산정
        bool isWin = matchState.HomeTeam.Score > matchState.AwayTeam.Score;

        // 실제 보상 데이터가 없으면 기본값 세팅 (에러 방지 - HasValue 체크 추가!)
        int rewardGoldEach = 0;
        float rewardGoldMultiplier = 0f;
        int rewardFameWin = 0;
        int finalParticipationFame = 0;

        if (rewardData.HasValue)
        {
            rewardGoldEach = rewardData.Value.rewardGoldEach;                // 승리 시 경기당 지원금
            rewardGoldMultiplier = rewardData.Value.rewardGoldMultiplier;  // 패배 시 지원금 배율
            rewardFameWin = rewardData.Value.rewardFameWin;                      // 리그 최종 우승 시 명성
            finalParticipationFame = rewardData.Value.finalParticipationFame;   // 결승 출전 시 선수별 명성 누적 값
        }
        else
        {
            Debug.LogWarning("[ResultState] 보상 데이터가 없습니다! 리그 데이터를 정상적으로 불러오지 못했거나 이미 종료되었습니다.");
        }

        // 승패에 따른 기본 지급금 계산
        int baseGold = isWin ? rewardGoldEach : Mathf.RoundToInt(rewardGoldEach * rewardGoldMultiplier);

        // 인프라 보너스 산정
        float infraBonusPercent = 0f;

        if (InfraManager.Instance != null)
            infraBonusPercent = InfraManager.Instance.GetInfraEffectValueByEffectType(infraEffectType.RewardGoldBonus);

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
                    Name = MakeName(matchPlayer.PlayerName),        // 선수 이름
                    Score = matchPlayer.Score                       // 선수의 득점
                });
                if (matchPlayer.PlayerId < 10000)
                {
                    Student realStudent = StudentManager.Instance.FindStudentById(matchPlayer.PlayerId);
                    if (realStudent != null)
                    {

                        // 경기 지원금 상승 패시브 (MatchGoldUp) 합산
                        foreach (var passive in realStudent.Passive)
                        {
                            // 현재 effectType Enum에 MatchGoldUp이 아직 없다면 문자열로 임시 체크
                            if (passive.effectType == effectType.MatchGoldUp)
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
        float infraBonus = baseGold * (infraBonusPercent / 100f);
        float totalMultiplier = 1f + passiveBonusPercent;
        int finalRewardAmount = Mathf.FloorToInt((baseGold + infraBonus) * totalMultiplier);

        // 결승 진출 시 선수들의 명성 누적값 적용
        if (currentLeague != null && currentLeague.teams.Count < 3 && StudentManager.Instance != null)
        {
            foreach (var std in StudentManager.Instance.CurrentTeam.Members)
            {
                std.AddFame(finalParticipationFame);
            }
        }

        if (currentLeague != null && currentLeague.isFinished && rewardData != null)
        {
            // 순위표에서 내 팀 찾기
            var myStanding = currentLeague.standings.Find(s => s.teamId == myTeamId);

            // 내가 1등(우승)이라면
            if (myStanding != null && myStanding.rank == 1)
            {
                // 우승 상금을 최종 획득 골드에 합산
                finalRewardAmount += rewardData.Value.rewardGoldWin;

                // 리그 우승 명성치 누적
                _gm.SetTotalWinHonor(_gm.SaveData.totalWinHonor + rewardFameWin);

                Debug.Log($"[리그 우승!] 상금 {rewardData.Value.rewardGoldWin}G 및 명성 {rewardData.Value.rewardFameWin} 획득!");
            }
            
            _gm.SaveData.leagueWinRecord.Add(new LeagueWinRecord(currentLeague.leagueId, myStanding.rank == 1));
        }

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
                if (currentLeague != null)
                {
                    // W2, L1, L2 판별 (리그가 완전히 끝났거나, 우리 팀이 져서 탈락했거나)
                    bool isTournamentEndForMe = currentLeague.isFinished || currentLeague.isPlayerEliminated;

                    // 상태에 따른 버튼 텍스트와 다음 액션 정의
                    string btnText = isTournamentEndForMe ? "결산 확인" : "경기 준비";
                    Action nextAction = () =>
                    {
                        if (isTournamentEndForMe)
                        {
                            // 결산창으로 이동
                            uiManager.ShowLeagueCalculatePanel(currentMatchId, () => GoToLobby());
                        }
                        else
                        {
                            // 다음 경기 준비로 이동
                            PlayerPrefs.SetInt(PrefKeys.MATCH_PREP_UI_INDEX, 1);
                            PlayerPrefs.Save();
                            _gm.ChangeState<MatchPrepState>();
                        }
                    };

                    // 리그 타입에 따라 대진표 띄우며 텍스트와 액션 전달
                    if (currentLeague.leagueType == "Tournament")
                    {
                        uiManager.ShowTournamentBoardPanel(nextAction, btnText);
                    }
                    else
                    {
                        uiManager.ShowSwissBoardPanel(nextAction, btnText);
                    }
                }
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
        if (LeagueManager.Instance.CurrentLeague != null && LeagueManager.Instance.CurrentLeague.isFinished)
        {
            ApplyLeagueEndConditionDrop(LeagueManager.Instance.CurrentLeague.currentRoundIndex + 1);
            if (LeagueRecordManager.Instance != null)
            {
                LeagueRecordManager.Instance.ClearLeagueRecords();
            }
        }
        MatchState matchState = UnityEngine.Object.FindFirstObjectByType<MatchState>();
        if (matchState != null && matchState.HomeTeam != null)
        {
            foreach (var matchPlayer in matchState.HomeTeam.Roster)
            {
                if (matchPlayer.PlayerId < 10000) // 실제 유저의 학생인 경우만
                {
                    Student realStudent = StudentManager.Instance.FindStudentById(matchPlayer.PlayerId);
                    if (realStudent != null) realStudent.SetMatchPosition(Position.None); // 임시 포지션 초기화
                }
            }
        }
        // 껍데기 데이터 저장
        var data = new StudentSaveData();
        SaveLoadManager.Instance.Save<StudentSaveData>(FilePath.MY_STUDENT_MATCHING_PATH, data);

        CalendarManager.Instance.NextTurn();
        
        // GameManager에 다음 씬(LOBBY)과 다음 상태(LobbyState)를 세팅
        _gm.SetNextFlow(SceneName.LOBBY, _sm.Get<LobbyState>());

        // 로딩 상태로 전환하여 자연스럽게 씬 이동 처리
        _sm.ChangeState<LoadingState>();
    }
    private string MakeName(string[] nameKey)
    {
        if (nameKey == null || nameKey.Length < 3) return null;
        StringManager manager = StringManager.Instance;
        string name = manager.GetString(nameKey[0]) + manager.GetString(nameKey[1]) + manager.GetString(nameKey[2]);
        return name;
    }
    public void ApplyLeagueEndConditionDrop(int totalRounds)
    {
        // 중복 방지를 위한 HashSet
        HashSet<int> participatedIds = new HashSet<int>();

        // 리그 전체 기록을 뒤져서 1번이라도 출전한 '학생'의 ID만 긁어모음
        for (int i = 1; i <= totalRounds; i++)
        {
            var record = LeagueRecordManager.Instance.GetMatchRecord(i);
            if (record != null && record.HomePlayerIds != null)
            {
                foreach (int id in record.HomePlayerIds)
                {
                    // 용병(10000번 이상)은 제외하고 학생만 등록
                    if (id < 10000)
                    {
                        participatedIds.Add(id);
                    }
                }
            }
        }

        // 출전 학생들에게만 순서대로 로직 적용
        foreach (int id in participatedIds)
        {
            Student realStudent = StudentManager.Instance.FindStudentById(id);
            if (realStudent != null)
            {
                // 컨디션 깎기전에 현재 컨디션이 0인지 먼저 판별
                if (realStudent.Condition <= 0 && realStudent.State == StudentState.None)
                {
                    int rate = UnityEngine.Random.Range(0, 100);
                    if (rate < 60)
                    {
                        realStudent.ChangeState(StudentState.OverWorked);
                        Debug.Log($"[리그 종료 후유증] {realStudent.Name[0]} 학생이 과로 상태가 되었습니다.");
                    }
                    else
                    {
                        realStudent.ChangeState(StudentState.Injured);
                        Debug.Log($"[리그 종료 후유증] {realStudent.Name[0]} 학생이 부상 상태가 되었습니다.");
                    }
                }

                // 판정이 모두 끝난 뒤에 출전 페널티 -30 적용
                realStudent.ChangeCondition(-30);
            }
        }

        // 변경된 상태 일괄 저장
        StudentManager.Instance.SaveGame();
    }
}
