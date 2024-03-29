using System.Collections.Generic;
using UnityEngine;

public static class PauseManager
{
    #region Classes, Structs, & Enums
    public enum PauseState { Gameplay, InteractionsPaused, Paused }
    public delegate void PauseStateChangeHandler(PauseState _newState);
    public class PauseHashSet
    {
        public HashSet<object> m_set = new HashSet<object>();
        public bool Add(object _item) { bool ret = m_set.Add(_item);  UpdatePauseState(); return ret; }
        public bool Remove(object _item) { bool ret = m_set.Remove(_item); UpdatePauseState(); return ret; }
    }

    #endregion

    public static PauseState m_pauseState { get; private set; } = PauseState.Gameplay;
    public static event PauseStateChangeHandler m_onPauseStateChanged;
    public static PauseHashSet m_interactionsPaused { get; private set; } = new PauseHashSet(); //When this list have a value stored in it,
                                                                                                //all interactions will be paused such as player movement,
                                                                                                //however detatime still runs allowing for background elements
                                                                                                //such as enviromental effects to still function.
    public static PauseHashSet m_paused { get; private set; } = new PauseHashSet();             //When this list have a value stored in it,
                                                                                                //everything is paused. This takes priority over the interactions paused list.
    
    static void UpdatePauseState()
    {
        //Remove num pause states
        m_paused.m_set.Remove(null);
        m_interactionsPaused.m_set.Remove(null);

        //Update pause state
        PauseState prevPauseState = m_pauseState;
        if (m_paused.m_set.Count > 0) m_pauseState = PauseState.Paused;
        else if (m_interactionsPaused.m_set.Count > 0) m_pauseState = PauseState.InteractionsPaused;
        else m_pauseState = PauseState.Gameplay;

        //Invoke pause state changed event
        if (prevPauseState != m_pauseState) m_onPauseStateChanged?.Invoke(m_pauseState);

        //Set time scale
        Time.timeScale = (m_pauseState == PauseState.Paused) ? Time.timeScale = 0.0f : Time.timeScale = 1.0f;
    }
}
