using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class PlantPatch : GridObject, IDamageable
{
    [SerializeField] ItemSeed m_itemSeed; public ItemSeed m_ItemSeed
    {
        get { return m_itemSeed; }
        set
        {
            m_itemSeed = value;

            //Reset the plant patch
            if (m_itemSeed == null)
            {
                m_dryness = 0.0f;
                m_progress = 0.0f;
                m_plantSpriteRenderer.sprite = null;
            }
        }
    }
    public float m_progress; //Seed growth progress. Ready to harvest when final threshold is passed.
    public float m_dryness; //The current dryness of the plant
    public float m_health; //The health of the plant, depletes when attacked by enemies
    public bool m_isDead; //Whether the plant is killed
    public bool m_isOverlapPlayer; //Whether the patch is overlapping with the player
    [SerializeField] SpriteRenderer m_plantSpriteRenderer;
    Color m_dryColour = new Color(0.5471698f, 0.3159081f, 0.0f, 1.0f);
    //[SerializeField] BoxCollider2D m_collider;

    void OnEnable() { GameManager.m_current.m_onDayNightChange += OnDayNightChange; }

    void OnDisable() { GameManager.m_current.m_onDayNightChange -= OnDayNightChange; }

    void OnDayNightChange(bool _wasDay)
    {
        if (m_isDead) return;

        if (m_ItemSeed != null)
        {
            //Reset health
            m_health = m_ItemSeed.m_health;
        }
    }

    void Update()
    {
        if (m_isDead) return;

        //Do not update plant if no seed is assigned
        if (m_ItemSeed == null) return;

        //Make the plant patch dry overtime
        m_dryness += Time.deltaTime;
        m_plantSpriteRenderer.color = Color.Lerp(Color.white, m_dryColour, Mathf.Max(0.0f, m_dryness) / m_ItemSeed.m_maxDryness);
        if (m_dryness > m_ItemSeed.m_maxDryness) Kill();

        //Make the plant grow overtime
        if (m_progress < m_ItemSeed.m_finalProgress)
        {
            m_progress += Time.deltaTime;

            //Update sprites
            int spriteIndex = (int)((m_progress / m_ItemSeed.m_finalProgress) * (m_ItemSeed.m_growthStages.Length - 1));
            if (spriteIndex > m_ItemSeed.m_growthStages.Length) spriteIndex = m_ItemSeed.m_growthStages.Length - 1;
            m_plantSpriteRenderer.sprite = m_ItemSeed.m_growthStages[spriteIndex];
        }

        //Harvest Vegtable
        if (m_isOverlapPlayer && Player.m_current.m_interactAction.WasPressedThisFrame()) Harvest();
    }

    void OnTriggerEnter2D(Collider2D _collision)
    {
        //Set overlap player
        if (Player.m_current != null && _collision.gameObject == Player.m_current.gameObject) m_isOverlapPlayer = true;
    }

    void OnTriggerExit2D(Collider2D _collision)
    {
        //Set overlap player
        if (Player.m_current != null && _collision.gameObject == Player.m_current.gameObject) m_isOverlapPlayer = false;
    }

    public void Damage(float _damage, Vector2 _hitImpulse = new Vector2(), bool _playDamageAnimation = true)
    {
        if (m_isDead) return;

        m_health -= _damage;

        //Kill the plant
        if (m_health < 0) Kill();
    }

    public void Harvest()
    {
        if (m_isDead) return;

        //Ensure the plant patch have a plant to harvest
        if (m_ItemSeed == null) return;

        //Add the harvested item to the inventory
        if (GameManager.m_current.m_PlayerInventory.AddItem(m_itemSeed.m_harvestedItem, 1) > 0) return;

        //If harvested item is sucessfully added, reset the plant patch
        m_ItemSeed = null;
    }

    public void Kill()
    {
        if (m_isDead) return;

        m_isDead = true;
        m_plantSpriteRenderer.sprite = m_ItemSeed.m_deadSprite;
        m_plantSpriteRenderer.color = Color.white;
    }
}
