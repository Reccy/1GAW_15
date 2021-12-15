using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rigidbody2DFinder : MonoBehaviour
{
    private List<Rigidbody2D> m_rigidbodies;
    public List<Rigidbody2D> AttachedRigidbodies => m_rigidbodies;

    [SerializeField] private List<Rigidbody2D> m_ignoreList;
    public IReadOnlyCollection<Rigidbody2D> IgnoreList => m_ignoreList.AsReadOnly();

    private void Awake()
    {
        m_rigidbodies = new List<Rigidbody2D>();

        if (m_ignoreList == null)
            m_ignoreList = new List<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var rb = collision.attachedRigidbody;

        if (!m_rigidbodies.Contains(rb) && !m_ignoreList.Contains(rb))
            m_rigidbodies.Add(rb);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        var rb = collision.attachedRigidbody;

        if (m_rigidbodies.Contains(rb) && !m_ignoreList.Contains(rb))
            m_rigidbodies.Remove(rb);
    }
}
