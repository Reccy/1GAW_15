using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBrain : MonoBehaviour
{
    private Character m_char;

    private void Awake()
    {
        m_char = GetComponentInChildren<Character>();
    }

    private void Start()
    {
        m_char.Block();
    }
}
