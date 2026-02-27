using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DropPosition : MonoBehaviour, IDropHandler, IPointerClickHandler
{
    [SerializeField] private Outline _outline;
    [SerializeField] private bool allowOnlyOne = false;
    [SerializeField] private bool isPositionSlot = false;
    [SerializeField] private CharacterList _charList;
    [SerializeField] Position _position;
    private void OnEnable()
    {
        if (isPositionSlot)
        {
            if (_outline == null)
                _outline = this.GetComponent<Outline>();

            _outline.enabled = false;
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        var draggedObj = eventData.pointerDrag;
        if (draggedObj == null) return;

        var draggable = draggedObj.GetComponent<Draggable>();
        if (draggable == null) return;

        // 이 스크립트를 재사용하는 부분이 있어 위 포지션 배치는 allowOnlyOne을 체크한 상태로 할 것!
        if (allowOnlyOne && this.transform.childCount > 0) return;

        draggable.HandleDroppedSuccessfully(this.transform);

        var card = draggedObj.GetComponent<PlayerCard>();
        if (card == null) return;

        // 포지션 슬롯인지 아닌지에 따라 포함될 리스트 위치도 다름
        if (isPositionSlot)
        {
            _charList.AddOnPosition(card, this);
        }
        else
        {
            _charList.RemoveOnPosition(card);
        }
        card.Player.SetMatchPosition(_position);
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
}
