using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformFollow : MonoBehaviour
{
    [SerializeField] private GameObject m_follow;

    [SerializeField] private bool m_followPosition = true;
    [SerializeField] private bool m_followRotation = true;
    [SerializeField] private bool m_followScale = true;

    private void LateUpdate()
    {
        if (m_followPosition)
            transform.position = m_follow.transform.position;

        if (m_followRotation)
            transform.rotation = m_follow.transform.rotation;

        if (m_followScale)
            transform.localScale = m_follow.transform.localScale;
    }
}
