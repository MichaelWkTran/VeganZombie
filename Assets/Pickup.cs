using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Pickup : MonoBehaviour
{
    [SerializeField] Item m_item;
    [SerializeField] byte m_amount;
    [SerializeField] Image m_icon;
    [SerializeField] TMP_Text m_amountText;

    void Start()
    {
        OnEnable();

        //Set amount text and icon
        m_icon.sprite = m_item.m_icon;
        if (m_amount > 1) m_amountText.text = m_amount.ToString(); else m_amountText.gameObject.SetActive(false);
    }

    void OnEnable()
    {
        if (GameManager.m_current == null) return;
        GameManager.m_current.m_onDayNightChange += OnDayNightChange;
    }

    void OnDisable()
    {
        if (GameManager.m_current == null) return;
        GameManager.m_current.m_onDayNightChange -= OnDayNightChange;
    }

    void OnTriggerEnter2D(Collider2D _collision)
    {
        //Make sure the game manager exists
        if (GameManager.m_current == null) return;
        
        //Make sure the player collided with the pickup
        if (Player.m_current == null || _collision.gameObject != Player.m_current.gameObject) return;

        //Check whether the item can be added to the inventory
        if (GameManager.m_current.m_PlayerInventory.CheckAddItem(m_item, m_amount) > 0) return;

        //Pickup Item
        GameManager.m_current.m_PlayerInventory.AddItem(m_item, m_amount);
        DestroyPickup();
    }

    void OnDayNightChange(bool _wasDay)
    {
        //Destroy the pickup once the day changes to night or vice versa
        if (this != null)
        {
            DestroyPickup();
        }
    }

    void DestroyPickup()
    {
        Destroy(gameObject);
    }
}