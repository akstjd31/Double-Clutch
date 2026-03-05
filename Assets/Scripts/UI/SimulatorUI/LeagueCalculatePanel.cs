using UnityEngine;
using UnityEngine.UI;
using System;

public class LeagueCalculatePanel : MonoBehaviour
{
    [SerializeField] private Button _btnConfirm;
    [SerializeField] private Button _btnTotalRank;

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

        // 창이 열릴 때 대진 이력 리스트를 생성합니다.
        CreateHistoryList(currentRound);
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
