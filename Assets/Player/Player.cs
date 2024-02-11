using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : CharacterController, IDamageable
{
    public static Player m_current { get; private set; }

    [Header("Combat")]
    public Weapon m_weapon;
    float m_cooldown;
    [SerializeField] Transform m_gunTransform;
    [SerializeField] Transform m_gunMuzzle;
    Vector2 m_lookDir;

    [SerializeField] float m_invincibleTime;
    [SerializeField] float m_invincibleCooldown;

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

        //
        m_gunTransform.rotation = Quaternion.Euler(0.0f, 0.0f, Vector2.SignedAngle(Vector2.right, m_lookDir));

        //
        if (m_invincibleCooldown > 0.0f) m_invincibleCooldown -= m_invincibleCooldown;

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
        InventorySystem.Slot selectedSlot = GameManager.m_current.m_SelectedHotbarSlot;
        Weapon weapon = (selectedSlot.m_item != null) ? (Weapon)selectedSlot.m_item : null;

        if (!selectedSlot.IsValid()) return;
        if (m_cooldown > 0.0f) return;

        //Set current attack cooldown
        m_cooldown = weapon.m_cooldownDuration;

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

            //Set Hitbox Damage
            Hitbox spawnedHitboxInfo = hitboxRigidbody.GetComponent<Hitbox>();
            if (spawnedHitboxInfo != null) yield break;

            spawnedHitboxInfo = spawnedhitbox.gameObject.AddComponent<Hitbox>();
            spawnedHitboxInfo.m_damageRecipient = Hitbox.DamageRecipient.Enemy;
            spawnedHitboxInfo.m_damage = _hitbox.m_hitboxDamage;
        }
        foreach (Weapon.Hitbox hitbox in weapon.m_hitbox) StartCoroutine(SpawnHitbox(hitbox));
        
        //Play Attack Animation
        m_animator.SetTrigger("Attack");
    }

    public void Damage(float _damage, Vector2 _hitImpulse = new Vector2())
    {
        if (m_invincibleCooldown > 0.0f) return;
        m_invincibleCooldown = m_invincibleTime;
        
        //Flash Sprite
        LeanTween.value(gameObject, 0.0f, 0.1f, 0.5f).setEasePunch().setOnUpdate((float _flashAlpha) => { m_spriteRenderer.material.SetFloat("_FlashAlpha", _flashAlpha); });

        //Push the player when they are hit
        if (_hitImpulse != Vector2.zero) m_rigidbody.AddForce(_hitImpulse, ForceMode2D.Impulse);

        //Update Game Manager
        if (!GameManager.m_current) return;

        GameManager.m_current.m_Health -= _damage;
        //Kill the player when all health is lost
        if (GameManager.m_current.m_Health <= 0) Kill();
    }

    public void Kill()
    {

    }
}
