using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DropPosition : MonoBehaviour, IDropHandler, IPointerClickHandler
{
    [SerializeField] private Outline _outline;
    [SerializeField] private bool allowOnlyOne = false;
    [SerializeField] private bool isPositionSlot = false;
    [SerializeField] private CharacterList _charList;
    [SerializeField] private Position _position;

    private void OnEnable()
    {
        if (!isPositionSlot) return;

        if (_outline == null)
            _outline = GetComponent<Outline>();

        if (_outline != null)
            _outline.enabled = false;
    }

public void OnDrop(PointerEventData eventData)
{
    var draggedObj = eventData.pointerDrag;
    if (draggedObj == null) return;

    var draggable = draggedObj.GetComponent<Draggable>();
    if (draggable == null) return;

    var card = draggedObj.GetComponent<PlayerCard>();
    if (card == null) return;

    bool success = false;

    if (isPositionSlot)
    {
        success = _charList.AddOnPosition(card, this);

        if (success)
            card.Player.SetMatchPosition(_position);
    }
    else
    {
        success = _charList.MoveToCardList(card);
        if (success)
            card.Player.SetMatchPosition(Position.None);
    }

    if (success)
        draggable.SetDroppedSuccessfully();
}

    public void SetSelected(bool on)
    {
        if (_outline != null)
            _outline.enabled = on;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_charList != null)
            _charList.OnClickPosition(this);
    }

    public Position GetPosition() => _position;
}