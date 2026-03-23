using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Reflection;

public class MatchReplayer : MonoBehaviour
{
    [Header("UI & References")]
    [SerializeField] private MatchUIManager _uiManager;
    [SerializeField] private MatchState _matchState;
    [SerializeField] private RectTransform _courtPanel; // CourtPanel 연결할 곳

    // [v0.1.4 Update] 시각적 요소 리소스 및 크기 설정 추가
    [Header("Match Visual Settings")]
    [SerializeField, Tooltip("코트 중앙 서클의 지름")]
    private float _centerCircleDiameter = 200f;
    [SerializeField] private Image _courtBackgroundImage; // 코트 배경 Image 컴포넌트
    [SerializeField] private Sprite _circleMaskSprite; // 원형 마스킹용 스프라이트

    // 배속 기능 (인스펙터에서 조절용)
    [Header("Playback Settings")]
    [Range(1f, 10f)]
    public float PlaybackSpeed = 1.0f;

    // 공 UI 오브젝트
    private GameObject _ballUI;

    // 홈/어웨이 골대 UI 오브젝트
    private RectTransform _homeHoopUI;
    private RectTransform _awayHoopUI;

    // 오디오 관련 컴포넌트
    [Header("Audio Settings")]
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _sfxCheer; // 2점슛 함성 사운드
    [SerializeField] private AudioClip _sfxClap;  // 스틸 박수 사운드

    public RectTransform CourtPanel => _courtPanel;
    private List<MatchLogData> _logs;
    public System.Action OnReplayEnded;

    private Coroutine _replayCoroutine;
    private int _currentLogIndex = 0;
    private bool _isSkipping = false;

    // 이전 로그의 남은 시간을 추적하기 위한 변수
    private float _previousRemainTime = 600f; // 1쿼터 시작 시간 기준

    public void Init(List<MatchLogData> logs)
    {
        _logs = logs;

        // 경기장 배경 설정
        if (_courtBackgroundImage != null)
        {
            _courtBackgroundImage.sprite = SpriteManager.Instance.GetSprite("Bg_Basketball_Court");
        }

        // 기존 선수 오브젝트 전부 정리 후 새로 생성
        CleanUpVisuals();

        SpawnPlayerCircles(_matchState.HomeTeam, Color.blue);
        SpawnPlayerCircles(_matchState.AwayTeam, Color.red);
        SpawnHoops();
        SpawnBall();

        if (_uiManager != null) _uiManager.UpdateScoreBoard(_matchState);

        // 초기화 시점의 남은 시간 세팅
        _previousRemainTime = _matchState.RemainTime;
    }

    private void CleanUpVisuals()
    {
        foreach (Transform child in _courtPanel)
        {
            Destroy(child.gameObject);
        }

        _ballUI = null;
        _homeHoopUI = null;
        _awayHoopUI = null;

        if (_matchState.HomeTeam != null)
            foreach (var player in _matchState.HomeTeam.Roster)
                player.VisualObject = null;

        if (_matchState.AwayTeam != null)
            foreach (var player in _matchState.AwayTeam.Roster)
                player.VisualObject = null;
    }


    // 선수 생성 (원형 마스킹 + 포트레이트)
    private void SpawnPlayerCircles(MatchTeam team, Color outlineColor)
    {
        if (team == null || team.Roster == null) return;

        // 유닛 크기는 센터 서클 지름의 1/2
        float playerUnitSize = _centerCircleDiameter * 0.5f;

        foreach (var player in team.Roster)
        {
            if (player.VisualObject == null)
            {
                // 선수의 초상화 스프라이트 로드
                Sprite portraitSprite = SpriteManager.Instance.GetSprite(player.ResourceKey);

                player.VisualObject = CreatePlayerMaskedUI(MakeName(player.PlayerName), outlineColor, playerUnitSize, portraitSprite);
                player.VisualObject.GetComponent<RectTransform>().anchoredPosition = LogicToUIPos(player.LogicPosition);
            }
        }
    }
    // 마스크가 적용된 선수 오브젝트를 동적 생성
    private GameObject CreatePlayerMaskedUI(string name, Color outlineColor, float size, Sprite portraitSprite)
    {
        // 최상위 오브젝트 (테두리)
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(_courtPanel, false);
        RectTransform rt = obj.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(size, size);

        Image bgImg = obj.AddComponent<Image>();
        bgImg.color = outlineColor;
        bgImg.sprite = _circleMaskSprite != null ? _circleMaskSprite : CreateCircleSprite();

        // 마스크 오브젝트
        GameObject maskObj = new GameObject("Mask");
        maskObj.transform.SetParent(obj.transform, false);
        RectTransform maskRt = maskObj.AddComponent<RectTransform>();
        float innerSize = size * 0.85f; // 테두리 두께 확보
        maskRt.sizeDelta = new Vector2(innerSize, innerSize);

        Image maskImg = maskObj.AddComponent<Image>();
        maskImg.color = Color.black;
        maskImg.sprite = _circleMaskSprite != null ? _circleMaskSprite : CreateCircleSprite();
        Mask mask = maskObj.AddComponent<Mask>();
        mask.showMaskGraphic = true;

        // 실제 초상화 이미지
        GameObject portraitObj = new GameObject("Portrait");
        portraitObj.transform.SetParent(maskObj.transform, false);
        RectTransform portraitRt = portraitObj.AddComponent<RectTransform>();
        portraitRt.sizeDelta = new Vector2(innerSize, innerSize);

        Image portraitImg = portraitObj.AddComponent<Image>();
        portraitImg.sprite = portraitSprite;
        portraitImg.preserveAspect = true; // 비율 유지

        return obj;
    }
    private string MakeName(string[] nameKey)
    {
        StringManager manager = StringManager.Instance;
        string name = manager.GetString(nameKey[0]) + manager.GetString(nameKey[1]) + manager.GetString(nameKey[2]);
        return name;
    }

