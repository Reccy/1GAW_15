using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Reccy.DebugExtensions;

public class EnemyBrain : MonoBehaviour
{
    private Character m_char;
    private Character m_playerCharacter;

    [SerializeField] private float m_inRangeAmount = 5.0f;
    [SerializeField] private Rigidbody2DFinder m_ffZone;

    private bool m_punchLeft = false;

    private void Awake()
    {
        m_char = GetComponentInChildren<Character>();
        m_char.OnHit += HandleOnHit;
    }

    private void Start()
    {
        m_playerCharacter = FindObjectOfType<PlayerBrain>().Character;
    }

    private void FixedUpdate()
    {
        // Do nothing if the player has died
        if (m_playerCharacter == null || m_playerCharacter.IsDead)
            return;

        var move = m_playerCharacter.transform.position - m_char.transform.position;
        
        m_char.Move(move);
        m_char.LookAt(m_playerCharacter.transform.position, -90);

        if (IsInRangeOfPlayer() && PlayerIsInLineOfSight())
        {
            if (m_punchLeft)
            {
                m_char.WindUpLeftStrike();
                m_char.ReleaseLeftStrike();
                m_punchLeft = false;
            }
            else
            {
                m_char.WindUpRightStrike();
                m_char.ReleaseRightStrike();
                m_punchLeft = true;
            }
        }
    }

    private bool IsInRangeOfPlayer()
    {
        return Vector3.Distance(m_playerCharacter.transform.position, m_char.transform.position) < m_inRangeAmount;
    }

    private void HandleOnHit()
    {
        m_char.Block();
    }

    private bool PlayerIsInLineOfSight()
    {
        var closestRb = m_ffZone.ClosestRigidbodyTo(m_char.transform.position);

        var c = closestRb.GetComponentInParent<PlayerBrain>();

        return c != null;
    }
}
