using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class PlantPatch : MonoBehaviour
{
    public float m_progress; 
    public float m_finalProgress; //If progress reached final progress, the plant is ready to be plucked
    public float m_maxDryness; //The max dryness of the plant
    public float m_minDryness; //If below min dryness, the plant is dead
    public float m_dryness; //The current dryness of the plant
    [SerializeField] BoxCollider2D m_collider;
    [SerializeField] SpriteRenderer m_spiteRenderer;

    [SerializeField] GameObject m_plant;
    [SerializeField] Sprite[] m_growthStages;

    void Update()
    {
        //m_dryness -= Time.deltaTime;


        if (m_progress < m_finalProgress)
        {
            m_progress += Time.deltaTime;

            int spriteIndex = (int)((m_progress / m_finalProgress) * (m_growthStages.Length-1));
            if (spriteIndex > m_growthStages.Length) spriteIndex = m_growthStages.Length-1;
            m_spiteRenderer.sprite = m_growthStages[spriteIndex];
        }
    }

    void OnTriggerEnter2D(Collider2D _collision)
    {
        
    }

    void OnTriggerExit2D(Collider2D _collision)
    {

    }
}
