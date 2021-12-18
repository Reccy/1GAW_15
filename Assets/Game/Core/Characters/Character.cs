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
    [SerializeField] private MMFeedbacks m_onStartBlockFeedbacks;
    [SerializeField] private MMFeedbacks m_onFinishBlockFeedbacks;
    [SerializeField] private MMFeedbacks m_onShieldHitFeedbacks;
    [SerializeField] private MMFeedbacks m_onBreakBlockFeedbacks;

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
    [SerializeField, Range(0.0f, 1.0f)] private float m_movementPenaltyPercent = 0.3f;

    [SerializeField] private Fist m_leftFist;
    [SerializeField] private Fist m_rightFist;

    public Fist LeftFist => m_leftFist;
    public Fist RightFist => m_rightFist;

    private bool IsAttacking => !m_leftFist.IsIdle || !m_rightFist.IsIdle;
    private bool CanAttack => IsAlive && !m_isBlocking;

    private Rigidbody2D m_rb;

    [Header("Combat Settings")]
    [SerializeField] private float m_hitbackTime = 0.2f;
    [SerializeField] private int m_blockHitPenalty = 1;
    private Cooldown m_hitbackCooldown;
    private bool m_isBlocking = false;

    public bool IsBlocking => m_isBlocking;

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
        m_deathCooldown = new Cooldown(m_deathTime);

        m_deathCooldown.OnCooldownComplete += HandleDeathComplete;
    }

    private void FixedUpdate()
    {
        m_hitbackCooldown.Tick(Time.deltaTime);
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
        var force = (transform.position - hitbox.transform.position).normalized * hitbox.AttackForce;

        // Do Hitback
        m_rb.velocity = Vector3.zero;
        m_rb.AddForce(force, ForceMode2D.Impulse);
        m_hitbackCooldown.Begin();

        if (IsBlocking)
        {
            m_onShieldHitFeedbacks.PlayFeedbacks();
            TakeStaminaDamage();
        }
        else
        {
            m_onHitFeedbacks.PlayFeedbacks();
            TakeHealthDamage();
        }
    }

    private void TakeHealthDamage()
    {
        if (m_infiniteHp)
            return;

        if (m_hpCurrent > 0)
            m_hpCurrent--;

        if (m_hpCurrent <= 0 && IsAlive)
            Die();
    }

    private void TakeStaminaDamage()
    {
        if (m_infiniteStam)
            return;

        if (m_staminaCurrent > 0)
            m_staminaCurrent--;

        if (m_staminaCurrent <= 0 && IsAlive)
        {
            BreakShield();
        }
    }

    private void Die()
    {
        m_rb.velocity *= 3; // Multiply attack feedback to emphasize character death
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

        if (IsAttacking || HasNoStamina || IsBlocking)
            movementPercent = m_movementPenaltyPercent;

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

    public void WindUpLeftStrike()
    {
        if (!CanAttack)
            return;

        if (CheckStaminaAndRunFeedbacks())
            return;

        m_leftFist.WindUp();
    }

    public void WindUpRightStrike()
    {
        if (!CanAttack)
            return;

        if (CheckStaminaAndRunFeedbacks())
            return;

        m_rightFist.WindUp();
    }

    public void ReleaseLeftStrike()
    {
        if (!CanAttack)
            return;

        if (CheckStaminaAndRunFeedbacks())
            return;

        m_leftFist.Strike();
    }

    public void ReleaseRightStrike()
    {
        if (!CanAttack)
            return;

        if (CheckStaminaAndRunFeedbacks())
            return;

        m_rightFist.Strike();
    }

    public void Block()
    {
        if (IsDead)
            return;

        if (CheckStaminaAndRunFeedbacks())
            return;

        if (!m_isBlocking)
            BeginBlocking();

        m_isBlocking = true;
    }

    public void Unblock()
    {
        if (IsDead)
            return;

        if (m_isBlocking)
            BeginUnblocking();

        m_isBlocking = false;
    }

    private void BreakShield()
    {
        m_isBlocking = false;

        m_onBreakBlockFeedbacks.PlayFeedbacks();
    }

    private bool CheckStaminaAndRunFeedbacks()
    {
        if (m_staminaCurrent >= 1)
            return false;

        if (!m_outOfStaminaFeedbacks.IsPlaying)
            m_outOfStaminaFeedbacks.PlayFeedbacks();

        return true;
    }

    private void BeginBlocking()
    {
        m_onStartBlockFeedbacks.PlayFeedbacks();
    }

    private void BeginUnblocking()
    {
        m_onFinishBlockFeedbacks.PlayFeedbacks();
    }
}
