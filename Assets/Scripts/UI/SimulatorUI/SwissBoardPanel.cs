using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Linq;

public class SwissBoardPanel : MonoBehaviour
{
    [Header("Title UI")]
    [SerializeField] private TextMeshProUGUI _txtLeagueName; // {LeagueName} 들어갈 곳
    [SerializeField] private TextMeshProUGUI _txtRoundTitle; // N라운드 들어갈 곳

    [Header("Tabs UI")]
    [SerializeField] private Transform _tabContainer;
    [SerializeField] private SwissRoundTab _tabPrefab;
    private List<SwissRoundTab> _tabs = new List<SwissRoundTab>();

    [Header("Match List UI")]
    [SerializeField] private Transform _matchContainer;
    [SerializeField] private SwissMatchRow _matchRowPrefab;

    [Header("Bottom Actions")]
    [SerializeField] private Button _btnAction; // 경기 준비 or 닫기 버튼
    [SerializeField] private TextMeshProUGUI _txtBtnAction;

    private int _currentViewRoundIndex = 0;
    private Action _customAction;
    private string _customActionText;

    public void OpenPanel(Action onActionClick = null, string actionText = null)
    {
        gameObject.SetActive(true);
        _customAction = onActionClick;
        _customActionText = actionText;

        var currentLeague = LeagueManager.Instance.CurrentLeague;
        if (currentLeague == null) return;

        // 리그 이름 세팅
        var masterData = LeagueDataManager.Instance.GetMasterDataById(currentLeague.leagueId);
        if (_txtLeagueName != null && masterData.HasValue)
        {
            _txtLeagueName.text = StringManager.Instance.GetString(masterData.Value.leagueNameKey);
        }

        // 자동으로 현재 진행해야 할 라운드 탭으로 진입
        // 만약 리그가 완전히 끝났다면 마지막 라운드 탭으로 진입
        _currentViewRoundIndex = currentLeague.currentRoundIndex;

        // 탭 생성 및 초기화
        RefreshTabs();

        // 대진표 및 버튼 상태 갱신
        SelectTab(_currentViewRoundIndex);
    }

    private void RefreshTabs()
    {
        var currentLeague = LeagueManager.Instance.CurrentLeague;
        var masterData = LeagueDataManager.Instance.GetMasterDataById(currentLeague.leagueId);
        int totalRounds = masterData.Value.roundCount;

        // 기존 탭 삭제
        foreach (Transform child in _tabContainer) Destroy(child.gameObject);
        _tabs.Clear();

        // 탭 동적 생성
        for (int i = 0; i < totalRounds; i++)
        {
            SwissRoundTab newTab = Instantiate(_tabPrefab, _tabContainer);
            newTab.Init(i, currentLeague.currentRoundIndex, currentLeague.isFinished, SelectTab);
            _tabs.Add(newTab);
        }
        // 남은 공간을 채워줄 '투명 더미 탭' 생성 로직 
        // 무조건 6개의 탭이 있는 것처럼 Layout Group을 속여서 크기를 강제 고정합니다.
        int maxTabCount = 6;
        for (int i = totalRounds; i < maxTabCount; i++)
        {
            GameObject dummy = new GameObject("DummyTab");
            dummy.transform.SetParent(_tabContainer, false);

            // 더미가 기존 탭들과 완벽하게 동일한 비율(넓이)을 차지하도록 LayoutElement 추가
            var layoutElement = dummy.AddComponent<UnityEngine.UI.LayoutElement>();
            layoutElement.flexibleWidth = 1;
        }

    }

    private void SelectTab(int roundIndex)
    {
        _currentViewRoundIndex = roundIndex;

        // 라운드 타이틀 텍스트 갱신 (탭 누를 때마다 변경)
        if (_txtRoundTitle != null)
        {
            _txtRoundTitle.text = $"스위스 {roundIndex + 1}라운드 대진표";
        }

        // 탭 시각적 선택 상태 갱신
        for (int i = 0; i < _tabs.Count; i++)
        {
            _tabs[i].SetSelected(i == roundIndex);
        }

        // 대진표 리스트 갱신
        RefreshMatchList(roundIndex);

        // 하단 버튼 상태 갱신
        RefreshActionButton(roundIndex);
    }

    private void RefreshMatchList(int roundIndex)
    {
        var currentLeague = LeagueManager.Instance.CurrentLeague;

        foreach (Transform child in _matchContainer) Destroy(child.gameObject);

        // 해당 라운드의 매치들만 필터링
        List<LeagueMatchRecord> targetMatches = currentLeague.matchRecords.FindAll(m => m.roundIndex == roundIndex);

        // 현재 순위가 아닌 '해당 라운드 진입 당시'의 과거 순위를 역산합니다
        Dictionary<string, int> historyRankMap = GetHistoricalRanks(currentLeague, roundIndex);

        string myTeamId = StudentManager.TEAM_ID;

        // 내 팀이 포함된 매치만 찾아서 맨 위로 쏙 올려줍니다.
        int playerMatchIdx = targetMatches.FindIndex(m => m.homeTeamId == myTeamId || m.awayTeamId == myTeamId);
        if (playerMatchIdx > 0)
        {
            var pMatch = targetMatches[playerMatchIdx];
            targetMatches.RemoveAt(playerMatchIdx);
            targetMatches.Insert(0, pMatch);
        }

        // UI 생성
        foreach (var match in targetMatches)
        {
            string team1 = match.homeTeamId;
            string team2 = match.awayTeamId;

            // 플레이어 팀이 무조건 윗줄(1행)에 오도록 정렬
            if (team2 == myTeamId)
            {
                team1 = match.awayTeamId;
                team2 = match.homeTeamId;
            }
            else if (team1 != myTeamId)
            {
                // 플레이어 팀이 없는 일반 매치는 순위가 높은 팀이 윗줄
                int rank1 = historyRankMap.ContainsKey(team1) ? historyRankMap[team1] : 99;
                int rank2 = historyRankMap.ContainsKey(team2) ? historyRankMap[team2] : 99;
                if (rank2 < rank1)
                {
                    team1 = match.awayTeamId;
                    team2 = match.homeTeamId;
                }
            }

            // 첫 번째 줄 생성
            if (!string.IsNullOrEmpty(team1)) CreateRow(team1, match, roundIndex, historyRankMap);

            // 두 번째 줄 생성
            if (!string.IsNullOrEmpty(team2)) CreateRow(team2, match, roundIndex, historyRankMap);
        }
    }

