using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ShopScreenNamespace
{
    public class ShopScreen : MonoBehaviour
    {
        public LayoutGroup m_shopSlotContainer; //The layout group containing all the shop items as its children
        [ReadOnly] public ShopSlot m_selectedShopSlot; //The shop slot that the player selected to purchase

        [Serializable] public struct ConfirmPurchasePopup
        {
            public Menu m_menu;
            public RectTransform m_pannel;
            public IntegerInputField m_quantityInputField;
        }
        public ConfirmPurchasePopup m_confirmPurchasePopup; //The popup menu that appears when selecting an item



        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        void OnEnable()
        {
            
        }

        void OnDisable()
        {
            
        }

        public void OpenConfirmPurchasePopup(ShopSlot _selectedShopSlot)
        {
            //Select Shop Slot
            m_selectedShopSlot = _selectedShopSlot;

            //Open Popup
            m_confirmPurchasePopup.m_menu.gameObject.SetActive(true);

            //Place the popup above or underneath the inventory slot
            {
                //Save parent and sibling index of the popup
                Transform itemPopupParent = m_confirmPurchasePopup.m_pannel.parent;
                int itemPopupSiblingIndex = m_confirmPurchasePopup.m_pannel.GetSiblingIndex();

                //Set the parent of the popup to the item slot to allow anchor positions relative to the slot
                m_confirmPurchasePopup.m_pannel.SetParent(m_selectedShopSlot.transform);

                //Set anchors
                m_confirmPurchasePopup.m_pannel.anchorMax = m_confirmPurchasePopup.m_pannel.anchorMin =
                    (Camera.main.WorldToScreenPoint(m_selectedShopSlot.transform.position).y - (Screen.height * 0.5f) < 0.0f) ?
                    Vector2.up : Vector2.zero;

                //Set the position of the popup
                m_confirmPurchasePopup.m_pannel.anchoredPosition = Vector2.zero;

                //Restore the parent of the popup
                m_confirmPurchasePopup.m_pannel.SetSiblingIndex(itemPopupSiblingIndex);
                m_confirmPurchasePopup.m_pannel.SetParent(itemPopupParent);
            }

            //Reset input field for amount of the selected item to purchase
            m_confirmPurchasePopup.m_quantityInputField.m_Value = 1;

            //Set the max value of input field for capping the max amount of the selected item the player is able to purchase based on how much money they currently have
            if (m_selectedShopSlot.m_price > 0) m_confirmPurchasePopup.m_quantityInputField.m_maxValue = (int) (GameManager.m_current.m_money / m_selectedShopSlot.m_price);
        }

        public void ConfirmPurchase()
        {
            m_confirmPurchasePopup.m_menu.gameObject.SetActive(false);
        }
    }

}