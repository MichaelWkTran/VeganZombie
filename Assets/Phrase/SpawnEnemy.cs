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
    [SerializeField] public ulong m_startFrame; //The start frame of the clip
    [SerializeField] public ulong m_endFrame; //The ending frame of the clip

    public Enemy[] m_enemyPrefabs; //What enemy(ies) to spawn
    public bool m_spawnFromTombstone; //Whether to spawn the enemies from a tombstone that have been removed
    public Vector2 m_spawnLocation; //If not spawning from tombstone, specify spawn location
    public uint m_spawnNumber = 1U; //How many enemies to spawn during the length of the clip
    ulong m_spawnFrameDuration = 0; //How long does it take before the next enemy is spawned

    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        if (m_spawnNumber > 1) m_spawnFrameDuration = (m_endFrame - m_startFrame) / (m_spawnNumber - 1);
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
        if ((_info.frameId - m_startFrame) % m_spawnFrameDuration == 0) SpawnEnemy();
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