using System.Collections;
using UnityEngine;

public abstract class UsableItem : Item
{
    public AnimationClip m_useItemFront, m_useItemAngleFront, m_useItemBack, m_useItemAngleBack, m_useItemSide;
    public float m_cooldownDuration;
    public bool m_useContinuously;

    public abstract IEnumerator UseItem(Collider2D _userCollider, Vector3 _spawnPos = default, Vector2 _lookDir = default);
}
