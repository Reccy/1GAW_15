using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;

public class HurtboxFeedbacks : MonoBehaviour
{
    private Hurtbox m_hurtbox;
    private MMFeedbacks m_feedbacks;

    private void Awake()
    {
        m_hurtbox = GetComponentInChildren<Hurtbox>();
        m_hurtbox.OnHit += HandleOnHit;

        m_feedbacks = GetComponentInChildren<MMFeedbacks>();
    }

    private void HandleOnHit(Hitbox hitbox)
    {
        m_feedbacks.PlayFeedbacks();
    }
}
