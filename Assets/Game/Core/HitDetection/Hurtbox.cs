using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Hurtbox : MonoBehaviour
{
    private const string HITBOX = "Hitbox";

    public delegate void OnHitEvent(Hitbox h);
    public OnHitEvent OnHit;

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

        hitbox.OnHit(this);

        OnHit?.Invoke(hitbox);
    }
}
