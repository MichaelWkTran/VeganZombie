using UnityEngine;

public interface IDamageable
{
    public void Damage(float _damage, Vector2 _hitImpulse = new Vector2());
}
