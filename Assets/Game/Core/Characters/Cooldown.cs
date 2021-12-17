using UnityEngine;

public class Cooldown
{
    private float m_cooldownTime;
    private float m_timeRemaining;

    private bool m_cooldownComplete = true;

    public Cooldown(float cooldownTime)
    {
        m_cooldownTime = cooldownTime;
        m_timeRemaining = 0;
    }

    public void Tick(float dt)
    {
        if (m_cooldownComplete)
            return;

        m_timeRemaining = Mathf.Max(0, m_timeRemaining - dt);

        OnCooldownTick?.Invoke();

        if (m_timeRemaining == 0)
        {
            m_cooldownComplete = true;
            OnCooldownComplete?.Invoke();
        }
    }

    public void Begin()
    {
        m_timeRemaining = m_cooldownTime;
        m_cooldownComplete = false;

        OnCooldownBegin?.Invoke();
    }

    public bool Complete => m_timeRemaining <= 0;
    public bool InProgress => m_timeRemaining > 0;

    public delegate void OnCooldownUpdateEvent();

    public OnCooldownUpdateEvent OnCooldownBegin;
    public OnCooldownUpdateEvent OnCooldownTick;
    public OnCooldownUpdateEvent OnCooldownComplete;
}
