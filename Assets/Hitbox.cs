using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    public enum DamageRecipient { All, Player, Enemy }
    public DamageRecipient m_damageRecipient = DamageRecipient.All;
    public float m_damage;

    void OnCollisionEnter2D(Collision2D _collision)
    {
        //Destroy on collision
        Destroy(gameObject);

        //Check whether the collided object implements the damageable interface
        IDamageable damageable = _collision.gameObject.GetComponent<IDamageable>();
        if (damageable == null) return;

        //Check who can the hitbox damage
        switch (m_damageRecipient)
        {
            case DamageRecipient.All: break;
            case DamageRecipient.Player:
                if (_collision.gameObject != Player.m_current.gameObject) return;
                break;
            case DamageRecipient.Enemy:
                if (_collision.gameObject.GetComponent<Enemy>() == null) return;
                break;
        }

        //Apply Damage
        damageable.Damage(m_damage, _collision.GetContact(0).normalImpulse * _collision.GetContact(0).normal);
    }
}
