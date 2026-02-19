using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MatchReplayer : MonoBehaviour
{
    [Header("UI & References")]
    [SerializeField] private MatchUIManager _uiManager;
    [SerializeField] private MatchState _matchState;
    [SerializeField] private RectTransform _courtPanel; // CourtPanel 연결할 곳

    // 배속 기능 (인스펙터에서 1, 2, 8 등으로 조절 가능)
    [Header("Playback Settings")]
    [Range(1f, 10f)]
    public float PlaybackSpeed = 1.0f;

    // 공 UI 오브젝트
    private GameObject _ballUI;

    // 홈/어웨이 골대 UI 오브젝트
    private RectTransform _homeHoopUI;
    private RectTransform _awayHoopUI;

    private List<MatchLogData> _logs;
    public System.Action OnReplayEnded;

    public void Init(List<MatchLogData> logs)
    {
        _logs = logs;
        SpawnPlayerCircles(_matchState.HomeTeam, Color.blue);
        SpawnPlayerCircles(_matchState.AwayTeam, Color.red);
        SpawnHoops();
        SpawnBall();

        if (_uiManager != null) _uiManager.UpdateScoreBoard(_matchState);
    }

    // 선수 동그라미 생성
    private void SpawnPlayerCircles(MatchTeam team, Color color)
    {
        if (team == null || team.Roster == null) return;
        foreach (var player in team.Roster)
        {
            if (player.VisualObject == null)
            {
                player.VisualObject = CreateCircleUI(player.PlayerName, color, 30f);
                player.VisualObject.GetComponent<RectTransform>().anchoredPosition
                    = LogicToUIPos(player.LogicPosition);
            }
        }
    }

    // 골대 생성
    private void SpawnHoops()
    {
        // 이미 있으면 생성 안 함
        if (_homeHoopUI != null && _awayHoopUI != null) return;

        GameObject homeHoop = CreateCircleUI("HomeHoop", Color.blue, 20f);
        GameObject awayHoop = CreateCircleUI("AwayHoop", Color.red, 20f);

        _homeHoopUI = homeHoop.GetComponent<RectTransform>();
        _awayHoopUI = awayHoop.GetComponent<RectTransform>();

        _homeHoopUI.anchoredPosition = LogicToUIPos(new Vector2(0.5f, 0.05f));
        _awayHoopUI.anchoredPosition = LogicToUIPos(new Vector2(0.5f, 0.95f));
    }

    // 공 생성
    private void SpawnBall()
    {
        // 이미 있으면 생성 안 함
        if (_ballUI != null) return;
        _ballUI = CreateCircleUI("Ball", Color.yellow, 20f);
    }

    // UI 동그라미 오브젝트 생성 공통 함수
    private GameObject CreateCircleUI(string name, Color color, float size)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(_courtPanel, false);

        // RectTransform 설정
        RectTransform rt = obj.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(size, size);

        // Image 컴포넌트로 동그라미 표현
        UnityEngine.UI.Image img = obj.AddComponent<UnityEngine.UI.Image>();
        img.color = color;
        img.sprite = CreateCircleSprite(); // 동그라미 스프라이트 생성

        return obj;
    }

    // 동그라미 스프라이트 생성 함수
    private Sprite CreateCircleSprite()
    {
        // 유니티 기본 원형 스프라이트 사용
        Texture2D tex = new Texture2D(64, 64);
        Vector2 center = new Vector2(32, 32);
        float radius = 30f;

        for (int y = 0; y < 64; y++)
        {
            for (int x = 0; x < 64; x++)
            {
                float dist = Vector2.Distance(new Vector2(x, y), center);
                tex.SetPixel(x, y, dist <= radius ? Color.white : Color.clear);
            }
        }
        tex.Apply();

        return Sprite.Create(tex, new Rect(0, 0, 64, 64), new Vector2(0.5f, 0.5f));
    }

    private Vector2 LogicToUIPos(Vector2 logicPos)
    {
        float x = (logicPos.x - 0.5f) * _courtPanel.rect.width;
        float y = (logicPos.y - 0.5f) * _courtPanel.rect.height;
        return new Vector2(x, y);
    }

    public void PlayMatch()
    {
        if (_logs == null || _logs.Count == 0)
        {
            OnReplayEnded?.Invoke();
            return;
        }
        StartCoroutine(ReplayRoutine());
    }

    private IEnumerator ReplayRoutine()
    {
        if (_uiManager != null) _uiManager.UpdateLogText("=== Match Replay Start ===");

        foreach (var log in _logs)
        {
            float speed = Mathf.Max(1.0f, PlaybackSpeed);

            _matchState.SetReplayState(log.Quarter, log.GameTime);

            if (_uiManager != null)
            {
                _uiManager.UpdateLogText(log.LogText);
                _uiManager.UpdateScoreBoard(_matchState);
            }

            // 선수 이동
            MoveAllCircles(_matchState.HomeTeam, log.HomePositions, 0.5f / speed);
            MoveAllCircles(_matchState.AwayTeam, log.AwayPositions, 0.5f / speed);

            // 공 이동
            if (_ballUI != null)
            {
                RectTransform ballRT = _ballUI.GetComponent<RectTransform>();
                Vector2 targetUIPos = LogicToUIPos(log.BallPos);

                if (log.EventType == "GOAL" || log.EventType == "MISS")
                {
                    Vector2 hoopUIPos = (log.TeamId == 0)
                        ? _awayHoopUI.anchoredPosition
                        : _homeHoopUI.anchoredPosition;

                    ballRT.anchoredPosition = LogicToUIPos(log.BallPos);
                    ballRT.DOAnchorPos(hoopUIPos, 0.5f / speed);
                }
                else if (log.EventType == "PASS" || log.EventType == "DRIBBLE")
                {
                    ballRT.DOAnchorPos(targetUIPos, 0.4f / speed);
                }
            }

            // 득점 처리
            if (log.EventType == "GOAL")
            {
                if (log.TeamId == 0) _matchState.HomeTeam.AddScore(log.ScoreAdded);
                else _matchState.AwayTeam.AddScore(log.ScoreAdded);

                if (_uiManager != null)
                {
                    _uiManager.UpdateScoreBoard(_matchState);
                    if (log.IsCutIn)
                    {
                        _uiManager.ShowCutInEffect(log.CutInType);
                        yield return new WaitForSeconds(1.5f / speed);
                    }
                }
            }

            yield return new WaitForSeconds(1.0f / speed);
        }

        if (_uiManager != null)
        {
            _uiManager.UpdateLogText("=== Match Phase Ended ===");
        }

        OnReplayEnded?.Invoke();
    }


    // 캡슐 이동 처리 함수
    private void MoveAllCircles(MatchTeam team, Vector2[] posArray, float duration)
    {
        for (int i = 0; i < team.Roster.Count; i++)
        {
            if (team.Roster[i].VisualObject != null && posArray != null && i < posArray.Length)
            {
                RectTransform rt = team.Roster[i].VisualObject.GetComponent<RectTransform>();
                Vector2 uiPos = LogicToUIPos(posArray[i]);
                rt.DOAnchorPos(uiPos, duration);
            }
        }
    }
}
