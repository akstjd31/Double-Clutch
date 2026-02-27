using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private Canvas _rootCanvas;
    private Transform _currentParent;
    private RectTransform _rect;
    private CanvasGroup _canvasGroup;
    private bool _droppedSuccessfully;
    private Vector2 _pointerOffset;
    private PlayerCard _playerCard;


    private void Awake()
    {
        _rootCanvas = GameObject.FindAnyObjectByType<Canvas>();
        _currentParent = this.transform.parent;
        _canvasGroup = this.GetComponent<CanvasGroup>();
        _playerCard = this.GetComponent<PlayerCard>();
        _rect = (RectTransform)this.transform;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _droppedSuccessfully = false;
        _canvasGroup.blocksRaycasts = false;

        if (_rootCanvas != null)
            this.transform.SetParent(_rootCanvas.transform, true);

        RectTransformUtility.ScreenPointToLocalPointInRectangle
        (
            (RectTransform)_rootCanvas.transform,
            eventData.position,
            eventData.pressEventCamera,
            out var localPoint
        );

        _pointerOffset = (Vector2)_rect.localPosition - localPoint;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (_rootCanvas == null) return;

        RectTransformUtility.ScreenPointToLocalPointInRectangle
        (
            (RectTransform)_rootCanvas.transform,
            eventData.position,
            eventData.pressEventCamera,
            out var localPoint
        );

        _rect.localPosition = localPoint + _pointerOffset;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _canvasGroup.blocksRaycasts = true;

        // 드랍 실패 시
        if (!_droppedSuccessfully)
        {
            this.transform.SetParent(_currentParent, false);
            _rect.anchoredPosition = Vector2.zero;
        }
    }

    public void HandleDroppedSuccessfully(Transform newParent)
    {
        var oldList = _currentParent != null ? _currentParent.GetComponent<CharacterList>() : null;

        _droppedSuccessfully = true;

        // 새 부모로 적용

        // 기존 부모 변경
        _currentParent = newParent;
        //_charList = newParent.GetComponent<CharacterList>();
    }
}