    // 골대 생성
    private void SpawnHoops()
    {
        // 이미 있으면 생성 안 함
        if (_homeHoopUI != null && _awayHoopUI != null) return;

        float hoopSize = _centerCircleDiameter * 0.6f;

        // 골대 리소스 호출
        Sprite homeHoopSprite = SpriteManager.Instance.GetSprite("Icon_RimHome");
        Sprite awayHoopSprite = SpriteManager.Instance.GetSprite("Icon_RimAway");

        GameObject homeHoop = CreateIconUI("HomeHoop", homeHoopSprite, hoopSize, true);
        GameObject awayHoop = CreateIconUI("AwayHoop", awayHoopSprite, hoopSize, true);

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

        // 공 크기는 선수 유닛 지름의 1/2
        float ballSize = (_centerCircleDiameter * 0.5f) * 0.5f;

        // 농구공 리소스 호출
        Sprite ballSprite = SpriteManager.Instance.GetSprite("Icon_Basketball");

        _ballUI = CreateIconUI("Ball", ballSprite, ballSize, false);
    }

    // 아이콘 생성용 헬퍼 함수
    private GameObject CreateIconUI(string name, Sprite iconSprite, float size, bool isHoop)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(_courtPanel, false);

        // RectTransform 설정
        RectTransform rt = obj.AddComponent<RectTransform>();

        // Image 컴포넌트로 아이콘 표현
        UnityEngine.UI.Image img = obj.AddComponent<UnityEngine.UI.Image>();
        
        img.sprite = iconSprite;

        if (iconSprite == null)
        {
            img.color = Color.clear; // 스프라이트 미할당 시 투명처리
            rt.sizeDelta = new Vector2(size, size);
        }

        else
        {
            if (isHoop)
            {
                // 골대의 경우 원본 이미지의 비율을 유지
                img.preserveAspect = true;
                // 가로 너비를 넉넉하게 설정
                rt.sizeDelta = new Vector2(size * 1.5f, size);
            }
            else
            {
                // 1:1 비율인 경우
                rt.sizeDelta = new Vector2(size, size);
            }
        }

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
        _isSkipping = false;
        _currentLogIndex = 0;

