using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class Menu : MonoBehaviour
{
    public CanvasGroup m_canvasGroup;
    static List<Menu> m_menus = new List<Menu>(); public static List<Menu> m_Menus { get { return m_menus; } }
    [SerializeField] PauseManager.PauseState m_pauseState = PauseManager.PauseState.Paused;
    public bool m_isSubmenu;
    public bool m_syncInteractCastBlock;

    void Awake()
    {
        m_canvasGroup.interactable = false;
    }

    protected void OnEnable()
    {
        if (m_isSubmenu) m_canvasGroup.interactable = true;
        else
        {
            //Disable prev menu interactable
            if (m_menus.Count > 0) m_menus[^1].m_canvasGroup.interactable = false;

            //Add menu to list
            m_menus.Add(this);

            //Enable interactable
            m_canvasGroup.interactable = true;
        }

        //Sync the canvas group interactable with block raycasts
        if (m_syncInteractCastBlock) m_canvasGroup.blocksRaycasts = m_canvasGroup.interactable;

        //Add to pause manager
        switch (m_pauseState)
        {
            case PauseManager.PauseState.InteractionsPaused: PauseManager.m_interactionsPaused.Add(this); break;
            case PauseManager.PauseState.Paused: PauseManager.m_paused.Add(this); break;
        }
    }

    protected void OnDisable()
    {
        if (m_isSubmenu) m_canvasGroup.interactable = false;
        else
        {
            //Remove menu from list
            m_menus.Remove(this);

            //Disable interactable
            m_canvasGroup.interactable = false;

            //Set last menu to be interactable
            if (m_menus.Count > 0) m_menus[^1].m_canvasGroup.interactable = true;
        }

        //Sync the canvas group interactable with block raycasts
        if (m_syncInteractCastBlock) m_canvasGroup.blocksRaycasts = m_canvasGroup.interactable;

        //Remove from pause manager
        switch (m_pauseState)
        {
            case PauseManager.PauseState.InteractionsPaused: PauseManager.m_interactionsPaused.Remove(this); break;
            case PauseManager.PauseState.Paused: PauseManager.m_paused.Remove(this); break;
        }
    }
}
