using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class LeagueCalculatePanel : MonoBehaviour
{
    [SerializeField] private Button _btnConfirm;
    [SerializeField] private Button _btnTotalRank;

    // 대진표 확인 버튼과 대진표 패널 연결
    [SerializeField] private Button _btnShowBracket;
    [SerializeField] private SwissBoardPanel _swissBoardPanel;

    [Header("결산 텍스트 UI")]
    [SerializeField] private TextMeshProUGUI _txtLeagueName;     // 리그 이름
    [SerializeField] private TextMeshProUGUI _txtWinnerTeamName; // (예: XX 고등학교)
    [SerializeField] private TextMeshProUGUI _txtMyTeamRank;     // (예: XX 고등학교 순위 :\n1 위)

    [Header("보상 텍스트 UI (값만 출력)")]
    [SerializeField] private TextMeshProUGUI _txtMatchReward;     // 경기 승패 보상 종합 (예: + 400G)
    [SerializeField] private TextMeshProUGUI _txtInfraBonus;      // 인프라 보너스 (예: + 0G)
    [SerializeField] private TextMeshProUGUI _txtSubtotalReward;  // 소계
    [SerializeField] private TextMeshProUGUI _txtLeagueWinReward; // 리그 우승 보상 (예: + 500G)
    [SerializeField] private TextMeshProUGUI _txtTotalReward;     // 합계 (예: + 900G)

    [Header("History List UI")]
    [SerializeField] private Transform _historyContainer;       // 대진 이력이 생성될 부모 (Scroll View의 Content)
    [SerializeField] private MatchHistoryRow _historyRowPrefab; // 대진 이력 프리팹

    [Header("Panels")]
    [SerializeField] private TotalRankPanel _totalRankPanel;
    [SerializeField] private LogHistoryPanel _logHistoryPanel;

    private Action _onConfirmAction;

    public void Init(int currentRound, Action onConfirm)
    {
        _onConfirmAction = onConfirm;

        // 확인 및 전체 순위표 버튼 설정
        _btnConfirm.onClick.RemoveAllListeners();
        _btnConfirm.onClick.AddListener(() =>
        {
            gameObject.SetActive(false);
            _onConfirmAction?.Invoke();
        });

        if (_btnTotalRank != null)
        {
            _btnTotalRank.onClick.RemoveAllListeners();
            _btnTotalRank.onClick.AddListener(() => _totalRankPanel.OpenPanel());
        }
        // 대회 대진표 버튼 클릭 이벤트
        if (_btnShowBracket != null)
        {
            _btnShowBracket.onClick.RemoveAllListeners();
            _btnShowBracket.onClick.AddListener(() =>
            {
                if (_swissBoardPanel != null) _swissBoardPanel.OpenPanel();
            });
        }
        // 데이터 계산 및 텍스트 적용
        UpdateCalculateData();

        // 창이 열릴 때 대진 이력 리스트를 생성합니다.
        CreateHistoryList(currentRound);
    }
    private void UpdateCalculateData()
    {
        var league = LeagueManager.Instance.CurrentLeague;
        if (league == null || league.standings == null || league.standings.Count == 0) return;

        // 데이터 로드
        var masterData = LeagueDataManager.Instance.GetMasterDataById(league.leagueId);
        if (!masterData.HasValue) return;

        var rewardData = LeagueDataManager.Instance.GetLeagueRewardDataById(masterData.Value.leagueRewardId);
        if (!rewardData.HasValue) return;

        if (_txtLeagueName != null)
        {
            string leagueName = StringManager.Instance.GetString(masterData.Value.leagueNameKey);

            // 다국어 연동용 (CSV에 "Str_League_Calc_Title"이 없으면 기본값 사용)
            string titleFormat = StringManager.Instance.GetString("Str_League_Calc_Title");
            if (titleFormat == "Str_League_Calc_Title")
            {
                titleFormat = "{0} 리그 결산";
            }
            _txtLeagueName.text = string.Format(titleFormat, leagueName);
        }

        // 우승팀 텍스트 출력
        var firstPlace = league.standings[0];
        string winnerName = GetTeamName(firstPlace.teamId);

        if (_txtWinnerTeamName != null)
        {
            _txtWinnerTeamName.text = winnerName;
        }

        // 우리 팀 순위
        string myTeamId = StudentManager.TEAM_ID;
        var myStanding = league.standings.Find(s => s.teamId == myTeamId);
        string myTeamName = GameManager.Instance.SaveData.schoolName;
        int myRank = myStanding != null ? myStanding.rank : 99;

        if (_txtMyTeamRank != null)
        {
            // 다국어 연동용 (CSV에 "Str_League_MyRank"가 없으면 기본값 사용)
            string rankFormat = StringManager.Instance.GetString("Str_League_MyRank");
            if (rankFormat == "Str_League_MyRank")
            {
                // {0} = 팀명, {1} = 순위
                rankFormat = "{0} 순위 :\n{1} 위";
            }
            _txtMyTeamRank.text = string.Format(rankFormat, myTeamName, myRank);
        }

        // 보상 금액 계산
        if (myStanding != null)
        {
            // 경기 승패 보상금
            int matchReward = (myStanding.win * rewardData.Value.rewardGoldEach) +
                              Mathf.RoundToInt(myStanding.lose * rewardData.Value.rewardGoldEach * rewardData.Value.rewardGoldMultiplier);

            // 인프라 보너스
            float infraPercent = 0f;
            if (InfraManager.Instance != null)
            {
                infraPercent = InfraManager.Instance.GetInfraEffectValueByEffectType(infraEffectType.RewardGoldBonus) / 100f;
            }
            int infraBonus = Mathf.RoundToInt(matchReward * infraPercent);

            // 리그 우승 보상
            int leagueWinReward = (myRank == 1) ? rewardData.Value.rewardGoldWin : 0;

            // 소계
            int subtotal = matchReward + leagueWinReward;

            // 총합계
            int totalReward = matchReward + infraBonus + leagueWinReward;

            // UI 텍스트 적용 (오른쪽 값 영역에 "+ 000G" 형태로 출력)
            if (_txtMatchReward != null) _txtMatchReward.text = $"+ {matchReward:N0}G";
            if (_txtInfraBonus != null) _txtInfraBonus.text = $"+ {infraBonus:N0}G";
            if (_txtSubtotalReward != null) _txtSubtotalReward.text = $"{subtotal:N0}G";
            if (_txtLeagueWinReward != null) _txtLeagueWinReward.text = $"+ {leagueWinReward:N0}G";
            if (_txtTotalReward != null) _txtTotalReward.text = $"{totalReward:N0}G";
        }
    }
    // 팀 ID를 이름으로 변환
    private string GetTeamName(string teamId)
    {
        if (teamId == StudentManager.TEAM_ID)
            return GameManager.Instance.SaveData.schoolName;

        var rivalData = LeagueDataManager.Instance.GetRivalMasterDataById(teamId);
        if (rivalData != null)
            return StringManager.Instance.GetString(rivalData.Value.teamNameKey);

        return teamId;
    }

    private void CreateHistoryList(int currentRound)
    {
        // 인스펙터 연결이 누락되었을 때 게임이 멈추지 않도록 방어
        if (_historyContainer == null || _historyRowPrefab == null)
        {
            Debug.LogWarning("LeagueCalculatePanel: History Container 또는 Prefab이 연결되지 않았습니다.");
            return;
        }

        foreach (Transform child in _historyContainer)
        {
            Destroy(child.gameObject);
        }

        for (int i = 1; i <= currentRound; i++)
        {
            var record = LeagueRecordManager.Instance.GetMatchRecord(i);
            if (record == null) continue;

            // 프리팹 복사 후 부모 아래에 붙이기
            MatchHistoryRow newRow = Instantiate(_historyRowPrefab, _historyContainer);
            
            // 해당 줄에 데이터 주입 & 로그 버튼 클릭 시 행동 정의
            newRow.Init(i, record, (clickedRound) =>
            {
                // 클릭된 줄의 라운드 번호를 넘겨주며 로그 패널을 엽니다.
                _logHistoryPanel.OpenPanel(clickedRound);
            });
        }
    }
}
