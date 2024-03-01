using System;
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
    public int m_slotIndex { get; private set; } = -1;

    void Start()
    {
        m_slotIndex = transform.GetSiblingIndex();
        m_toggle.onValueChanged.AddListener(delegate { OnValueChanged(); });
        GameManager.m_current.m_PlayerInventory.m_onChange += UpdateSlot; UpdateSlot();
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
            //m_toggle.interactable = true;
            m_contentRectTransform.gameObject.SetActive(true);
            m_slotIcon.sprite = slot.m_item.m_icon;
            m_amountText.text = slot.m_amount.ToString();
        }
        //Disable slot contents if it is empty
        else
        {
            //m_toggle.interactable = false;
            m_contentRectTransform.gameObject.SetActive(false);
        }
    }

    void OnValueChanged()
    {
        if (!GameManager.m_current) return;
        GameManager.m_current.m_selectedHotbarSlot = m_slotIndex;
    }
}
