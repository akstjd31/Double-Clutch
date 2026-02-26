using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private Canvas rootCanvas;
    private Transform _currentParent;
    private RectTransform _rect;
    private bool _droppedSuccessfully;
    private Vector2 _pointerOffset;


    private void Awake()
    {
        _currentParent = this.transform.parent;
        _rect = (RectTransform)this.transform;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _droppedSuccessfully = false;

        if (rootCanvas != null)
            this.transform.SetParent(rootCanvas.transform, true);

        RectTransformUtility.ScreenPointToLocalPointInRectangle
        (
            (RectTransform)rootCanvas.transform,
            eventData.position,
            eventData.pressEventCamera,
            out var localPoint
        );

        _pointerOffset = (Vector2)_rect.localPosition - localPoint;
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        if (rootCanvas == null) return;

        RectTransformUtility.ScreenPointToLocalPointInRectangle
        (
            (RectTransform)rootCanvas.transform,
            eventData.position,
            eventData.pressEventCamera,
            out var localPoint
        );

        _rect.localPosition = localPoint + _pointerOffset;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!_droppedSuccessfully)
        {
            this.transform.SetParent(_currentParent, false);
            _rect.anchoredPosition = Vector2.zero;
        }
    }
}
