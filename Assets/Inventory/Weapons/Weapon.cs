using System;
using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Untitled Weapon", menuName = "Tool/Weapon", order = 1)]
public class Weapon : UsableItem
{
   [Serializable] public struct HitboxData
    {
        public Collider2D m_prefab;
        public float m_hitboxDamage;
        public bool m_rotateWithVelocity;
        public float m_projectileAngle;
        public float m_projectileSpeed;
        public float m_hitboxSpawnTime;
        public float m_hitboxDuration;
    }
    public HitboxData[] m_hitbox;

    public override IEnumerator UseItem(Collider2D _userCollider, Vector3 _spawnPos = default, Vector2 _lookDir = default)
    {
        //Shoot projectiles
        foreach (HitboxData hitbox in m_hitbox)
        {
            //Spawn hitbox after a few seconds
            yield return new WaitForSeconds(hitbox.m_hitboxSpawnTime);

            //Spawn hitbox
            Collider2D spawnedhitbox = Instantiate(hitbox.m_prefab, _spawnPos, Quaternion.identity);
            Destroy(spawnedhitbox.gameObject, hitbox.m_hitboxDuration);
            Physics2D.IgnoreCollision(_userCollider, spawnedhitbox);
            Physics2D.IgnoreCollision(spawnedhitbox, _userCollider);

            //Check whether the hitbox has a rigidbody
            Rigidbody2D hitboxRigidbody;
            if (!spawnedhitbox.TryGetComponent(out hitboxRigidbody)) yield break;

            //Move the hitbox
            hitboxRigidbody.velocity = Quaternion.Euler(0.0f, 0.0f, hitbox.m_projectileAngle) * _lookDir * hitbox.m_projectileSpeed;
            if (hitbox.m_rotateWithVelocity) hitboxRigidbody.transform.rotation = Quaternion.Euler(0.0f, 0.0f, Vector2.SignedAngle(Vector2.right, hitboxRigidbody.velocity));

            //Set Hitbox Damage
            Hitbox spawnedHitboxInfo = hitboxRigidbody.GetComponent<Hitbox>();
            if (spawnedHitboxInfo != null) continue;

            spawnedHitboxInfo = spawnedhitbox.gameObject.AddComponent<Hitbox>();
            spawnedHitboxInfo.m_damageRecipient = Hitbox.DamageRecipient.Enemy;
            spawnedHitboxInfo.m_damage = hitbox.m_hitboxDamage;
        }
    }
}
