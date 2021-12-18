using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyLink : MonoBehaviour
{
    public GameObject Target;

    private void OnDestroy()
    {
        if (Target != null)
            Destroy(Target);
    }
}
