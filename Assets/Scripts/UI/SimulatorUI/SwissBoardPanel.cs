using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

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

        // 기존 대진표 삭제
        foreach (Transform child in _matchContainer) Destroy(child.gameObject);

        // 해당 라운드의 매치들만 필터링
        List<LeagueMatchRecord> targetMatches = currentLeague.matchRecords.FindAll(m => m.roundIndex == roundIndex);

        // 랭킹 정보를 빠르게 찾기 위한 딕셔너리 생성
        Dictionary<string, int> rankMap = new Dictionary<string, int>();
        if (currentLeague.standings != null)
        {
            foreach (var standing in currentLeague.standings)
            {
                rankMap[standing.teamId] = standing.rank; // 1위, 2위...
            }
        }

        // 플레이어 팀을 항상 최상단에 노출, 나머지는 랭킹순 정렬
        string myTeamId = StudentManager.TEAM_ID;
        targetMatches.Sort((a, b) =>
        {
            bool aHasPlayer = (a.homeTeamId == myTeamId || a.awayTeamId == myTeamId);
            bool bHasPlayer = (b.homeTeamId == myTeamId || b.awayTeamId == myTeamId);

            // 플레이어 팀이 포함된 매치를 무조건 최상단으로
            if (aHasPlayer && !bHasPlayer) return -1;
            if (!aHasPlayer && bHasPlayer) return 1;

            // 순위(랭킹)가 높은 팀이 포함된 매치부터 차례로 출력
            // ( awayTeamId가 비어있을 경우 순위를 낮게(99) 깔아버립니다.)
            int aRank = Mathf.Min(
                rankMap.ContainsKey(a.homeTeamId) ? rankMap[a.homeTeamId] : 99,
                string.IsNullOrEmpty(a.awayTeamId) ? 99 : (rankMap.ContainsKey(a.awayTeamId) ? rankMap[a.awayTeamId] : 99)
            );
            int bRank = Mathf.Min(
                rankMap.ContainsKey(b.homeTeamId) ? rankMap[b.homeTeamId] : 99,
                string.IsNullOrEmpty(b.awayTeamId) ? 99 : (rankMap.ContainsKey(b.awayTeamId) ? rankMap[b.awayTeamId] : 99)
            );

            return aRank.CompareTo(bRank);
        });

        // UI 생성
        foreach (var match in targetMatches)
        {
            string team1 = match.homeTeamId;
            string team2 = match.awayTeamId;

            // 플레이어 팀이 무조건 윗줄(1행)에 오도록 정렬, 나머지는 순위 높은 팀이 윗줄
            if (team2 == myTeamId)
            {
                team1 = match.awayTeamId;
                team2 = match.homeTeamId;
            }
            else if (team1 != myTeamId)
            {
                int rank1 = rankMap.ContainsKey(team1) ? rankMap[team1] : 99;
                int rank2 = rankMap.ContainsKey(team2) ? rankMap[team2] : 99;
                if (rank2 < rank1)
                {
                    team1 = match.awayTeamId;
                    team2 = match.homeTeamId;
                }
            }

            // 첫 번째 줄 생성 (예: 6위 플레이어팀)
            if (!string.IsNullOrEmpty(team1)) CreateRow(team1, match, roundIndex, rankMap);

            // 두 번째 줄 생성 (예: 5위 상대팀)
            if (!string.IsNullOrEmpty(team2)) CreateRow(team2, match, roundIndex, rankMap);
        }
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

        // 리그가 완전히 종료된 상태
        if (currentLeague.isFinished)
        {
            _txtBtnAction.text = "닫기";
            _btnAction.interactable = true;
            _btnAction.onClick.AddListener(() => gameObject.SetActive(false));
            return;
        }

        // 리그 진행 중
        _txtBtnAction.text = string.IsNullOrEmpty(_customActionText) ? "경기 준비" : _customActionText;

        // 내가 봐야 하는 현재 라운드 탭을 보고 있을 때만 터치 활성화
        if (roundIndex == currentLeague.currentRoundIndex)
        {
            _btnAction.interactable = true;
            _btnAction.onClick.AddListener(() =>
            {
                gameObject.SetActive(false);
                // 외부에서 넘겨준 커스텀 행동(로비 이동 등)이 있으면 실행, 없으면 매치 배치로 이동
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
            // 과거나 미래 탭을 보고 있으면 경기 준비 비활성화 (흐리게 처리)
            _btnAction.interactable = false;
        }
    }
}
