using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    static public GameManager m_current { get; private set; } = null;
#if UNITY_EDITOR
    bool m_checkSingleton = false;
#endif

    [Header("Player")]
    [SerializeField] float m_maxHealth; public float m_MaxHealth { get { return m_maxHealth; } }
    float m_health; public float m_Health
    {
        get { return m_health; }
        set { m_healthSlider.value = m_health = value; }
    }
    [SerializeField] InventorySystem m_playerInventory; public InventorySystem m_PlayerInventory { get { return m_playerInventory; } }
    public int m_selectedHotbarSlot = 0;
    public InventorySystem.Slot m_SelectedHotbarSlot { get { return m_PlayerInventory.m_slots[m_selectedHotbarSlot]; } }


    [Header("Day Night Cycle")]
    public bool m_progressTime = true; //Whether the day and night cycle should be paused
    public float m_time { get; private set; } = 0.0f; //The current time in the day or night
    public bool m_isDay { get; private set; } = true; //Whether it is currently day time or night time
    public delegate void OnDayNightChangeDelegate(bool _wasDay);
    public event OnDayNightChangeDelegate m_onDayNightChange; //Called when m_isDay has changed
    [SerializeField] float m_dayNightDuration = 0.0f; //How long does day or night time last?
    [SerializeField] CanvasGroup m_nightCanvasGroup;

    [Header("Miscellaneous")]
    [SerializeField] Tilemap m_ploughableTilemap; public Tilemap m_PloughableTilemap { get { return m_ploughableTilemap; } }
    [SerializeField] GridManager m_gridManager; public GridManager m_GridManager { get { return m_gridManager; } }


    #region UI
    [Header("UI")]
    [SerializeField] Slider m_healthSlider;
    [SerializeField] Slider m_timerSlider;
    [SerializeField] ToggleGroup m_hotbarToggleGroup;

    #endregion

    #region Events
    void Awake()
    {
        m_current = this;

        //Set Attributes
        {
            m_Health = m_healthSlider.maxValue = m_maxHealth;
            m_timerSlider.maxValue = m_dayNightDuration;
        }
    }

    void Start()
    {
#if UNITY_EDITOR
        //Print errors when there is more than one overworld manager in the scene.
        if (!m_checkSingleton)
        {
            var gameManagers = FindObjectsOfType<GameManager>();
            if (gameManagers.Length > 1)
                foreach (GameManager gameManager in gameManagers)
                    Debug.LogError("Can not have more than one game manager in a scene", gameManager);
        }

#endif

        //Start the day
        SetIsDay(true);

        //Load Save Data
    }

    void Update()
    {
        if (Time.timeScale > 0.0f)
        {
            if (m_progressTime)
            {
                //Progress the time of day
                m_time += Time.deltaTime;

                //Set placeholder night canvas alpha
                m_nightCanvasGroup.alpha = m_time / m_dayNightDuration;
                if (!m_isDay) m_nightCanvasGroup.alpha = 1.0f - m_nightCanvasGroup.alpha;

                //Swap from daytime to night time
                if (m_time > m_dayNightDuration) SetIsDay(!m_isDay);
            }            
        }
    }

    void LateUpdate()
    {
        if (Time.timeScale > 0.0f)
        {
            m_timerSlider.value = m_time;
        }
    }

    #endregion

    public void SetIsDay(bool _isDay)
    {
        bool wasDay = m_isDay;
        m_isDay = _isDay;
        m_onDayNightChange?.Invoke(wasDay);
        m_time = 0.0f;

        //Set up day time
        if (wasDay) DayTimeEnd();
        else NightTimeEnd();

        if (m_isDay) DayTimeStart();
        else NightTimeStart();
    }

    #region Daytime
    //[Header("Day Time")]

    void DayTimeStart()
    {
        Debug.Log("Day Time");
    }

    void DayTimeEnd()
    {

    }

    #endregion

    #region Night Time
    [Header("Night Time")]
    [SerializeField] PlayableDirector m_playableDirector; //Used for spawning enemies in night time

    void NightTimeStart()
    {
        Debug.Log("Night Time");

        m_playableDirector.Play();
    }

    void NightTimeEnd()
    {
        m_playableDirector.Stop();
        
        //Destroy Enemies
        foreach (Enemy enemy in FindObjectsOfType<Enemy>()) Destroy(enemy.gameObject);
    }

    #endregion
}
