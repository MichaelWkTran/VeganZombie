using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//Represents a slot position in m_slots in the InventorySystem class
public class InventoryScreenSquare : Selectable, IPointerClickHandler
{
    public InventoryScreenSlot m_inventoryMenuItem; //The slot it contains
    public Vector2Int m_slotPosition; //Slot position in the InventorySystem m_slots

    public void OnPointerClick(PointerEventData _eventData)
    {
        if (InventoryScreenSlot.m_heldInventoryScreenSlot == null) InventoryScreenSlot.GrabItem(m_inventoryMenuItem);
        else InventoryScreenSlot.DropItem(this);
    }
}
