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
        {
            _txtLeagueName.text = StringManager.Instance.GetString(masterData.Value.leagueNameKey);
            StringManager.Instance.ApplyFont(_txtLeagueName);
        }

        int currentRound = league.isFinished ? league.currentRoundIndex - 1 : league.currentRoundIndex;
        currentRound = Mathf.Max(0, currentRound);

        int displayRound = currentRound + 1;
        if (_txtCurrentRound != null) _txtCurrentRound.text = $"{displayRound}";
        // 대진표 데이터 채우기 (16강)
        PopulateBracket(league, currentRound);
        bool cannotPlay = league.isFinished || league.isPlayerEliminated;
        _txtBtnAction.text = string.IsNullOrEmpty(actionText) ? (cannotPlay ? StringManager.Instance.GetString("UI_Popup_닫기") : StringManager.Instance.GetString("UI_Matchlog_경기준비")) : actionText;
        StringManager.Instance.ApplyFont(_txtBtnAction);

        _btnAction.onClick.RemoveAllListeners();
        _btnAction.onClick.AddListener(() =>
        {
            gameObject.SetActive(false);
            if (_customAction != null)
            {
                _customAction.Invoke(); // 결산창 띄우기 실행!
            }
            else if (!cannotPlay)
            {
                GameManager.Instance.ChangeState<MatchPrepState>();
            }
        });

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
        // 가장 먼저 모든 노드를 통째로 끕니다. 
        DisableAllNodes();
        // 대진표 크기 확인 및 미정(?) 슬롯 켜기
        var masterData = LeagueDataManager.Instance.GetMasterDataById(league.leagueId);

        int totalRounds = masterData.HasValue ? masterData.Value.roundCount : 4;

        // 단 2팀만 플레이하는 토너먼트라면, 1라운드(결승)만 존재하므로 totalRounds를 1로 고정
        if (league.teams != null && league.teams.Count == 2)
        {
            totalRounds = 1;
        }

        int uiOffset = 4 - totalRounds;


        // 이번 토너먼트 규모에 해당하는 노드들은 기본값을 "?"로 주어 켜줍니다.
        for (int depth = uiOffset; depth <= 4; depth++)
        {
            List<TournamentNode> nodes = GetUINodesByDepth(depth);
            if (nodes != null)
            {
                foreach (var n in nodes) n.Init("?", depth, false, false, false);
            }
            else if (depth == 4 && _winnerNode != null)
            {
                _winnerNode.Init("?", 4, false, false, false);
            }
        }

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
            string loserId = match.homeScore < match.awayScore ? match.homeTeamId : match.awayTeamId;

            string scoreString = $"{match.homeScore}-{match.awayScore}";

            // 같은 라운드 내에서 몇 번째 매치인지 계산
            List<LeagueMatchRecord> roundMatches = league.matchRecords.FindAll(m => m.roundIndex == match.roundIndex);
            int matchIndexInRound = roundMatches.IndexOf(match);

            int uiDepth = match.roundIndex + uiOffset;
            int nextUiDepth = uiDepth + 1;

            // 노드들에 점수와 비주얼 규칙 적용
            List<TournamentNode> targetNodes = GetUINodesByDepth(uiDepth);
            if (targetNodes != null)
            {
                int nodeIndex1 = matchIndexInRound * 2;
                int nodeIndex2 = matchIndexInRound * 2 + 1;

                if (nodeIndex1 < targetNodes.Count)
                {
                    var n = targetNodes[nodeIndex1];
                    if (n.TeamId == match.homeTeamId || n.TeamId == match.awayTeamId)
                    {
                        bool isWinner = n.TeamId == winnerId;
                        bool isEliminated = n.TeamId == loserId;
                        // Init 호출: isEliminated로 박스 밝기 조절, isWinner로 선 색상 조절
                        n.Init(n.TeamId, match.roundIndex, false, isEliminated, isWinner, scoreString);
                    }
                }

                if (nodeIndex2 < targetNodes.Count)
                {
                    var n = targetNodes[nodeIndex2];
                    if (n.TeamId == match.homeTeamId || n.TeamId == match.awayTeamId)
                    {
                        bool isWinner = n.TeamId == winnerId;
                        bool isEliminated = n.TeamId == loserId;
                        n.Init(n.TeamId, match.roundIndex, false, isEliminated, isWinner, scoreString);
                    }
                }
            }
            // 승리한 팀을 다음 라운드 노드로 올림
            // 박스는 밝게 유지(isEliminated=false)하고, 선 색상은 white로 초기화합니다.
            List<TournamentNode> nextNodes = GetUINodesByDepth(nextUiDepth);
            if (nextNodes != null && matchIndexInRound < nextNodes.Count)
            {
                nextNodes[matchIndexInRound].Init(winnerId, match.roundIndex + 1, (currentRound == (match.roundIndex + 1)), false, false, "");
            }
            else if (nextUiDepth == 4 && _winnerNode != null)
            {
                _winnerNode.Init(winnerId, match.roundIndex + 1, (currentRound == (match.roundIndex + 1)), false, true, ""); // 최종 우승 노드는 선이 나가지 않지만 밝게 유지
            }
        }
    }
    // 모든 노드를 강력하게 비활성화하는 헬퍼 함수
    //★ HJ
    //각 노드를 비활성화 하면 안그러면 줄만 남기 때문에, 노드의 부모패널('Team16''Team8''Team4')을 비활성화 해야 합니다. 
    //이긴 팀의 파이프 라인색을 변경하기 위해서는 'Out Pipe Image' 외에도 두가지 이미지('MiddlePipeLine', 'NextLine')를 더 변경해야 합니다.

    private void DisableAllNodes()
    {
        void DisableNodesInList(List<TournamentNode> nodes)
        { 
            if (nodes == null) return;
            foreach (var n in nodes) n.gameObject.SetActive(false);
        }
        DisableNodesInList(_round1Nodes);
        DisableNodesInList(_round2Nodes);
        DisableNodesInList(_round3Nodes);
        DisableNodesInList(_round4Nodes);
        if (_winnerNode != null) _winnerNode.gameObject.SetActive(false);

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
        TournamentNode playerNode = targetList.Find(n => n.TeamId == PrefKeys.PLAYER_TEAM_ID);

        if (playerNode == null || _scrollRect == null) yield break;

        Vector2 nodePos = (Vector2)_scrollRect.transform.InverseTransformPoint(playerNode.transform.position);
        Vector2 contentPos = (Vector2)_scrollRect.transform.InverseTransformPoint(_contentRect.position);

        Vector2 targetPos = contentPos - nodePos;
        targetPos.x += 150f; // 좌측 여백

        _contentRect.DOAnchorPos(targetPos, 0.5f).SetEase(Ease.OutCubic);
    }
}
