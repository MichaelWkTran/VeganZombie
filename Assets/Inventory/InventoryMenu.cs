using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class InventoryMenu : MonoBehaviour
{
    public InventorySystem m_inventorySystem { get; private set; }
    [SerializeField] InventoryScreenSquare m_slotPrefab;
    [SerializeField] LayoutGroup m_statusItemContents; //Where the status item slots are containted
    [SerializeField] LayoutGroup m_weaponItemContents; //Where the weapon item slots are containted
    [SerializeField] LayoutGroup m_armorItemContents; //Where the armor item slots are containted
    [SerializeField] LayoutGroup m_keyItemContents; //Where the key item slots are containted
    [SerializeField] TMP_Text m_itemDescription; //The textbox that shows the description of the item that is currently highlighted

    [Header("Item Popup")]
    [SerializeField] RectTransform m_itemPopupMenu; //The menu that appears when the player have selected an item
    [SerializeField] RectTransform m_itemPopupMenuPanel; //The pannel containting popup menu
    [SerializeField] RectTransform m_consumeMenu; //The menu that appears to allow the player to choose which party member to use the item on
    [SerializeField] RectTransform m_discardMenu; //The menu that appears for the player to choose how many of the selected item to discard
    [SerializeField] IntegerInputField m_discardNumberInputField; //The UI element used for selecting how many of the selected item to discard
    InventoryScreenSquare m_selectedItem;

    //void Start()
    //{
    //    m_inventorySystem = GameManager.m_current.m_PlayerInventory;
    //
    //    //Loop through all inventory slots
    //    foreach (var slot in m_inventorySystem.m_slots)
    //    {
    //        //Create slots in UI for each catergory
    //        UpdateSlots(new KeyValuePair<Item, InventorySystem.Slot>(slot.Key, slot.Value));
    //    }
    //
    //    m_inventorySystem.m_onChange += UpdateSlots;
    //}
    //
    //void UpdateSlots(KeyValuePair<Item, InventorySystem.Slot> _slot)
    //{
    //    var inventoryMenuSlots = GetComponentsInChildren<InventoryMenuSlot>();
    //    var categoryTransformMap = new Dictionary<Item.Category, Transform>
    //    {
    //        { Item.Category.Status, m_statusItemContents.transform },
    //        { Item.Category.Weapon, m_weaponItemContents.transform },
    //        { Item.Category.Armor, m_armorItemContents.transform },
    //        { Item.Category.Key, m_keyItemContents.transform }
    //    };
    //    var existingSlot = inventoryMenuSlots.FirstOrDefault(i => i.m_Item == _slot.Key);
    //
    //    //Create slot in UI for each catergory
    //    if (existingSlot == null)
    //    {
    //        foreach (var category in _slot.Key.m_category)
    //        {
    //            if (categoryTransformMap.TryGetValue(category, out var transform))
    //            {
    //                var instantiatedSlot = Instantiate(m_slotPrefab, transform);
    //                instantiatedSlot.Initialize(_slot.Key);
    //                instantiatedSlot.m_Button.onClick.AddListener(delegate { ShowItemPopup(instantiatedSlot); });
    //            }
    //        }
    //    }
    //    //If slot is empty, destroy the slot
    //    else if (_slot.Value.m_amount == 0)
    //    {
    //        Destroy(existingSlot.gameObject);
    //    }
    //}
    //
    //public void UpdateItemDescription(string _newDescription = "")
    //{
    //    m_itemDescription.text = _newDescription;
    //}
    //
    //public void ShowItemPopup(InventoryMenuSlot _selectedItem)
    //{
    //    m_itemPopupMenu.gameObject.SetActive(true);
    //    m_selectedItem = _selectedItem;
    //    HideItemPopupSubmenus();
    //
    //    //Place the popup above or underneath the inventory slot
    //    {
    //        //Save parent and sibling index of the popup
    //        Transform itemPopupParent = m_itemPopupMenuPanel.parent;
    //        int itemPopupSiblingIndex = m_itemPopupMenuPanel.GetSiblingIndex();
    //
    //        //Set the parent of the popup to the item slot to allow anchor positions relative to the slot
    //        m_itemPopupMenuPanel.SetParent(m_selectedItem.transform);
    //
    //        //Set anchors
    //        m_itemPopupMenuPanel.anchorMax = m_itemPopupMenuPanel.anchorMin =
    //            (Camera.main.WorldToScreenPoint(m_selectedItem.transform.position).y - (Screen.height * 0.5f) < 0.0f) ?
    //            Vector2.up : Vector2.zero;
    //
    //        //Set the position of the popup
    //        m_itemPopupMenuPanel.anchoredPosition = Vector2.zero;
    //
    //        //Restore the parent of the popup
    //        m_itemPopupMenuPanel.SetSiblingIndex(itemPopupSiblingIndex);
    //        m_itemPopupMenuPanel.SetParent(itemPopupParent);
    //    }
    //}
    //
    //public void ConfirmDiscardItem()
    //{
    //    m_inventorySystem.SubtractItem(m_selectedItem.m_Item, (byte)m_discardNumberInputField.GetValue());
    //}
    //
    //public void HideItemPopupSubmenus()
    //{
    //    m_consumeMenu.gameObject.SetActive(false);
    //    m_discardMenu.gameObject.SetActive(false);
    //}
    //
    //public void HideItemPopup()
    //{
    //    m_itemPopupMenu.gameObject.SetActive(false);
    //}
}