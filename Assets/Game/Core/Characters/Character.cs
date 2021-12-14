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

    // TODO: Focus punches on character in front
    private Character m_closestCharacter;
    public Character ClosestCharacter => m_closestCharacter;

    private bool IsAttacking => m_leftFist.IsAttacking || m_rightFist.IsAttacking;

    private Rigidbody2D m_rb;

    private void Awake()
    {
        m_hurtbox.OnHit += HandleOnHit;
        m_rb = GetComponent<Rigidbody2D>();
    }

    private void HandleOnHit(Hitbox hitbox)
    {
        Debug.Log($"You've been hit by {hitbox.gameObject.name}");
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

    public void PunchLeft()
    {
        m_leftFist.Attack();
    }

    public void PunchRight()
    {
        m_rightFist.Attack();
    }
}
