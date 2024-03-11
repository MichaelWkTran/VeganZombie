using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : CharacterController, IDamageable
{
    public static Player m_current { get; private set; }

    [Header("Combat")]
    [SerializeField] Weapon m_punch;
    float m_cooldown;
    [SerializeField] Transform m_gunTransform;
    [SerializeField] Transform m_gunMuzzle;
    Vector2 m_lookDir;

    [SerializeField] float m_invincibleTime;
    [SerializeField] float m_invincibleCooldown;

    [Header("Inputs")]
    [HideInInspector] public InputAction m_moveAction;
    [HideInInspector] public InputAction m_actionAction;
    [HideInInspector] public InputAction m_interactAction;

    [Header("Components")]
    [SerializeField] Collider2D m_collider;
    [SerializeField] SpriteRenderer m_spriteRenderer;
    [SerializeField] Animator m_animator; AnimatorOverrideController m_animatorOverrideController;
    [SerializeField] PlayerInput m_playerInput;

    void Start()
    {
        m_current = this;

        //Set Components
        m_animatorOverrideController = m_animator.runtimeAnimatorController as AnimatorOverrideController;

        //Set Inputs
        m_moveAction = m_playerInput.actions["Movement"];
        m_actionAction = m_playerInput.actions["Action"];
        m_interactAction = m_playerInput.actions["Interact"];
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

        //Update the gun rotation to face the direction of the cursor/right thumbstick
        m_gunTransform.rotation = Quaternion.Euler(0.0f, 0.0f, Vector2.SignedAngle(Vector2.right, m_lookDir));

        //Update cooldown
        if (m_invincibleCooldown > 0.0f) m_invincibleCooldown -= Time.deltaTime;
        if (m_cooldown > 0) m_cooldown -= Time.deltaTime;

        #endregion

        #region Inputs
        Action();

        #endregion

        //Movement
        m_targetVelocity = m_moveAction.ReadValue<Vector2>() * m_moveSpeed;
    }

    void LateUpdate()
    {
        if (m_lookDir.x != 0.0f) m_spriteRenderer.flipX = m_lookDir.x < 0;
        m_animator.SetFloat("Speed", m_rigidbody.velocity.magnitude);
        m_animator.SetFloat("Absolute X Dir", Mathf.Abs(m_lookDir.x));
        m_animator.SetFloat("Y Dir",          m_lookDir.y);
    }

    public void Action()
    {
        //Check whether the action button is pressed
        if (!m_actionAction.IsPressed()) return;

        //Use item from seleected slot

        //Wait for cooldown to finish before the next item can be used
        if (m_cooldown > 0.0f) return;

        //Check whether the selected slot contains an item
        InventorySystem.Slot selectedSlot = GameManager.m_current.m_SelectedHotbarSlot;
        UsableItem usableItem = selectedSlot.IsValid() ? selectedSlot.m_item as UsableItem : null;

        //If no item is held, punch instead
        if (usableItem == null) usableItem = m_punch;

        //Prevent triggering if the item can only be used when pressed on this frame
        if (!usableItem.m_useContinuously && !m_actionAction.WasPressedThisFrame()) return;

        //Replace action animations
        m_animatorOverrideController["Attack Angle Back"]  = usableItem.m_useItemAngleBack;
        m_animatorOverrideController["Attack Angle Front"] = usableItem.m_useItemAngleFront;
        m_animatorOverrideController["Attack Back"       ] = usableItem.m_useItemBack;
        m_animatorOverrideController["Attack Front"      ] = usableItem.m_useItemFront;
        m_animatorOverrideController["Attack Side"]        = usableItem.m_useItemSide;
        
        //Update action cooldown
        m_cooldown = usableItem.m_cooldownDuration;
        
        //Use Item
        StartCoroutine(usableItem.UseItem(m_collider, m_gunMuzzle.position, m_lookDir));

        //Play animation of the player using the item
        m_animator.SetTrigger("Action");
    }

    public void Damage(float _damage, Vector2 _hitImpulse = new Vector2(), bool _playDamageAnimation = true)
    {
        if (m_invincibleCooldown > 0.0f) return;
        m_invincibleCooldown = m_invincibleTime;

        //Play Damage Animation
        if (_playDamageAnimation) m_animator.SetTrigger("Damage");

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
