using UnityEngine;
using UnityEngine.EventSystems;

public class CardSlot : MonoBehaviour, IDropHandler
{
    private bool _isOccupied = false;
    public bool IsOccupied { get => _isOccupied;}
    [SerializeField] private AudioClip _snapSFX;
    
    public void OnDrop(PointerEventData eventData)
    {
        if(transform.childCount == 0)
        {
            GameObject droppedObject = eventData.pointerDrag;
            Draggable draggableItem = droppedObject.GetComponent<Draggable>();
            draggableItem.parentAfterDragTransform = transform;
            draggableItem.currentCardGridSlot.Occupy(false);
            UIManager.Instance.PlayOneShotClip(_snapSFX);
            Occupy(true);
        }
    }

    public void Occupy(bool isOccupied)
    {
        _isOccupied = isOccupied;
    }
}
