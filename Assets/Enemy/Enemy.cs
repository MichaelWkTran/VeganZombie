using System.Drawing;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class Enemy : CharacterController, IDamageable
{
    public float m_health;

    [Header("Offscreen Indicator")]
    [SerializeField] Canvas m_offscreenIndicatorCanvas;
    [SerializeField] RectTransform m_offscreenIndicator;

    [Header("Components")]
    [SerializeField] Collider2D m_collider;
    [SerializeField] SpriteRenderer m_spriteRenderer;
    [SerializeField] Animator m_animator;

    new protected void Update()
    {
        //Prevent updating if there is no player
        base.Update(); if (Player.m_current == null) return;

        //Make player follow player
        m_targetVelocity = Vector2.MoveTowards(transform.position, Player.m_current.transform.position, m_moveSpeed) - (Vector2)transform.position;
    }

    protected void LateUpdate()
    {
        //Update Animations
        if (m_rigidbody.velocity.x != 0.0f) m_spriteRenderer.flipX = m_rigidbody.velocity.x < 0;
        m_animator.SetFloat("Speed", m_rigidbody.velocity.magnitude);
        m_animator.SetFloat("Absolute X Dir", Mathf.Abs(m_rigidbody.velocity.x));
        m_animator.SetFloat("Y Dir", m_rigidbody.velocity.y);

        //Update Offscreen Indicator
        if (m_offscreenIndicatorCanvas.gameObject.activeSelf)
        {
            const float offset = 50.0f;
            Rect viewport = Camera.main.pixelRect;
            Vector2 canvasSpace = Camera.main.WorldToScreenPoint(transform.position);
            Vector2 offscreenIndicatorPos = canvasSpace;

            //Get new offscreen indicator position on canvas
            offscreenIndicatorPos.x = Mathf.Clamp(offscreenIndicatorPos.x, viewport.xMin + offset, viewport.xMax - offset);
            offscreenIndicatorPos.y = Mathf.Clamp(offscreenIndicatorPos.y, viewport.yMin + offset, viewport.yMax - offset);

            //Set offscreen indicator position & rotation
            m_offscreenIndicator.position = offscreenIndicatorPos;
            m_offscreenIndicator.rotation = Quaternion.Euler(0.0f, 0.0f, Vector2.SignedAngle(Vector2.right, canvasSpace - offscreenIndicatorPos) - 90.0f);
        }
    }

    protected void OnBecameVisible()
    {
        m_offscreenIndicatorCanvas.gameObject.SetActive(false);
    }

    protected void OnBecameInvisible()
    {
        m_offscreenIndicatorCanvas.gameObject.SetActive(true);
    }

    protected void OnCollisionStay2D(Collision2D _collision)
    {
        //
        if (Player.m_current != null && _collision.gameObject == Player.m_current.gameObject)
        {
            Player.m_current.Damage(1);
        }
    }

    public void Damage(float _damage, Vector2 _hitImpulse = new Vector2())
    {
        m_health -= _damage;

        //Sprite Flash
        LeanTween.value(gameObject, 0.0f, 1.0f, 0.5f).setEasePunch()
            .setOnUpdate((float _flashAlpha) => { m_spriteRenderer.material.SetFloat("_FlashAlpha", _flashAlpha); });

        //Kill enemy when all health is lost
        if (m_health <= 0) Kill();
    }

    public void Kill()
    {
        Destroy(gameObject);
    }
}
