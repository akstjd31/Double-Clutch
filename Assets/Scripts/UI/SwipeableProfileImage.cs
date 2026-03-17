using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using DG.Tweening;

[RequireComponent(typeof(RectTransform))]
public class SwipeableProfileImage : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("움직일 대상 (실제 캐릭터 이미지)")]
    [SerializeField, Tooltip("여기에 실제 움직일 '이미지' 오브젝트를 드래그해서 넣으세요.")]
    private RectTransform _targetImage;

    [Header("스와이프 설정")]
    [SerializeField, Tooltip("이 거리 이상 드래그해야 선수가 넘어갑니다.")]
    private float _dragThreshold = 300f;

    [SerializeField, Tooltip("화면 밖으로 날아갈 때의 이동 거리")]
    private float _slideOffDistance = 1000f;

    [Header("이벤트 연결")]
    [Tooltip("왼쪽으로 밀었을 때 (다음 선수 보기)")]
    public UnityEvent OnSwipeLeft_NextPlayer;

    [Tooltip("오른쪽으로 밀었을 때 (이전 선수 보기)")]
    public UnityEvent OnSwipeRight_PrevPlayer;

    private Vector2 _originPos;

    private void Awake()
    {
     
    }

    private void OnEnable()
    {
        // 팝업이 켜질 때마다 원래 위치 초기화 및 저장
        _targetImage.DOKill();
        _originPos = _targetImage.anchoredPosition;
    }

    // 드래그 시작 시점
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (_targetImage == null) return;
        _targetImage.DOKill();
    }

    // 드래그 중 (이미지가 마우스를 따라다님)
    public void OnDrag(PointerEventData eventData)
    {
        if (_targetImage == null) return;
        // 좌우(x축)로만 움직이게 제한
        _targetImage.anchoredPosition += new Vector2(eventData.delta.x, 0);
    }

    // 드래그 종료 (손을 뗐을 때)
    public void OnEndDrag(PointerEventData eventData)
    {
        if (_targetImage == null) return;

        float diffX = _targetImage.anchoredPosition.x - _originPos.x;

        if (diffX < -_dragThreshold)
        {
            // 왼쪽으로 충분히 밀었음 -> 다음 선수
            SlideOutAndTrigger(-_slideOffDistance, OnSwipeLeft_NextPlayer, _slideOffDistance);
        }
        else if (diffX > _dragThreshold)
        {
            // 오른쪽으로 충분히 밀었음 -> 이전 선수
            SlideOutAndTrigger(_slideOffDistance, OnSwipeRight_PrevPlayer, -_slideOffDistance);
        }
        else
        {
            // 충분히 밀지 않음 -> 제자리로 튕겨서 복귀
            _targetImage.DOAnchorPos(_originPos, 0.3f).SetEase(Ease.OutBack);
        }
    }

    // 애니메이션 및 데이터 교체 실행 함수
    private void SlideOutAndTrigger(float outPosX, UnityEvent triggerEvent, float inPosX)
    {
        // 1단계: 화면 밖으로 빠르게 날아감
        _targetImage.DOAnchorPosX(_originPos.x + outPosX, 0.15f).SetEase(Ease.InQuad).OnComplete(() =>
        {
            // 2단계: 이 타이밍에 실제로 다음/이전 선수 데이터를 불러옴
            triggerEvent?.Invoke();

            // 3단계: 이미지를 반대편 화면 밖으로 즉시 순간이동 시킴
            _targetImage.anchoredPosition = new Vector2(_originPos.x + inPosX, _originPos.y);

            // 4단계: 반대편에서 원래 자리로 부드럽게 미끄러져 들어옴
            _targetImage.DOAnchorPos(_originPos, 0.25f).SetEase(Ease.OutQuad);
        });
    }
}