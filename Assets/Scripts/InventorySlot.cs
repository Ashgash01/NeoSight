using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IDropHandler
{

    public Image image;
    public Sprite selectedSprite;
    public Sprite notSelectedSprite;

    private void Awake()
    {
        Deselect();
    }
    public void Select()
    {
        image.sprite = selectedSprite;
    }

    public void Deselect()
    {
        image.sprite = notSelectedSprite;
    }
    public void OnDrop(PointerEventData eventData)
    {
        if(transform.childCount == 0)
        {
            InventoryItem inventoryItem = eventData.pointerDrag.GetComponent<InventoryItem>();
            inventoryItem.parentAfterDrag = transform;
        }
    }
}
