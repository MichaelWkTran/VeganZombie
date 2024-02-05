using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : CharacterController, IDamageable
{
    public static Player m_current { get; private set; }

    [SerializeField] float m_maxHealth; public float m_MaxHealth { get { return m_maxHealth; } }
    public float m_health;
    public delegate void OnPropertyChanged();
    public event OnPropertyChanged m_onPropertyChanged; //Called when a player property has changed

    [Header("Combat")]
    public Weapon m_weapon;
    float m_cooldown;
    [SerializeField] Transform m_gunTransform;
    [SerializeField] Transform m_gunMuzzle;
    Vector2 m_lookDir;

    [SerializeField] float m_invincibleTime;
    float m_invincibleCooldown;

    [Header("Inputs")]
    InputAction m_moveAction;
    InputAction m_attackAction;

    [Header("Components")]
    [SerializeField] Collider2D m_collider;
    [SerializeField] SpriteRenderer m_spriteRenderer;
    [SerializeField] Animator m_animator;
    [SerializeField] PlayerInput m_playerInput;

    void Start()
    {
        m_current = this;

        //Set variables
        m_health = m_maxHealth;

        //Set Inputs
        m_moveAction = m_playerInput.actions["Movement"];
        m_attackAction = m_playerInput.actions["Attack"];
    }

    new void Update()
    {
        base.Update();

        #region Update Variables
        //Set Look Dir Variable
        {
            if (Gamepad.current != null) m_lookDir = m_rigidbody.velocity;
            else if (Mouse.current != null) m_lookDir = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue()) - m_gunTransform.position;
            m_lookDir.Normalize();
        }

        //Rotate gun
        m_gunTransform.rotation = Quaternion.Euler(0.0f, 0.0f, Vector2.SignedAngle(Vector2.right, m_lookDir));

        //
        if (m_invincibleCooldown > 0.0f) m_invincibleCooldown -= Time.deltaTime;

        #endregion

        //Movement
        m_targetVelocity = m_moveAction.ReadValue<Vector2>() * m_moveSpeed;

        //Attack
        if (m_cooldown > 0) m_cooldown -= Time.deltaTime;
        if (m_attackAction.IsPressed()) Attack();
    }

    void LateUpdate()
    {
        if (m_lookDir.x != 0.0f) m_spriteRenderer.flipX = m_lookDir.x < 0;
        m_animator.SetFloat("Speed", m_rigidbody.velocity.magnitude);
        m_animator.SetFloat("Absolute X Dir", Mathf.Abs(m_lookDir.x));
        m_animator.SetFloat("Y Dir",          m_lookDir.y);
    }

    private void Attack()
    {
        if (m_weapon == null) return;
        if (m_cooldown > 0.0f) return;

        //Set current attack cooldown
        m_cooldown = m_weapon.m_cooldownDuration;

        //Shoot projectile
        IEnumerator SpawnHitbox(Weapon.Hitbox _hitbox)
        {
            //Soawn hitbox after a few seconds
            yield return new WaitForSeconds(_hitbox.m_hitboxSpawnTime);
            
            //Spawn hitbox
            Collider2D spawnedhitbox = Instantiate(_hitbox.m_prefab, m_gunMuzzle.position, Quaternion.identity);
            Destroy(spawnedhitbox.gameObject, _hitbox.m_hitboxDuration);
            Physics2D.IgnoreCollision(m_collider, spawnedhitbox);
            Physics2D.IgnoreCollision(spawnedhitbox, m_collider);

            //Check whether the hitbox have a rigidbody
            Rigidbody2D hitboxRigidbody;
            if (!spawnedhitbox.TryGetComponent(out hitboxRigidbody)) yield break;
            
            //Move the hitbox
            hitboxRigidbody.velocity = Quaternion.Euler(0.0f, 0.0f, _hitbox.m_projectileAngle) * m_lookDir * _hitbox.m_projectileSpeed;
            if (_hitbox.m_rotateWithVelocity)
                hitboxRigidbody.transform.rotation =
                    Quaternion.Euler(0.0f, 0.0f, Vector2.SignedAngle(Vector2.right, hitboxRigidbody.velocity));

            //Set hitbox damage
            Hitbox spawnedHitboxInfo = hitboxRigidbody.GetComponent<Hitbox>();
            if (spawnedHitboxInfo != null) yield break;

            spawnedHitboxInfo = spawnedhitbox.gameObject.AddComponent<Hitbox>();
            spawnedHitboxInfo.m_damageRecipient = Hitbox.DamageRecipient.Enemy;
            spawnedHitboxInfo.m_damage = _hitbox.m_hitboxDamage;
        }
        foreach (Weapon.Hitbox hitbox in m_weapon.m_hitbox) StartCoroutine(SpawnHitbox(hitbox));
        
        //Play Attack Animation
        m_animator.SetTrigger("Attack");
    }

    public void Damage(float _damage, Vector2 _hitImpulse = new Vector2())
    {
        if (m_invincibleCooldown > 0.0f) return;

        m_health -= _damage;
        m_invincibleCooldown = m_invincibleTime;

        //Sprite Flash
        LeanTween.value(gameObject, 0.0f, 1.0f, 0.5f).setEasePunch()
            .setOnUpdate((float _flashAlpha) => { m_spriteRenderer.material.SetFloat("_FlashAlpha", _flashAlpha); });

        //Push player when they are hit
        if (_hitImpulse != Vector2.zero) m_rigidbody.AddForce(_hitImpulse, ForceMode2D.Impulse);
        
        //Kill enemy when all health is lost
        if (m_health <= 0) Kill();

        //Notify bindings of health update such as for UI
        m_onPropertyChanged?.Invoke();
    }

    public void Kill()
    {

    }
}
