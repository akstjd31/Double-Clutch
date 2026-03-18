using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TournamentBoardPanel : MonoBehaviour
{
    [Header("Top UI")]
    [SerializeField] private TextMeshProUGUI _txtLeagueName;
    [SerializeField] private TextMeshProUGUI _txtCurrentRound;

    [Header("Bracket Nodes (16강 기준)")]
    [SerializeField] private List<TournamentNode> _round1Nodes; // 16강 (16개)
    [SerializeField] private List<TournamentNode> _round2Nodes; // 8강 (8개)
    [SerializeField] private List<TournamentNode> _round3Nodes; // 4강 (4개)
    [SerializeField] private List<TournamentNode> _round4Nodes; // 결승 (2개)
    [SerializeField] private TournamentNode _winnerNode;        // 최종 우승 (1개)

    [Header("Scroll & Camera")]
    [SerializeField] private ScrollRect _scrollRect;
    [SerializeField] private RectTransform _contentRect;

    [Header("Bottom Actions")]
    [SerializeField] private Button _btnAction;
    [SerializeField] private TextMeshProUGUI _txtBtnAction;

    private Action _customAction;
    private string _customActionText;

    public void OpenPanel(Action onActionClick = null, string actionText = null)
    {
        gameObject.SetActive(true);
        _customAction = onActionClick;
        _customActionText = actionText;

        var league = LeagueManager.Instance.CurrentLeague;
        if (league == null) return;

        // 타이틀 세팅
        var masterData = LeagueDataManager.Instance.GetMasterDataById(league.leagueId);
        if (_txtLeagueName != null && masterData.HasValue)
            _txtLeagueName.text = StringManager.Instance.GetString(masterData.Value.leagueNameKey);

        int currentRound = league.isFinished ? league.currentRoundIndex - 1 : league.currentRoundIndex;
        currentRound = Mathf.Max(0, currentRound);

        int displayRound = currentRound + 1;
        if (_txtCurrentRound != null) _txtCurrentRound.text = $"{displayRound}";
        // 대진표 데이터 채우기 (16강)
        PopulateBracket(league, currentRound);

        // 버튼 세팅
        if (league.isFinished)
        {
            _txtBtnAction.text = "닫기";
            _btnAction.onClick.RemoveAllListeners();
            _btnAction.onClick.AddListener(() => gameObject.SetActive(false));
        }
        else
        {
            _txtBtnAction.text = string.IsNullOrEmpty(actionText) ? "경기 준비" : actionText;
            _btnAction.onClick.RemoveAllListeners();
            _btnAction.onClick.AddListener(() =>
            {
                gameObject.SetActive(false);
                if (_customAction != null) _customAction.Invoke();
                else GameManager.Instance.ChangeState<MatchPrepState>();
            });
        }

        // 내 팀 위치로 카메라 포커싱
        StartCoroutine(FocusOnPlayerNode(league.currentRoundIndex));
    }
    // 뎁스(Depth)에 따라 노드 리스트를 반환하는 헬퍼 함수
    private List<TournamentNode> GetUINodesByDepth(int depth)
    {
        switch (depth)
        {
            case 0: return _round1Nodes;
            case 1: return _round2Nodes;
            case 2: return _round3Nodes;
            case 3: return _round4Nodes;
            default: return null;
        }
    }
    private void PopulateBracket(LeagueSaveData league, int currentRound)
    {
        // 초기화 (모든 노드 비우기)
        foreach (var n in _round1Nodes) n.Init("", 0, false, false, false);
        foreach (var n in _round2Nodes) n.Init("", 1, false, false, false);
        foreach (var n in _round3Nodes) n.Init("", 2, false, false, false);
        foreach (var n in _round4Nodes) n.Init("", 3, false, false, false);
        if (_winnerNode != null) _winnerNode.Init("", 4, false, false, false);

        // 총 라운드 수를 계산하여 UI가 시작될 오프셋을 구합니다 (16강이면 0, 8강이면 1, 결승이면 3)
        var masterData = LeagueDataManager.Instance.GetMasterDataById(league.leagueId);
        int totalRounds = masterData.HasValue ? masterData.Value.roundCount : 4;
        int uiOffset = 4 - totalRounds;

        HashSet<string> eliminatedTeams = new HashSet<string>();
        if (league.matchRecords != null)
        {
            foreach (var match in league.matchRecords)
            {
                if (!match.isPlayed) continue;
                string loserId = match.homeScore < match.awayScore ? match.homeTeamId : match.awayTeamId;
                eliminatedTeams.Add(loserId);
            }
        }

        // 1라운드(16강) 세팅 (초기 참가팀 16팀)
        bool isR1Current = (currentRound == 0); // 1라운드가 현재 라운드인지?

        // 동적으로 계산된 출발 노드 라인
        List<TournamentNode> startNodes = GetUINodesByDepth(uiOffset);

        if (startNodes != null)
        {
            for (int i = 0; i < league.teams.Count; i++)
            {
                if (i < startNodes.Count)
                {
                    string tId = league.teams[i].teamId;
                    startNodes[i].Init(tId, 0, isR1Current, eliminatedTeams.Contains(tId), false);
                }
            }
        }

        // 진행된 매치 기록을 읽어서 승자 노드 채우기
        if (league.matchRecords == null) return;

        foreach (var match in league.matchRecords)
        {
            if (!match.isPlayed) continue;

            string winnerId = match.homeScore > match.awayScore ? match.homeTeamId : match.awayTeamId;

            // 같은 라운드 내에서 몇 번째 매치인지 계산
            List<LeagueMatchRecord> roundMatches = league.matchRecords.FindAll(m => m.roundIndex == match.roundIndex);
            int matchIndexInRound = roundMatches.IndexOf(match);

            // 승자가 진출하는 '다음 라운드'의 인덱스
            int nextRoundIndex = match.roundIndex + 1;
            bool isNextRoundCurrent = (currentRound == nextRoundIndex); // 그 다음 라운드가 현재 라운드인지?
            bool isEliminated = eliminatedTeams.Contains(winnerId);

            int uiDepth = match.roundIndex + uiOffset;
            int nextUiDepth = uiDepth + 1;

            // 이전 라운드 노드를 찾아서 승리시 파이프 색 변경
            UpdateWinnerPipe(uiDepth, match.roundIndex, matchIndexInRound, winnerId, eliminatedTeams);

            List<TournamentNode> nextNodes = GetUINodesByDepth(nextUiDepth);
            if (nextNodes != null && matchIndexInRound < nextNodes.Count)
            {
                nextNodes[matchIndexInRound].Init(winnerId, nextRoundIndex, isNextRoundCurrent, isEliminated, false);
            }
            else if (nextUiDepth == 4 && _winnerNode != null)
            {
                _winnerNode.Init(winnerId, nextRoundIndex, isNextRoundCurrent, isEliminated, true);
            }
        }
    }
    // 승리한 노드의 파이프만 불을 켜주기 위한 보조 함수
    private void UpdateWinnerPipe(int uiDepth, int originalRoundIndex, int matchIndex, string winnerId, HashSet<string> eliminatedTeams)
    {
        List<TournamentNode> targetNodes = GetUINodesByDepth(uiDepth);
        if (targetNodes == null) return;

        int nodeIndex1 = matchIndex * 2;
        int nodeIndex2 = matchIndex * 2 + 1;

        if (nodeIndex1 < targetNodes.Count && targetNodes[nodeIndex1].TeamId == winnerId)
            targetNodes[nodeIndex1].Init(winnerId, originalRoundIndex, false, eliminatedTeams.Contains(winnerId), true);

        if (nodeIndex2 < targetNodes.Count && targetNodes[nodeIndex2].TeamId == winnerId)
            targetNodes[nodeIndex2].Init(winnerId, originalRoundIndex, false, eliminatedTeams.Contains(winnerId), true);
    }
    private IEnumerator FocusOnPlayerNode(int currentRound)
    {
        yield return null;

        var league = LeagueManager.Instance.CurrentLeague;
        var masterData = LeagueDataManager.Instance.GetMasterDataById(league.leagueId);
        int totalRounds = masterData.HasValue ? masterData.Value.roundCount : 4;

        int uiOffset = 4 - totalRounds;
        int uiDepth = currentRound + uiOffset;

        List<TournamentNode> targetList = GetUINodesByDepth(uiDepth) ?? _round1Nodes;
        TournamentNode playerNode = targetList.Find(n => n.TeamId == StudentManager.TEAM_ID);

        if (playerNode == null || _scrollRect == null) yield break;

        Vector2 nodePos = (Vector2)_scrollRect.transform.InverseTransformPoint(playerNode.transform.position);
        Vector2 contentPos = (Vector2)_scrollRect.transform.InverseTransformPoint(_contentRect.position);

        Vector2 targetPos = contentPos - nodePos;
        targetPos.x += 150f; // 좌측 여백

        _contentRect.DOAnchorPos(targetPos, 0.5f).SetEase(Ease.OutCubic);
    }
}