    // 과거 라운드 당시의 랭킹을 똑같이 계산해 주는 역산 함수
    private Dictionary<string, int> GetHistoricalRanks(LeagueSaveData league, int upToRoundIndex)
    {
        Dictionary<string, LeagueStandingData> dict = new Dictionary<string, LeagueStandingData>();
        foreach (var t in league.teams)
        {
            dict[t.teamId] = new LeagueStandingData { teamId = t.teamId };
        }

        // 선택한 라운드 '이전'까지의 경기 결과만 누적해서 당시의 승패/득실차를 구함
        for (int i = 0; i < upToRoundIndex; i++)
        {
            var matches = league.matchRecords.FindAll(m => m.roundIndex == i && m.isPlayed);
            foreach (var m in matches)
            {
                if (!dict.ContainsKey(m.homeTeamId) || !dict.ContainsKey(m.awayTeamId)) continue;

                var home = dict[m.homeTeamId];
                var away = dict[m.awayTeamId];

                home.played++; away.played++;
                home.scored += m.homeScore; home.conceded += m.awayScore;
                away.scored += m.awayScore; away.conceded += m.homeScore;

                if (m.homeScore > m.awayScore) { home.win++; away.lose++; home.points += 3; }
                else if (m.homeScore < m.awayScore) { away.win++; home.lose++; away.points += 3; }
            }
        }

        foreach (var s in dict.Values) s.goalDiff = s.scored - s.conceded;

        var list = dict.Values.ToList();

        // 당시 기록을 바탕으로 타이브레이커 정렬
        list.Sort((a, b) =>
        {
            if (b.win != a.win) return b.win.CompareTo(a.win);
            if (b.goalDiff != a.goalDiff) return b.goalDiff.CompareTo(a.goalDiff);
            return string.Compare(a.teamId, b.teamId, StringComparison.Ordinal);
        });

        // 랭킹 부여
        Dictionary<string, int> ranks = new Dictionary<string, int>();
        for (int i = 0; i < list.Count; i++) ranks[list[i].teamId] = i + 1;

        return ranks;
    }

    // 한 줄(Row)을 생성하는 함수
    private void CreateRow(string teamId, LeagueMatchRecord match, int viewRoundIndex, Dictionary<string, int> rankMap)
    {
        SwissMatchRow row = Instantiate(_matchRowPrefab, _matchContainer);
        bool isMyTeam = (teamId == StudentManager.TEAM_ID);
        int rank = rankMap.ContainsKey(teamId) ? rankMap[teamId] : 0;
        var record = GetCumulativeRecord(teamId, viewRoundIndex);

        string scoreStr = "-";
        if (match.isPlayed)
        {
            scoreStr = (teamId == match.homeTeamId) ? match.homeScore.ToString() : match.awayScore.ToString();
        }

        row.Init(teamId, rank, record.win, record.lose, scoreStr, isMyTeam);
    }

    // 누적 승패 역산
    private (int win, int lose) GetCumulativeRecord(string teamId, int upToRoundIndex)
    {
        int w = 0, l = 0;
        var league = LeagueManager.Instance.CurrentLeague;

        for (int i = 0; i < upToRoundIndex; i++)
        {
            var match = league.matchRecords.Find(m => m.roundIndex == i && (m.homeTeamId == teamId || m.awayTeamId == teamId));
            if (match != null && match.isPlayed)
            {
                int myScore = match.homeTeamId == teamId ? match.homeScore : match.awayScore;
                int opScore = match.homeTeamId == teamId ? match.awayScore : match.homeScore;

                if (myScore > opScore) w++;
                else if (myScore < opScore) l++;
            }
        }
        return (w, l);
    }

    private void RefreshActionButton(int roundIndex)
    {
        var currentLeague = LeagueManager.Instance.CurrentLeague;
        _btnAction.onClick.RemoveAllListeners();

        // 버튼 텍스트 세팅 수정
        _txtBtnAction.text = string.IsNullOrEmpty(_customActionText) ? (currentLeague.isFinished ? "닫기" : "경기 준비") : _customActionText;

        // 리그가 완전히 종료된 상태 (수정: _customAction.Invoke() 추가)
        if (currentLeague.isFinished)
        {
            _btnAction.interactable = true;
            _btnAction.onClick.AddListener(() =>
            {
                gameObject.SetActive(false);
                if (_customAction != null) _customAction.Invoke();
            });
            return;
        }

        // 내가 봐야 하는 현재 라운드 탭을 보고 있을 때만 터치 활성화 (수정: _customAction 처리 추가)
        if (roundIndex == currentLeague.currentRoundIndex)
        {
            _btnAction.interactable = true;
            _btnAction.onClick.AddListener(() =>
            {
                gameObject.SetActive(false);
                if (_customAction != null)
                {
                    _customAction.Invoke();
                }
                else
                {
                    GameManager.Instance.ChangeState<MatchPrepState>();
                }
            });
        }
        else
        {
            _btnAction.interactable = false;
        }
    }
}
