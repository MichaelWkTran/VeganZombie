using UnityEngine;
using UnityEngine.InputSystem;
using static InventorySystem;

public class InventoryScreenSlot : MonoBehaviour
{
    RectTransform m_rectTransform;
    public InventoryScreenSquare m_inventoryScreenSquare;
    public static InventoryScreenSlot m_heldInventoryScreenSlot { get; private set; }
    public Slot m_slot;

    void Awake()
    {
        //Get Components
        m_rectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        if (m_heldInventoryScreenSlot == this) m_rectTransform.position = Mouse.current.position.value;
    }

    public static void GrabItem(InventoryScreenSlot _grabbedItem)
    {
        _grabbedItem.transform.SetParent(_grabbedItem.GetComponentInParent<Canvas>().transform);
        m_heldInventoryScreenSlot = _grabbedItem;
    }

    public static void DropItem(InventoryScreenSquare _targetScreenSlot = null)
    {
        //Only drop the item on a slot
        if (_targetScreenSlot == null)
        {

        }

        //
        if (_targetScreenSlot == m_heldInventoryScreenSlot.m_inventoryScreenSquare)
        {

        }

        //if (_targetScreenSlot.m_slot.m_item == m+)
        _targetScreenSlot.m_inventoryMenuItem = m_heldInventoryScreenSlot;
    }
}
