using UnityEngine;

namespace InventoryScreenNamespace
{
    public class InventoryScreen : MonoBehaviour
    {
        public InventorySlotUI[] m_screenSlots;

        void Start()
        {
            for (int slotIndex = 0; slotIndex < m_screenSlots.Length; slotIndex++)
            {
                InventorySlotUI inventorySlotUI = m_screenSlots[slotIndex];
                inventorySlotUI.Init(GameManager.m_current.m_PlayerInventory, slotIndex);
            }
        }
    }
}