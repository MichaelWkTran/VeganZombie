using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    static public GameManager m_current { get; private set; } = null;
#if UNITY_EDITOR
    bool m_checkSingleton = false;
#endif
    [SerializeField] InventorySystem m_playerInventory = new InventorySystem(); public InventorySystem m_PlayerInventory { get { return m_playerInventory; } }

    public bool m_isDay { get; private set; } = true; //Whether it is currently day time or night time
    public delegate void OnDayNightChangeDelegate(bool _wasDay);
    public event OnDayNightChangeDelegate m_onDayNightChange; //Called when m_isDay has changed
    public float m_time { get; private set; } = 0.0f; //The current time in the day or night
    [SerializeField] float m_dayNightDuration = 0.0f; //How long does day or night time last?
    [SerializeField] CanvasGroup m_nightCanvasGroup;


    #region
    [Header("UI")]
    [SerializeField] Slider m_healthSlider;
    [SerializeField] Slider m_timerSlider;

    #endregion

    #region Events
    void Awake()
    {
        m_current = this;

        //Set Attributes
        {
            m_timerSlider.maxValue = m_dayNightDuration;
        }
    }


    void OnEnable()
    {
        IEnumerator SetPlayerAttributes()
        {
            yield return new WaitUntil(() => Player.m_current != null);
            
            m_healthSlider.maxValue = Player.m_current.m_MaxHealth;
            Player.m_current.m_onPropertyChanged += OnPlayerPropertyChanged;
            OnPlayerPropertyChanged();
        }
        StartCoroutine(SetPlayerAttributes());
    }

    void OnDisable()
    {
        if (Player.m_current != null) Player.m_current.m_onPropertyChanged -= OnPlayerPropertyChanged;
        
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

        ////Set Attributes
        //{
        //    m_healthSlider.maxValue = Player.m_current.m_MaxHealth;
        //}

        //Start the day
        SetIsDay(true);

        //Load Save Data
    }

    void Update()
    {
        if (Time.timeScale > 0.0f)
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

    void LateUpdate()
    {
        if (Time.timeScale > 0.0f)
        {
            m_timerSlider.value = m_time;
        }
    }

    void OnPlayerPropertyChanged()
    {
        m_healthSlider.value = Player.m_current.m_health;
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
