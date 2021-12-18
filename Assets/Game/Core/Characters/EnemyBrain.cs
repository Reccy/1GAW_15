using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBrain : MonoBehaviour
{
    private Character m_char;

    private Cooldown m_leftPunch;
    private Cooldown m_rightPunch;
    private Cooldown m_punchDelay;

    private void Awake()
    {
        m_char = GetComponentInChildren<Character>();

        m_leftPunch = new Cooldown(0.3f);
        m_rightPunch = new Cooldown(0.3f);
        m_punchDelay = new Cooldown(0.15f);

        m_punchDelay.OnCooldownComplete += BeginPunchRight;
        m_leftPunch.OnCooldownComplete += PunchLeft;
        m_rightPunch.OnCooldownComplete += PunchRight;
    }

    private void Start()
    {
        m_leftPunch.Begin();
        m_punchDelay.Begin();
    }

    private void FixedUpdate()
    {
        m_leftPunch.Tick(Time.deltaTime);
        m_rightPunch.Tick(Time.deltaTime);
        m_punchDelay.Tick(Time.deltaTime);
    }

    private void BeginPunchRight()
    {
        m_rightPunch.Begin();
    }

    private void PunchLeft()
    {
        m_char.WindUpLeftStrike();
        m_char.ReleaseLeftStrike();
        m_leftPunch.Begin();
    }

    private void PunchRight()
    {
        m_char.WindUpRightStrike();
        m_char.ReleaseRightStrike();
        m_rightPunch.Begin();
    }
}
