using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSession : MonoBehaviour
{
    private float m_elapsedTimeSeconds = 0;
    public float ElapsedTimeSeconds => m_elapsedTimeSeconds;

    private void Update()
    {
        m_elapsedTimeSeconds += Time.deltaTime;
    }
}