        // 실행한 코루틴을 변수에 담아둠 (나중에 강제 정지하기 위함)
        _replayCoroutine = StartCoroutine(ReplayRoutine());
    }

    private IEnumerator ReplayRoutine()
    {
        if (_uiManager != null) _uiManager.UpdateLogText("===============");

        for (_currentLogIndex = 0; _currentLogIndex < _logs.Count; _currentLogIndex++)
        {
            var log = _logs[_currentLogIndex];
            float speed = Mathf.Max(1.0f, PlaybackSpeed);

            // 쿼터가 바뀌었으면 previousTime 리셋
            if (log.EventType == "GameStart" || log.GameTime > _previousRemainTime)
            {
                _previousRemainTime = log.GameTime;
            }

            float startRemainTime = _previousRemainTime;
            float endRemainTime = log.GameTime;

            // 애니메이션 재생 시간을 고정합니다. (기본 1초)

            float baseDuration = 1.0f;
            float finalDuration = baseDuration / speed;

            // 다음 턴을 위해 값 갱신
            _previousRemainTime = endRemainTime;

            // 로그 텍스트는 즉시 띄우기
            if (_uiManager != null)
            {
                _uiManager.UpdateLogText(log.LogText);
            }

            // 점수판 타이머를 계산된 시간(finalDuration) 동안 부드럽게 감소
            DOTween.To(() => startRemainTime, x =>
            {
                _matchState.SetReplayState(log.Quarter, x);
                if (_uiManager != null) _uiManager.UpdateScoreBoard(_matchState);
            }, endRemainTime, finalDuration).SetEase(Ease.Linear).SetId("MatchReplay");



            if (!string.IsNullOrEmpty(log.SfxType) && _audioSource != null)
            {
                if (log.SfxType == "CHEER" && _sfxCheer != null) _audioSource.PlayOneShot(_sfxCheer);
                else if (log.SfxType == "CLAP" && _sfxClap != null) _audioSource.PlayOneShot(_sfxClap);
            }

            MoveAllCircles(_matchState.HomeTeam, log.HomePositions, finalDuration);
            MoveAllCircles(_matchState.AwayTeam, log.AwayPositions, finalDuration);

            if (_ballUI != null)
            {
                RectTransform ballRT = _ballUI.GetComponent<RectTransform>();
                Vector2 targetUIPos = LogicToUIPos(log.BallPos);

                if (log.EventType == "GOAL" || log.EventType == "MISS")
                {
                    Vector2 hoopUIPos = (log.TeamId == 0) ? _awayHoopUI.anchoredPosition : _homeHoopUI.anchoredPosition;
                    Vector2 shooterPos = LogicToUIPos(log.BallPos);

                    Sequence shootSeq = DOTween.Sequence().SetId("MatchReplay");
                    shootSeq.Append(ballRT.DOAnchorPos(shooterPos, finalDuration * 0.1f)); // 빠르게 슈터 손으로 정렬
                    shootSeq.Append(ballRT.DOAnchorPos(hoopUIPos, finalDuration * 0.9f)); // 골대로 슛
                }
                else
                {
                    ballRT.DOAnchorPos(targetUIPos, finalDuration * 0.6f).SetId("MatchReplay");
                }
            }

            yield return new WaitForSeconds(finalDuration);

            if (log.EventType == "GOAL")
            {
                if (log.TeamId == 0) _matchState.HomeTeam.AddScore(log.ScoreAdded);
                else _matchState.AwayTeam.AddScore(log.ScoreAdded);

                if (_uiManager != null)
                {
                    _uiManager.UpdateScoreBoard(_matchState);
                    if (log.IsCutIn)
                    {
                        _uiManager.ShowCutInEffect(log.CutInType, log.CutInResourceKey, speed);
                        yield return new WaitForSeconds(1.5f / speed);
                    }
                }
            }

        }

        if (_uiManager != null) _uiManager.UpdateLogText("===============");
        OnReplayEnded?.Invoke();
    }

    private void MoveAllCircles(MatchTeam team, Vector2[] posArray, float duration)
    {
        for (int i = 0; i < team.Roster.Count; i++)
        {
            if (team.Roster[i].VisualObject != null && posArray != null && i < posArray.Length)
            {
                RectTransform rt = team.Roster[i].VisualObject.GetComponent<RectTransform>();
                Vector2 uiPos = LogicToUIPos(posArray[i]);
                rt.DOAnchorPos(uiPos, duration).SetId("MatchReplay");
            }
        }
    }
    public void SkipReplay()
    {
        if (_isSkipping) return;
        _isSkipping = true;

        // 재생 중이던 연출 코루틴 강제 정지
        if (_replayCoroutine != null)
        {
            StopCoroutine(_replayCoroutine);
            _replayCoroutine = null;
        }
        DOTween.Kill("MatchReplay");
        // 남은 로그들을 순식간에 돌리면서 점수만 한방에 합산
        for (int i = _currentLogIndex; i < _logs.Count; i++)
        {
            var log = _logs[i];
            if (log.EventType == "GOAL")
            {
                if (log.TeamId == 0) _matchState.HomeTeam.AddScore(log.ScoreAdded);
                else _matchState.AwayTeam.AddScore(log.ScoreAdded);
            }
        }

        // 상태 갱신 (마지막 로그의 시간/쿼터로 맞춤)
        if (_logs.Count > 0)
        {
            var lastLog = _logs[_logs.Count - 1];
            _matchState.SetReplayState(lastLog.Quarter, lastLog.GameTime);
        }

        // UI 최신화 및 진행 중이던 컷인 강제 종료
        if (_uiManager != null)
        {
            _uiManager.UpdateScoreBoard(_matchState);
            _uiManager.UpdateLogText("===============");
            _uiManager.ForceCloseCutIn();
        }

        // 다음 단계(하프타임 또는 경기 종료)로 즉시 넘어감
        OnReplayEnded?.Invoke();
    }
}
