using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HitDetectionGroup))]
public class Character : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private Hurtbox m_hurtbox;

    [Header("Tuning")]
    [SerializeField] private float m_movementMult = 3.0f;
    [SerializeField, Range(0.0f, 1.0f)] private float m_attackMovementPercent = 0.6f;

    [SerializeField] private Fist m_leftFist;
    [SerializeField] private Fist m_rightFist;

    public Fist LeftFist => m_leftFist;
    public Fist RightFist => m_rightFist;

    private bool IsAttacking => !m_leftFist.IsIdle || !m_rightFist.IsIdle;

    private Rigidbody2D m_rb;

    private void Awake()
    {
        m_hurtbox.OnHit += HandleOnHit;
        m_rb = GetComponent<Rigidbody2D>();
    }

    private void HandleOnHit(Hitbox hitbox)
    {
        // TODO
    }

    public void Move(Vector3 movement)
    {
        movement = Vector3.ClampMagnitude(movement, 1);

        float movementPercent = 1.0f;

        if (IsAttacking)
            movementPercent = m_attackMovementPercent;

        m_rb.AddForce(movement * m_movementMult * movementPercent, ForceMode2D.Impulse);
    }

    public void LookAt(Vector3 worldPosition, float lookOffsetDegrees = 0)
    {
        var direction = (worldPosition - transform.position).normalized;
        m_rb.SetRotation(Quaternion.LookRotation(direction, Vector3.forward));
        m_rb.rotation += lookOffsetDegrees;
    }

    public void WindUpLeftStrike()
    {
        m_leftFist.WindUp();
    }

    public void WindUpRightStrike()
    {
        m_rightFist.WindUp();
    }

    public void ReleaseLeftStrike()
    {
        m_leftFist.Strike();
    }

    public void ReleaseRightStrike()
    {
        m_rightFist.Strike();
    }
}
