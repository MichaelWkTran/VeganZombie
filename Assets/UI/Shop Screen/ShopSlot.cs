using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static InventorySystem;

namespace ShopScreenNamespace
{
    public class ShopSlot : MonoBehaviour
    {
        public Item m_item; //The item to be sold
        public uint m_price; //The price of the item

        [SerializeField] Button m_button; //The button component used for purchasing the item
        [SerializeField] Image m_slotIcon; //The image component displaying the item to be sold
        [SerializeField] TMP_Text m_priceText; //The text component showing how much money required to purchase the item

        ShopScreen m_shopScreen;

        void Start()
        {
            m_shopScreen = GetComponentInParent<ShopScreen>();
            InventorySystem inventorySystem = GameManager.m_current.m_PlayerInventory;

            m_button.onClick.AddListener(() => m_shopScreen.OpenConfirmPurchasePopup(this));
            inventorySystem.m_onChange += UpdateSlot; UpdateSlot();
        }

        void UpdateSlot(ref Slot _slot, int _slotIndex) { UpdateSlot(); }

        public void UpdateSlot()
        {
            InventorySystem inventorySystem = GameManager.m_current.m_PlayerInventory;

            m_slotIcon.sprite = m_item.m_icon;
            m_priceText.text = m_price.ToString();
            
            //Allow the player to purchase the item if they have enough money as well as they have enough space in their inventory
            m_button.interactable = inventorySystem.CountSpaceRemaining(m_item) > 0 && GameManager.m_current.m_money >= m_price;
        }
    }
}