using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class PlantPatch : GridObject
{
    public float m_progress; 
    [SerializeField] ItemSeed m_itemSeed; public ItemSeed m_ItemSeed
    {
        get { return m_itemSeed; }
        set
        {
            m_itemSeed = value;
            if (m_itemSeed == null) m_progress = 0.0f;
        }
    }
    public float m_dryness; //The current dryness of the plant
    [SerializeField] BoxCollider2D m_collider;
    [SerializeField] SpriteRenderer m_spiteRenderer;

    void Update()
    {
        //Do not update plant if no seed is assigned
        if (m_ItemSeed == null) return;

        //Make the plant patch dry overtime
        m_dryness -= Time.deltaTime;

        //Make the plant grow overtime
        if (m_progress < m_ItemSeed.m_finalProgress)
        {
            m_progress += Time.deltaTime;
        
            //Update sprites
            int spriteIndex = (int)((m_progress / m_ItemSeed.m_finalProgress) * (m_ItemSeed.m_growthStages.Length-1));
            if (spriteIndex > m_ItemSeed.m_growthStages.Length) spriteIndex = m_ItemSeed.m_growthStages.Length-1;
            m_spiteRenderer.sprite = m_ItemSeed.m_growthStages[spriteIndex];
        }
    }

    void OnTriggerStay2D(Collider2D _collision)
    {
        //Do not update plant if no seed is assigned
        if (m_ItemSeed == null) return;
        
        //Check the collided object
        if (_collision.gameObject == Player.m_current)
        {
            if (Player.m_current.m_interactAction.WasPressedThisFrame())
            {
                
            }
        }

    }
}
