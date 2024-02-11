using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static InventorySystem;

public class InvScreenSlot: MonoBehaviour
{
    static InvScreenSlot m_heldScreenSlot;

    [SerializeField] Button m_button;
    [SerializeField] Canvas m_canvas;
    [SerializeField] RectTransform m_contentRectTransform;
    [SerializeField] Image m_slotIcon;
    [SerializeField] TMP_Text m_amountText;
    InventoryScreen m_inventoryScreen;
    public int m_slotIndex { get; private set; } = -1;

    void Start()
    {
        m_inventoryScreen = FindObjectOfType<InventoryScreen>();
        m_slotIndex = Array.IndexOf(m_inventoryScreen.m_screenSlots, this);
        m_button.onClick.AddListener(() => { if (m_heldScreenSlot == null) Grab(); else m_heldScreenSlot.Drop(this); });
        GameManager.m_current.m_PlayerInventory.m_onChange += UpdateSlot; UpdateSlot();
    }

    void Update()
    {
        //Update the position of the slot when it is currently being dragged
        if (m_heldScreenSlot == this) m_heldScreenSlot.m_contentRectTransform.transform.position = Mouse.current.position.value;
    }

    public void UpdateSlot(ref Slot _slot, int _slotIndex)
    {
        UpdateSlot();
    }

    public void UpdateSlot()
    {
        ref Slot slot = ref GameManager.m_current.m_PlayerInventory.m_slots[m_slotIndex];

        //Update slot data
        if (slot.IsValid())
        {
            m_contentRectTransform.gameObject.SetActive(true);
            m_slotIcon.sprite = slot.m_item.m_icon;
            m_amountText.text = slot.m_amount.ToString();
        }
        //Disable slot contents if it is empty
        else
        {
            m_contentRectTransform.gameObject.SetActive(false);
        }
    }

    public void Grab()
    {
        //Do not grab if the slot is empty
        if (!m_contentRectTransform.gameObject.activeSelf) return;

        //Grab the item
        m_heldScreenSlot = this;
        m_contentRectTransform.transform.SetParent(gameObject.GetComponentInParent<Canvas>().transform);
        m_canvas.sortingOrder = 20;
    }

    public void Drop(InvScreenSlot _selectedScreenSlot)
    {
        ref Slot thisSlot = ref GameManager.m_current.m_PlayerInventory.m_slots[m_slotIndex];
        ref Slot otherSlot = ref GameManager.m_current.m_PlayerInventory.m_slots[_selectedScreenSlot.m_slotIndex];

        //Drop the slot back into its original slot
        if (m_heldScreenSlot == _selectedScreenSlot) ResetSlot();
        
        //Drop the slot into an empty slot
        else if (!otherSlot.IsValid())
        {
            GameManager.m_current.m_PlayerInventory.SwapItem(Array.IndexOf(m_inventoryScreen.m_screenSlots, this), Array.IndexOf(m_inventoryScreen.m_screenSlots, _selectedScreenSlot));
            ResetSlot();
        }

        //
        else if (otherSlot.m_item == thisSlot.m_item)
        {
            GameManager.m_current.m_PlayerInventory.FillSlot(Array.IndexOf(m_inventoryScreen.m_screenSlots, _selectedScreenSlot), Array.IndexOf(m_inventoryScreen.m_screenSlots, this));
            ResetSlot();
            if (thisSlot.IsValid() && thisSlot.m_amount > 0) m_heldScreenSlot = this;
        }

        //
        else if (otherSlot.m_item != thisSlot.m_item)
        {
            GameManager.m_current.m_PlayerInventory.SwapItem(Array.IndexOf(m_inventoryScreen.m_screenSlots, this), Array.IndexOf(m_inventoryScreen.m_screenSlots, _selectedScreenSlot));
            ResetSlot();
            m_heldScreenSlot = this;
        }
    }

    public void ResetSlot()
    {
        m_contentRectTransform.transform.SetParent(transform);
        m_contentRectTransform.localPosition = Vector3.zero;
        m_heldScreenSlot = null;
        m_canvas.sortingOrder = 10;
    }
}
