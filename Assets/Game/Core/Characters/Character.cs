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
    [SerializeField] private MMFeedbacks m_outOfStaminaFeedbacks;
    [SerializeField] private MMFeedbacks m_onFinalDieFeedbacks;

    [Header("Character Stats")]
    [SerializeField] private int m_hpMax = 10;
    [SerializeField] private int m_staminaMax = 10;
    [SerializeField] private bool m_infiniteHp = false;
    [SerializeField] private bool m_infiniteStam = false;

    private int m_hpCurrent;
    private int m_staminaCurrent;

    public int HPMax => m_hpMax;
    public int HPCurrent => m_hpCurrent;
    public int StaminaMax => m_staminaMax;
    public int StaminaCurrent => m_staminaCurrent;

    private bool m_isAlive = true;
    public bool IsAlive => m_isAlive;
    public bool IsDead => !m_isAlive;

    public bool HasNoStamina => m_staminaCurrent <= 0;
    public bool HasStamina => m_staminaCurrent > 0;

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
    private Cooldown m_hitbackCooldown;

    [Header("Dash Settings")]
    [SerializeField] private float m_dashCooldownTime = 0.3f;
    [SerializeField] private float m_dashForce = 50.0f;
    private Cooldown m_dashCooldown;

    [Header("Death Settings")]
    [SerializeField] private float m_deathTime = 2.0f;
    private Cooldown m_deathCooldown;

    private void Awake()
    {
        m_hurtbox.OnHit += HandleOnHit;
        m_rb = GetComponent<Rigidbody2D>();

        m_hpCurrent = m_hpMax;
        m_staminaCurrent = m_staminaMax;

        m_leftFist.OnStrikeBegin += HandleOnStrikeBegin;
        m_rightFist.OnStrikeBegin += HandleOnStrikeBegin;

        m_hitbackCooldown = new Cooldown(m_hitbackTime);
        m_dashCooldown = new Cooldown(m_dashCooldownTime);
        m_deathCooldown = new Cooldown(m_deathTime);

        m_deathCooldown.OnCooldownComplete += HandleDeathComplete;
    }

    private void FixedUpdate()
    {
        m_hitbackCooldown.Tick(Time.deltaTime);
        m_dashCooldown.Tick(Time.deltaTime);
        m_deathCooldown.Tick(Time.deltaTime);
    }

    private void HandleOnStrikeBegin()
    {
        if (m_infiniteStam)
            return;

        m_staminaCurrent--;

        if (m_staminaCurrent < 0)
            m_staminaCurrent = 0;
    }

    private void HandleOnHit(Hitbox hitbox)
    {
        m_onHitFeedbacks.PlayFeedbacks(); // TODO: Remaining HP Feedbacks

        var force = (transform.position - hitbox.transform.position).normalized * hitbox.AttackForce;

        // Do Hitback
        m_rb.velocity = Vector3.zero;
        m_rb.AddForce(force, ForceMode2D.Impulse);
        m_hitbackCooldown.Begin();

        // TODO: Update HP system to actually have variable damage

        if (m_infiniteHp)
            return;

        if (m_hpCurrent > 0)
            m_hpCurrent--;

        if (m_hpCurrent <= 0 && IsAlive)
            Die();
    }

    private void Die()
    {
        m_onDieFeedbacks.PlayFeedbacks();
        m_isAlive = false;
        m_deathCooldown.Begin();
    }

    private void HandleDeathComplete()
    {
        m_onFinalDieFeedbacks.PlayFeedbacks();
    }

    public void Move(Vector3 movement)
    {
        if (m_hitbackCooldown.InProgress || IsDead)
            return;

        movement = Vector3.ClampMagnitude(movement, 1);

        float movementPercent = 1.0f;

        if (IsAttacking || HasNoStamina)
            movementPercent = m_attackMovementPercent;

        m_rb.AddForce(movement * m_movementMult * movementPercent, ForceMode2D.Impulse);
    }

    public void LookAt(Vector3 worldPosition, float lookOffsetDegrees = 0)
    {
        if (m_hitbackCooldown.InProgress || IsDead)
            return;

        var direction = (worldPosition - transform.position).normalized;
        m_rb.SetRotation(Quaternion.LookRotation(direction, Vector3.forward));
        m_rb.rotation += lookOffsetDegrees;
    }

    public void Dash(Vector3 dashDirection)
    {
        dashDirection = dashDirection.normalized;

        if (m_dashCooldown.InProgress) // TODO: Feedbacks
            return;

        m_rb.AddForce(dashDirection * m_dashForce, ForceMode2D.Impulse);

        m_dashCooldown.Begin();
    }

    public void WindUpLeftStrike()
    {
        if (IsDead)
            return;

        if (CheckStaminaAndRunFeedbacks())
            return;

        m_leftFist.WindUp();
    }

    public void WindUpRightStrike()
    {
        if (IsDead)
            return;

        if (CheckStaminaAndRunFeedbacks())
            return;

        m_rightFist.WindUp();
    }

    public void ReleaseLeftStrike()
    {
        if (IsDead)
            return;

        if (CheckStaminaAndRunFeedbacks())
            return;

        m_leftFist.Strike();
    }

    public void ReleaseRightStrike()
    {
        if (IsDead)
            return;

        if (CheckStaminaAndRunFeedbacks())
            return;

        m_rightFist.Strike();
    }

    private bool CheckStaminaAndRunFeedbacks()
    {
        if (m_staminaCurrent >= 1)
            return false;

        if (!m_outOfStaminaFeedbacks.IsPlaying)
            m_outOfStaminaFeedbacks.PlayFeedbacks();

        return true;
    }
}
