using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    [SerializeField] private float m_attackForce = 5.0f;
    public float AttackForce => m_attackForce;

    [SerializeField] private bool m_intergroupCollisions = false;
    public bool IntergroupCollisions => m_intergroupCollisions;

    public delegate void OnHurtEvent(Hurtbox h);
    public OnHurtEvent OnHurt;

    private HitDetectionGroup m_group;
    public HitDetectionGroup Group => m_group;

    private void Awake()
    {
        m_group = GetComponentInParent<HitDetectionGroup>();
    }

    public void OnHit(Hurtbox collidedWith)
    {
        OnHurt?.Invoke(collidedWith);
    }
}
