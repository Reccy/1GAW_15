using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;

[RequireComponent(typeof(HitDetectionGroup))]
public class Character : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private Hurtbox m_hurtbox;
    [SerializeField] private MMFeedbacks m_onHitFeedbacks;
    [SerializeField] private MMFeedbacks m_onDieFeedbacks;

    [Header("Character Stats")]
    [SerializeField] private int m_hpMax = 10;
    [SerializeField] private int m_staminaMax = 10;

    private int m_hpCurrent;
    private int m_staminaCurrent;

    public int HPMax => m_hpMax;
    public int HPCurrent => m_hpCurrent;
    public int StaminaMax => m_staminaMax;
    public int StaminaCurrent => m_staminaCurrent;

    private bool m_isAlive = true;
    public bool IsAlive => m_isAlive;
    public bool IsDead => !m_isAlive;

    [Header("Movement Tuning")]
    [SerializeField] private float m_movementMult = 3.0f;
    [SerializeField, Range(0.0f, 1.0f)] private float m_attackMovementPercent = 0.6f;

    [SerializeField] private Fist m_leftFist;
    [SerializeField] private Fist m_rightFist;

    public Fist LeftFist => m_leftFist;
    public Fist RightFist => m_rightFist;

    private bool IsAttacking => !m_leftFist.IsIdle || !m_rightFist.IsIdle;

    private Rigidbody2D m_rb;

    [Header("Combat Settings")]
    [SerializeField] private float m_hitbackTime = 0.2f;
    private float m_hitbackTimeRemaining = 0.0f;
    private bool IsBeingHitback => m_hitbackTimeRemaining > 0;

    private void Awake()
    {
        m_hurtbox.OnHit += HandleOnHit;
        m_rb = GetComponent<Rigidbody2D>();

        m_hpCurrent = m_hpMax;
        m_staminaCurrent = m_staminaMax;
    }

    private void FixedUpdate()
    {
        if (IsBeingHitback)
            m_hitbackTimeRemaining -= Time.deltaTime;
    }

    private void HandleOnHit(Hitbox hitbox)
    {
        m_onHitFeedbacks.PlayFeedbacks();

        var force = (transform.position - hitbox.transform.position).normalized * hitbox.AttackForce;

        // Do Hitback
        m_rb.velocity = Vector3.zero;
        m_rb.AddForce(force, ForceMode2D.Impulse);
        m_hitbackTimeRemaining = m_hitbackTime;

        // TODO: Update HP system to actually have variable damage
        m_hpCurrent--;

        if (m_hpCurrent <= 0 && IsAlive)
            Die();
    }

    private void Die()
    {
        m_onDieFeedbacks.PlayFeedbacks();
        m_isAlive = false;
    }

    public void Move(Vector3 movement)
    {
        if (IsBeingHitback || IsDead)
            return;

        movement = Vector3.ClampMagnitude(movement, 1);

        float movementPercent = 1.0f;

        if (IsAttacking)
            movementPercent = m_attackMovementPercent;

        m_rb.AddForce(movement * m_movementMult * movementPercent, ForceMode2D.Impulse);
    }

    public void LookAt(Vector3 worldPosition, float lookOffsetDegrees = 0)
    {
        if (IsBeingHitback || IsDead)
            return;

        var direction = (worldPosition - transform.position).normalized;
        m_rb.SetRotation(Quaternion.LookRotation(direction, Vector3.forward));
        m_rb.rotation += lookOffsetDegrees;
    }

    public void WindUpLeftStrike()
    {
        if (IsDead)
            return;

        m_leftFist.WindUp();
    }

    public void WindUpRightStrike()
    {
        if (IsDead)
            return;

        m_rightFist.WindUp();
    }

    public void ReleaseLeftStrike()
    {
        if (IsDead)
            return;

        m_leftFist.Strike();
    }

    public void ReleaseRightStrike()
    {
        if (IsDead)
            return;

        m_rightFist.Strike();
    }
}
