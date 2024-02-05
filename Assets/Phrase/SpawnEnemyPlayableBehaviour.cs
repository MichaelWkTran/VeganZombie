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
            PlayableDirector director = graph.GetResolver() as PlayableDirector;
            TimelineAsset timelineAsset = director.playableAsset as TimelineAsset;
            double fps = timelineAsset.editorSettings.frameRate;

            m_template.m_startFrame = (ulong)(m_clipPassthrough.start * fps);
            m_template.m_endFrame = (ulong)(m_clipPassthrough.end * fps);
        }
        
#endif

        return ScriptPlayable<SpawnEnemyPlayableBehaviour>.Create(graph, m_template);
    }
}
