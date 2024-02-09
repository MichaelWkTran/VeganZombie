using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static InventorySystem;

public class InvHotbarSlot: MonoBehaviour
{
    [SerializeField] Toggle m_toggle;
    [SerializeField] RectTransform m_contentRectTransform;
    [SerializeField] Image m_slotIcon;
    [SerializeField] TMP_Text m_amountText;
    Slot m_slot;
    
    void Start()
    {
        GameManager.m_current.m_PlayerInventory.m_onChange += UpdateSlot;
        UpdateSlot();
    }

    public void UpdateSlot(Slot _slot = null)
    {
        m_slot = GameManager.m_current.m_PlayerInventory.m_slots[transform.GetSiblingIndex()];

        //Disable slot contents if it is empty
        if (m_slot == null || m_slot.m_amount <= 0)
        {
            m_toggle.interactable = false;
            m_contentRectTransform.gameObject.SetActive(false);
        }
        //Update slot data
        else
        {
            m_toggle.interactable = true;
            m_contentRectTransform.gameObject.SetActive(true);
            m_slotIcon.sprite = m_slot.m_item.m_icon;
            m_amountText.text = m_slot.m_amount.ToString();
        }
    }
}