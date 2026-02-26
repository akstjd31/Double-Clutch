using UnityEngine;
using UnityEngine.EventSystems;

public class DropPosition : MonoBehaviour, IDropHandler
{
    [SerializeField] private bool allowOnlyOne = false;

    public void OnDrop(PointerEventData eventData)
    {
        var draggedObj = eventData.pointerDrag;
        if (draggedObj == null) return;

        var draggable = draggedObj.GetComponent<Draggable>();
        if (draggable == null) return;

        // 이미 배치되어 있는 상태면 교체하도록 해야하는데 일단 리턴
        if (this.transform.childCount > 0) return;

        // draggable.HandleDroppedSuccessfully(this.transform);
    }
}
