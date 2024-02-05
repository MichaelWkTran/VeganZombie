using UnityEngine;

public class CharacterController : MonoBehaviour
{
    [HideInInspector] public Vector2 m_targetVelocity; //The velocity the character is aiming to reach
    [SerializeField] protected float m_moveSpeed; //The max speed the player will be moving at
    [SerializeField] protected float m_moveAcceleration; //The acceleration applied when the character is moving
    [SerializeField] protected float m_moveDeceleration; //The deceleration applied when the character is moving against their current velocity
    [SerializeField] protected float m_idleDeceleration; //The deceleration applied when the character stops moving
    [SerializeField] protected Rigidbody2D m_rigidbody;

    public void Update()
    {
        //Do not process movement if the rigidbody is not moving and no input 
        if (m_rigidbody.velocity == Vector2.zero && m_targetVelocity == Vector2.zero) return;
        float deltaTime = Time.deltaTime;

        //Set the current acceleration based on the target velocity the character
        float currentAcceleration = m_idleDeceleration;
        if (m_targetVelocity != Vector2.zero) currentAcceleration = (Vector2.Dot(m_rigidbody.velocity, m_targetVelocity) >= 0) ?
            m_moveAcceleration : m_moveDeceleration;

        //Applies acceleration onto the player
        m_rigidbody.velocity = Vector2.MoveTowards(m_rigidbody.velocity, m_targetVelocity, currentAcceleration * deltaTime);
        
        //Reset Target Velocity
        m_targetVelocity = Vector2.zero;
    }
}