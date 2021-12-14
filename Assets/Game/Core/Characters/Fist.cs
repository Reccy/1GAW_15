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

    private bool m_isAttacking = false;
    public bool IsAttacking => m_isAttacking;

    private void Awake()
    {
        m_targetPosition = m_targetTransform.position;
        m_restingPosition = transform.position;
        m_windupPosition = m_windupTransform.position;

        m_hitbox = GetComponentInChildren<Hitbox>();
        m_hitbox.OnHurt += HandleOnHurt;

        m_hitbox.gameObject.SetActive(false);
    }

    private void HandleOnHurt(Hurtbox hurtbox)
    {
        if (!IsAttacking)
            return;

        m_currentSequence.Kill();
        OnStrikeComplete();

        m_currentSequence = DOTween.Sequence();
        m_currentSequence.Append(transform.DOLocalMove(m_restingPosition, m_recoveryTime, false));
        m_currentSequence.AppendCallback(() => { OnRecoveryComplete(); });
        m_currentSequence.Play();
    }

    // Returns true if attack triggered, false otherwise
    public bool Attack()
    {
        if (IsAttacking)
            return false;

        m_isAttacking = true;
        m_hitbox.gameObject.SetActive(true);

        m_currentSequence = DOTween.Sequence();
        m_currentSequence.Append(transform.DOLocalMove(m_windupPosition, m_windupTime, false));
        m_currentSequence.Append(transform.DOLocalMove(m_targetPosition, m_attackTime, false));
        m_currentSequence.AppendCallback(() => { OnStrikeComplete(); });
        m_currentSequence.AppendInterval(m_missPenaltyTime);
        m_currentSequence.Append(transform.DOLocalMove(m_restingPosition, m_recoveryTime, false));
        m_currentSequence.AppendCallback(() => { OnRecoveryComplete(); });
        m_currentSequence.Play();

        return true;
    }

    private void OnStrikeComplete()
    {
        m_hitbox.gameObject.SetActive(false);
    }

    private void OnRecoveryComplete()
    {
        m_isAttacking = false;
    }
}
