using UnityEngine;
using UnityEngine.EventSystems;

public class DropPosition : MonoBehaviour, IDropHandler
{
    [SerializeField] private bool allowOnlyOne = false;
    [SerializeField] private bool isPositionSlot = false;
    [SerializeField] private CharacterList _charList;

    public void OnDrop(PointerEventData eventData)
    {
        var draggedObj = eventData.pointerDrag;
        if (draggedObj == null) return;

        var draggable = draggedObj.GetComponent<Draggable>();
        if (draggable == null) return;

        // 이미 배치되어 있는 상태면 교체하도록 해야하는데 일단 리턴
        // 이 스크립트를 재사용하는 부분이 있어 위 포지션 배치는 allowOnlyOne을 체크한 상태로 할 것!
        if (allowOnlyOne && this.transform.childCount > 0) return;

        draggable.HandleDroppedSuccessfully(this.transform);

        var card = draggedObj.GetComponent<PlayerCard>();
        if (card == null) return;

        if (isPositionSlot)
        {
            _charList.RemoveOnPosition(card);
            _charList.AddOnPosition(card);
        }
        else
        {
            _charList.AddOnPosition(card);
            _charList.RemoveOnPosition(card);
        }
    }
}
