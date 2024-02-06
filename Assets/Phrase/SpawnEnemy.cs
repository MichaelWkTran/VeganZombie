using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;

[TrackBindingType(typeof(GameManager)), TrackClipType(typeof(SpawnEnemyControlClip)), TrackColor(0.0f, 0.0f, 1.0f)]
public class SpawnEnemyTrack : TrackAsset
{
#if UNITY_EDITOR
    public override Playable CreateTrackMixer(PlayableGraph _graph, GameObject _go, int _inputCount)
    {
        var clips = GetClips(); foreach (var clip in clips)
        {
            var loopClip = clip.asset as SpawnEnemyControlClip;
            loopClip.m_clipPassthrough = clip;
        }

        return Playable.Create(_graph, _inputCount);
    }

#endif
}

[System.Serializable]
public class SpawnEnemyPlayableBehaviour : PlayableBehaviour
{
    
    [SerializeField] public double m_startTime; //The start frame of the clip
    [SerializeField] public double m_endTime; //The ending frame of the clip

    public Enemy[] m_enemyPrefabs; //What enemy(ies) to spawn
    public bool m_spawnFromTombstone; //Whether to spawn the enemies from a tombstone that have been removed
    public Vector2 m_spawnLocation; //If not spawning from tombstone, specify spawn location
    public uint m_spawnNumber = 1U; //How many enemies to spawn during the length of the clip
    double m_spawnTimeDuration = 0.0; //How long does it take before the next enemy is spawned
    double m_currentTime = 0.0f;

    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        if (m_spawnNumber > 1) m_spawnTimeDuration = (m_endTime - m_startTime) / (m_spawnNumber - 1);
        SpawnEnemy();
    }

    public override void ProcessFrame(Playable _playable, FrameData _info, object _playerData)
    {
        //Only process during playmode
        if (!Application.isPlaying) return;
        
        //Get Game Manager
        GameManager gameManager = GameManager.m_current;
        if (gameManager == null) return;

        //Spawn Enemy
        m_currentTime += _info.deltaTime;
        if (m_currentTime > m_spawnTimeDuration) { SpawnEnemy(); m_currentTime -= m_spawnTimeDuration; }
    }

    void SpawnEnemy()
    {
        Vector2 spawnLocation = m_spawnLocation;
        if (m_spawnFromTombstone)
        {

        }

        MonoBehaviour.Instantiate(m_enemyPrefabs[Random.Range(0, m_enemyPrefabs.Length)], spawnLocation, Quaternion.identity);
    }
}