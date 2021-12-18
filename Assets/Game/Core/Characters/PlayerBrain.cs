using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;
using MoreMountains.Feedbacks;
using Reccy.ScriptExtensions;

public class PlayerBrain : MonoBehaviour
{
    private const int PLAYER_ID = 0;
    private Player m_rp;
    private Character m_char;

    private MouseCursorWorldPosition m_cursorPositionW;

    [Header("Feedbacks")]
    [SerializeField] private MMFeedbacks m_attackLandedFeedbacks;

    [Header("Lockon")]
    [SerializeField] private Rigidbody2DFinder m_lockonFinder;
    [SerializeField] private bool m_lockonEnabled = true;

    #region INPUT
    private const string BTN_LEFT_STRIKE = "LeftStrike";
    private const string BTN_RIGHT_STRIKE = "RightStrike";
    private const string BTN_DASH = "Dash";

    private bool m_inputLeftStrike = false;
    private bool m_inputLeftStrikeReleased = false;

    private bool m_inputRightStrike = false;
    private bool m_inputRightStrikeReleased = false;
    
    private bool m_inputDash = false;
    #endregion

    private void Awake()
    {
        m_rp = ReInput.players.GetPlayer(PLAYER_ID);
        m_char = GetComponentInChildren<Character>();
        m_cursorPositionW = FindObjectOfType<MouseCursorWorldPosition>();

        m_char.LeftFist.OnStrikeLanded += HandleOnAttackLanded;
        m_char.RightFist.OnStrikeLanded += HandleOnAttackLanded;
    }

    private void FixedUpdate()
    {
        Vector3 move = m_rp.GetAxis2D("MoveHorizontal", "MoveVertical");

        m_char.Move(move);

        var lookTarget = CalculateLookTarget();
        m_char.LookAt(lookTarget, -90);

        // Wind up
        if (m_inputLeftStrike)
        {
            m_char.WindUpLeftStrike();
            m_inputLeftStrike = false;
        }

        if (m_inputRightStrike)
        {
            m_char.WindUpRightStrike();
            m_inputRightStrike = false;
        }

        // Release
        if (m_inputLeftStrikeReleased)
        {
            m_char.ReleaseLeftStrike();
            m_inputLeftStrikeReleased = false;
        }

        if (m_inputRightStrikeReleased)
        {
            m_char.ReleaseRightStrike();
            m_inputRightStrikeReleased = false;
        }
    }

    private void Update()
    {
        if (!m_inputLeftStrike)
            m_inputLeftStrike = m_rp.GetButton(BTN_LEFT_STRIKE);

        if (!m_inputLeftStrikeReleased)
            m_inputLeftStrikeReleased = m_rp.GetButtonUp(BTN_LEFT_STRIKE);

        if (!m_inputRightStrike)
            m_inputRightStrike = m_rp.GetButton(BTN_RIGHT_STRIKE);

        if (!m_inputRightStrikeReleased)
            m_inputRightStrikeReleased = m_rp.GetButtonUp(BTN_RIGHT_STRIKE);

        if (!m_inputDash)
            m_inputDash = m_rp.GetButtonDown(BTN_DASH);
    }

    private void HandleOnAttackLanded()
    {
        m_attackLandedFeedbacks.PlayFeedbacks();
    }

    private Vector3 CalculateLookTarget()
    {
        if (!m_lockonEnabled || m_lockonFinder.AttachedRigidbodies.IsEmpty())
        {
            return m_cursorPositionW.transform.position;
        }

        var rb = m_lockonFinder.ClosestRigidbodyTo(transform.position);

        var character = rb.GetComponentInChildren<Character>();

        if (character == null || character.IsDead)
            return m_cursorPositionW.transform.position;

        return character.transform.position;
    }
}
