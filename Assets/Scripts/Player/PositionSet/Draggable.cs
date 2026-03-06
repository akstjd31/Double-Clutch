using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CanvasGroup))]
public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private Canvas _rootCanvas;

    private Transform _originParent;
    private RectTransform _rect;
    private CanvasGroup _canvasGroup;
    private bool _droppedSuccessfully;
    private Vector2 _pointerOffset;

    private void Awake()
    {
        if (_rootCanvas == null)
            _rootCanvas = GameObject.FindAnyObjectByType<Canvas>();

        _rect = GetComponent<RectTransform>();
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (_rootCanvas == null) return;

        _droppedSuccessfully = false;
        _originParent = transform.parent;

        _canvasGroup.blocksRaycasts = false;

        transform.SetParent(_rootCanvas.transform, true);
        transform.SetAsLastSibling();

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
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

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
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

        // 드랍 실패면 원래 부모로 복귀
        if (!_droppedSuccessfully)
        {
            transform.SetParent(_originParent, false);
            ResetRect();
        }
    }

    public void SetDroppedSuccessfully()
    {
        _droppedSuccessfully = true;
    }

    private void ResetRect()
    {
        _rect.anchorMin = new Vector2(0.5f, 0.5f);
        _rect.anchorMax = new Vector2(0.5f, 0.5f);
        _rect.pivot = new Vector2(0.5f, 0.5f);
        _rect.anchoredPosition = Vector2.zero;
        _rect.localScale = Vector3.one;
        _rect.localRotation = Quaternion.identity;
    }
}