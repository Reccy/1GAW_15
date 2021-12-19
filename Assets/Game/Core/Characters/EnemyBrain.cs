using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Reccy.DebugExtensions;

public class EnemyBrain : MonoBehaviour
{
    private Character m_char;
    private Character m_playerCharacter;

    [SerializeField] private Rigidbody2DFinder m_ffZone;

    private bool m_punchLeft = false;
    private bool m_canPunch = true;
    private Cooldown m_punchCooldown;

    private float m_timeSinceLastAttack = 0;

    private LevelSession m_levelSession;

    private void Awake()
    {
        m_char = GetComponentInChildren<Character>();
        m_char.OnHit += HandleOnHit;

        m_punchCooldown = new Cooldown(0.3f);
    }

    private void Start()
    {
        m_punchCooldown.Begin();
        m_punchCooldown.OnCooldownComplete = () => { m_canPunch = true; };

        m_playerCharacter = FindObjectOfType<PlayerBrain>().Character;

        m_levelSession = FindObjectOfType<LevelSession>();
        m_levelSession.NotifyEnemySpawned();
        m_char.OnDied += () => { m_levelSession.NotifyEnemyKilled(); };
    }

    private void FixedUpdate()
    {
        m_punchCooldown.Tick(Time.deltaTime);
        m_timeSinceLastAttack += Time.deltaTime;

        // Do nothing if the player has died
        if (m_playerCharacter == null || m_playerCharacter.IsDead)
            return;

        var move = m_playerCharacter.transform.position - m_char.transform.position;

        // Unblock when close and has stamina
        if (m_timeSinceLastAttack > 5.0f && m_char.StaminaCurrent >= (m_char.StaminaMax / 2))
        {
            if (m_char.IsBlocking)
                m_char.Unblock();
        }

        // Run away when out of stamina!
        if (m_char.HasNoStamina)
            move = -move;

        // Punch Player
        if (DistanceToPlayer < 4.0f && PlayerIsInLineOfSight())
        {
            if (m_canPunch)
            {
                if (m_punchLeft)
                {
                    m_char.WindUpLeftStrike();
                    m_char.ReleaseLeftStrike();
                    m_punchLeft = false;
                    m_canPunch = false;
                    m_punchCooldown.Begin();
                }
                else
                {
                    m_char.WindUpRightStrike();
                    m_char.ReleaseRightStrike();
                    m_punchLeft = true;
                    m_canPunch = false;
                    m_punchCooldown.Begin();
                }
            }
        }

        // Unblock when far away
        if (DistanceToPlayer > 15.0f)
        {
            if (m_char.IsBlocking)
                m_char.Unblock();
        }

        m_char.Move(move);
        m_char.LookAt(m_playerCharacter.transform.position, -90);
    }

    private float DistanceToPlayer => Vector3.Distance(m_playerCharacter.transform.position, m_char.transform.position);

    private void HandleOnHit()
    {
        m_timeSinceLastAttack = 0;
        m_char.Block();
    }

    private bool PlayerIsInLineOfSight()
    {
        var closestRb = m_ffZone.ClosestRigidbodyTo(m_char.transform.position);

        if (closestRb == null)
            return false;

        var c = closestRb.GetComponentInParent<PlayerBrain>();

        return c != null;
    }
}
