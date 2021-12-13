using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField] private Hurtbox m_hurtbox;

    private void Awake()
    {
        m_hurtbox.OnHit += HandleOnHit;
    }

    private void HandleOnHit(Hitbox hitbox)
    {
        Debug.Log($"You've been hit by {hitbox.gameObject.name}");
    }
}
