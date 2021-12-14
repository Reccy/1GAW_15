using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feel;
using MoreMountains.Feedbacks;

public class Dummy : MonoBehaviour
{
    private Hurtbox m_hurtbox;
    private MMFeedbacks m_feedbacks;
    private Rigidbody2D m_rb;

    private void Awake()
    {
        m_hurtbox = GetComponentInChildren<Hurtbox>();
        m_hurtbox.OnHit += HandleOnHit;
        m_feedbacks = GetComponent<MMFeedbacks>();
        m_rb = GetComponent<Rigidbody2D>();
    }

    private void HandleOnHit(Hitbox hitbox)
    {
        m_feedbacks.PlayFeedbacks();

        var force = (transform.position - hitbox.transform.position).normalized * hitbox.AttackForce;

        m_rb.AddForce(force, ForceMode2D.Impulse);
    }
}
