using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Untitled Weapon", menuName = "Weapon", order = 1)]
public class Weapon : MonoBehaviour
{
   [Serializable] public struct Hitbox
    {
        public Collider2D m_prefab;
        public bool m_rotateWithVelocity;
        public float m_projectileAngle;
        public float m_projectileSpeed;
        public float m_hitboxSpawnTime;
        public float m_hitboxDuration;
    }

    public AnimationClip m_attackFront, m_attackAngleFront, m_attackBack, m_attackAngleBack, m_attackSide;
    public Hitbox[] m_hitbox;
    public float m_cooldownDuration;
}
