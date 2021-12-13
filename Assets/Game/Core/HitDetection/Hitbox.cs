using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    public void OnHit(Hurtbox collidedWith)
    {
        Debug.Log("Hitbox is hitting!");
    }
}
