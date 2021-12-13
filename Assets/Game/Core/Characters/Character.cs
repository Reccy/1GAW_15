using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private Hurtbox m_hurtbox;

    [Header("Tuning")]
    [SerializeField] private float m_movementMult = 3.0f;
    
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
        m_rb.AddForce(movement * m_movementMult, ForceMode2D.Impulse);
    }

    public void LookAt(Vector3 worldPosition, float lookOffsetDegrees = 0)
    {
        var direction = (worldPosition - transform.position).normalized;
        m_rb.SetRotation(Quaternion.LookRotation(direction, Vector3.forward));
        m_rb.rotation += lookOffsetDegrees;
    }
}
