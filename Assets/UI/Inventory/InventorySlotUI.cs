using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static InventorySystem;

    public class InventorySlotUI : MonoBehaviour
    {
        static InventorySlotUI m_heldScreenSlot;

        [SerializeField] Button m_button; //The button triggering the dragging and dropping
        [SerializeField] Canvas m_canvas; //The canvas that the player can drag and drop to distribute tiems to different slots
        [SerializeField] Image m_slotIcon; //The image component that displays what item the slot is storing
        [SerializeField] TMP_Text m_amountText; //The text component displaying how many items are stored in the slot
        
        public int m_slotIndex = -1; //What slot is being referenced
        [HideInInspector] public InventorySystem m_inventorySystem; //What inventory system does the slot belong to

        public void Init(InventorySystem _inventorySystem, int _slotIndex)
        {
            m_inventorySystem = _inventorySystem;
            m_slotIndex = _slotIndex;
            m_button.onClick.AddListener(() => { if (m_heldScreenSlot == null) Grab(); else m_heldScreenSlot.Drop(this); });
            m_inventorySystem.m_onChange += UpdateSlot; UpdateSlot();
        }

        void Update()
        {
            if (!IsValid()) return;

            if (m_heldScreenSlot == this) m_heldScreenSlot.m_canvas.transform.position = Mouse.current.position.value;
        }

        public void UpdateSlot(ref Slot _slot, int _slotIndex) { UpdateSlot(); }

        public void UpdateSlot()
        {
            if (!IsValid()) return;

            ref Slot slot = ref m_inventorySystem.m_slots[m_slotIndex];
            
            //Update slot data
            if (slot.IsValid())
            {
                m_canvas.gameObject.SetActive(true);
                m_slotIcon.sprite = slot.m_item.m_icon;
                m_amountText.text = slot.m_amount.ToString();
            }
            //Disable slot contents if it is empty
            else
            {
                m_canvas.gameObject.SetActive(false);
            }
        }

        public void Grab()
        {
            if (!IsValid()) return;

            //Do not grab if the slot is empty
            if (!m_canvas.gameObject.activeSelf) return;

            //Grab the item
            m_heldScreenSlot = this;
            m_canvas.transform.SetParent(gameObject.GetComponentInParent<Canvas>().transform);
            m_canvas.sortingOrder = 20;
        }

        public void Drop(InventorySlotUI _selectedScreenSlot)
        {
            if (!IsValid()) return;

            ref Slot thisSlot = ref m_inventorySystem.m_slots[m_slotIndex];
            ref Slot otherSlot = ref m_inventorySystem.m_slots[_selectedScreenSlot.m_slotIndex];

            //Drop the slot back into its original slot
            if (m_heldScreenSlot == _selectedScreenSlot) ResetSlot();

            //Drop the slot into an empty slot
            else if (!otherSlot.IsValid())
            {
                m_inventorySystem.SwapItem(m_slotIndex, _selectedScreenSlot.m_slotIndex, _selectedScreenSlot.m_inventorySystem);
                ResetSlot();
            }

            //Fill the slot into a slot with a matching item
            else if (otherSlot.m_item == thisSlot.m_item)
            {
                m_inventorySystem.FillSlot(m_slotIndex, _selectedScreenSlot.m_slotIndex, _selectedScreenSlot.m_inventorySystem);
                ResetSlot();
                if (thisSlot.IsValid()) m_heldScreenSlot = this;
            }

            //Swap the slots if the items do not match
            else
            {
                m_inventorySystem.SwapItem(m_slotIndex, _selectedScreenSlot.m_slotIndex, _selectedScreenSlot.m_inventorySystem);
                ResetSlot();
                m_heldScreenSlot = this;
            }
        }

        public void ResetSlot()
        {
            m_canvas.transform.SetParent(transform);
            m_canvas.transform.localPosition = Vector3.zero;
            m_heldScreenSlot = null;
            m_canvas.sortingOrder = 10;
        }

        bool IsValid()
        {
            return m_slotIndex > 0 && m_inventorySystem != null;
        }
    }
