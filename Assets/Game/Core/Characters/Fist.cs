using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fist : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private Transform m_targetTransform;
    [SerializeField] private Transform m_windupTransform;

    [Header("Timing")]
    [SerializeField, Range(0.01f, 1.0f)] private float m_windupTime = 0.3f;
    [SerializeField, Range(0.01f, 1.0f)] private float m_attackTime = 0.05f;
    [SerializeField, Range(0.01f, 1.0f)] private float m_missPenaltyTime = 0.05f;
    [SerializeField, Range(0.01f, 1.0f)] private float m_recoveryTime = 0.2f;

    Sequence m_currentSequence;

    private Vector3 m_targetPosition;
    private Vector3 m_restingPosition;
    private Vector3 m_windupPosition;
    private Hitbox m_hitbox;

    private enum FistState { IDLE, WINDUP, STRIKE, MISS, COOLDOWN };
    private FistState State = FistState.IDLE;

    public bool IsIdle => State == FistState.IDLE;
    public bool IsWindup => State == FistState.WINDUP;
    public bool IsStrike => State == FistState.STRIKE;
    public bool IsMiss => State == FistState.MISS;
    public bool IsCooldown => State == FistState.COOLDOWN;

    private void Awake()
    {
        m_targetPosition = m_targetTransform.position;
        m_restingPosition = transform.position;
        m_windupPosition = m_windupTransform.position;

        m_hitbox = GetComponentInChildren<Hitbox>();
        m_hitbox.OnHurt += OnFistAttackLanded;
    }

    private void Idle()
    {
        MoveToState(FistState.IDLE);

        SetHitboxActive(false);
    }

    // Returns true if attack triggered, false otherwise
    public void WindUp()
    {
        if (!IsIdle)
            return;

        MoveToState(FistState.WINDUP);

        m_currentSequence = DOTween.Sequence();
        m_currentSequence.Append(transform.DOLocalMove(m_windupPosition, m_windupTime, false));
        m_currentSequence.Play();
    }

    public void Strike()
    {
        if (!IsWindup)
            return;

        MoveToState(FistState.STRIKE);

        SetHitboxActive(true);

        m_currentSequence = DOTween.Sequence();
        m_currentSequence.Append(transform.DOLocalMove(m_targetPosition, m_attackTime, false));
        // Can get interrupted here by OnFistAttackLanded which will move immediately to Cooldown();
        m_currentSequence.AppendCallback(() => { Miss(); });
    }

    private void Miss()
    {
        MoveToState(FistState.MISS);

        SetHitboxActive(false);

        m_currentSequence = DOTween.Sequence();
        m_currentSequence.AppendInterval(m_missPenaltyTime);
        m_currentSequence.AppendCallback(() => { Cooldown(); });
        m_currentSequence.Play();
    }

    private void Cooldown()
    {
        MoveToState(FistState.COOLDOWN);

        SetHitboxActive(false);

        m_currentSequence = DOTween.Sequence();
        m_currentSequence.Append(transform.DOLocalMove(m_restingPosition, m_recoveryTime, false));
        m_currentSequence.AppendCallback(() => { Idle(); });
        m_currentSequence.Play();
    }

    private void SetHitboxActive(bool active)
    {
        m_hitbox.gameObject.SetActive(active);
    }

    private void MoveToState(FistState newState)
    {
        State = newState;

        if (m_currentSequence != null)
            m_currentSequence.Kill();
    }

    private void OnFistAttackLanded(Hurtbox hurtbox)
    {
        if (!IsStrike)
            return;

        Cooldown();
    }
}