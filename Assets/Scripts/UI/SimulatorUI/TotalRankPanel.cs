using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Game.Constants;

public class TotalRankPanel : MonoBehaviour
{
    [SerializeField] private Button _btnClose;

    [Header("Rank List UI")]
    [SerializeField] private Transform _rowContainer;     // 순위표가 들어갈 부모 (ScrollView Content)
    [SerializeField] private TotalRankRow _rowPrefab;     // TotalRankRow 프리팹

    [Header("Title UI")]
    [SerializeField] private TextMeshProUGUI _txtLeagueName; // 리그 이름

    private void Awake()
    {
        // 닫기 버튼에 패널 비활성화 기능 연결
        if (_btnClose != null)
            _btnClose.onClick.AddListener(() => gameObject.SetActive(false));
    }

    public void OpenPanel()
    {
        gameObject.SetActive(true);
        RefreshRankList();
    }

    private void RefreshRankList()
    {
        if (_rowContainer == null || _rowPrefab == null) return;

        // 기존 목록 초기화 
        foreach (Transform child in _rowContainer)
        {
            Destroy(child.gameObject);
        }

        // 리그 순위 데이터 가져오기
        var league = LeagueManager.Instance.CurrentLeague;
        if (league == null || league.standings == null) return;

        if (_txtLeagueName != null)
        {
            var masterData = LeagueDataManager.Instance.GetMasterDataById(league.leagueId);
            if (masterData.HasValue)
            {
                // 번역된 실제 리그 이름으로 텍스트 변경
                _txtLeagueName.text = StringManager.Instance.GetString(masterData.Value.leagueNameKey);
                StringManager.Instance.ApplyFont(_txtLeagueName);
            }
        }

        string myTeamId = PrefKeys.PLAYER_TEAM_ID;

        // 순위표 생성
        foreach (var standing in league.standings)
        {
            TotalRankRow newRow = Instantiate(_rowPrefab, _rowContainer);
            bool isMyTeam = (standing.teamId == myTeamId);

            // TotalRankRow.cs의 Init 함수 호출
            newRow.Init(standing, isMyTeam);
        }
    }
}