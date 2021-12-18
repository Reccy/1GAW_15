using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyLinkInverse : MonoBehaviour
{
    public GameObject Target;

    private void Awake()
    {
        DestroyLink link = Target.AddComponent<DestroyLink>();
        link.Target = gameObject;
    }
}
