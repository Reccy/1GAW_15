using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Hurtbox : MonoBehaviour
{
    private const string HITBOX = "Hitbox";

    public delegate void OnHitEvent(Hitbox h);
    public OnHitEvent OnHit;

    private HitDetectionGroup m_group;
    public HitDetectionGroup Group => m_group;

    [SerializeField] private bool m_intergroupCollisions = false;
    public bool IntergroupCollisions => m_intergroupCollisions;

    private void Awake()
    {
        m_group = GetComponentInParent<HitDetectionGroup>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag(HITBOX))
            return;

        Hitbox hitbox = collision.GetComponent<Hitbox>();

        if (hitbox == null)
        {
            Debug.LogError($"GameObject with tag \"{HITBOX}\" does not contain a Hitbox component! ({collision.gameObject.name})");
            return;
        }

        // If an intergroup collision occurs, ensure both boxes have intergroup collision enabled
        if (IsIntergroupCollision(hitbox) && !IntergroupCollisionEnabled(hitbox))
            return;

        hitbox.OnHit(this);
        OnHit?.Invoke(hitbox);
    }

    private bool IsIntergroupCollision(Hitbox hitbox) => hitbox.Group == Group;

    private bool IntergroupCollisionEnabled(Hitbox hitbox) => hitbox.IntergroupCollisions && m_intergroupCollisions;
}
