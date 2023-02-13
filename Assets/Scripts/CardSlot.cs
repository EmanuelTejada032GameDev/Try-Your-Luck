using UnityEngine;
using UnityEngine.EventSystems;

public class CardSlot : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        if(transform.childCount == 0)
        {
            GameObject droppedObject = eventData.pointerDrag;
            Draggable draggableItem = droppedObject.GetComponent<Draggable>();
            draggableItem.parentAfterDragTransform = transform;
        }
        
    }
}
