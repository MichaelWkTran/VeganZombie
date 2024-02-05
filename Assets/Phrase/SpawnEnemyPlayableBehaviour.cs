using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class SpawnEnemyControlClip : PlayableAsset, ITimelineClipAsset
{
#if UNITY_EDITOR
    [NonSerialized] public TimelineClip m_clipPassthrough = null;
#endif

    public ClipCaps clipCaps { get { return ClipCaps.None; } }
    [SerializeField] SpawnEnemyPlayableBehaviour m_template = new SpawnEnemyPlayableBehaviour();

    public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
    {
#if UNITY_EDITOR
        //Set Playable Behaviour Properties
        {
            m_template.m_startTime = m_clipPassthrough.start;
            m_template.m_endTime = m_clipPassthrough.end;
        }
        
#endif

        return ScriptPlayable<SpawnEnemyPlayableBehaviour>.Create(graph, m_template);
    }
}
