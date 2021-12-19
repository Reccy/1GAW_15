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
    public Character Character {
        get
        {
            if (m_char == null)
                m_char = GetComponentInChildren<Character>();

            return m_char;
        }
    }

    private MouseCursorWorldPosition m_cursorPositionW;

    [Header("Feedbacks")]
    [SerializeField] private MMFeedbacks m_attackLandedFeedbacks;

    [Header("Lockon")]
    [SerializeField] private Rigidbody2DFinder m_lockonFinder;
    [SerializeField] private bool m_lockonEnabled = true;

    #region INPUT
    private const string BTN_LEFT_STRIKE = "LeftStrike";
    private const string BTN_RIGHT_STRIKE = "RightStrike";
    private const string BTN_BLOCK = "Block";
    private const string BTN_WALK = "Walk";

    private bool m_inputLeftStrike = false;
    private bool m_inputLeftStrikeReleased = false;

    private bool m_inputRightStrike = false;
    private bool m_inputRightStrikeReleased = false;
    
    private bool m_inputBlock = false;
    private bool m_inputBlockReleased = false;

    private bool m_inputWalk = false;
    private bool m_inputWalkReleased = false;
    #endregion

    private void Awake()
    {
        m_rp = ReInput.players.GetPlayer(PLAYER_ID);
        m_cursorPositionW = FindObjectOfType<MouseCursorWorldPosition>();

        Character.LeftFist.OnStrikeLanded += HandleOnAttackLanded;
        Character.RightFist.OnStrikeLanded += HandleOnAttackLanded;
    }

    private void FixedUpdate()
    {
        Vector3 move = m_rp.GetAxis2D("MoveHorizontal", "MoveVertical");

        Character.Move(move);

        var lookTarget = CalculateLookTarget();
        Character.LookAt(lookTarget, -90);

        if (m_inputBlock)
        {
            Character.Block();
            m_inputBlock = false;
        }

        if (m_inputBlockReleased)
        {
            Character.Unblock();
            m_inputBlockReleased = false;
        }

        if (m_inputWalk)
        {
            Character.ToggleWalk(true);
            m_inputWalk = false;
        }

        if (m_inputWalkReleased)
        {
            Character.ToggleWalk(false);
            m_inputWalkReleased = false;
        }

        // Wind up
        if (m_inputLeftStrike)
        {
            Character.WindUpLeftStrike();
            m_inputLeftStrike = false;
        }

        if (m_inputRightStrike)
        {
            Character.WindUpRightStrike();
            m_inputRightStrike = false;
        }

        // Release
        if (m_inputLeftStrikeReleased)
        {
            Character.ReleaseLeftStrike();
            m_inputLeftStrikeReleased = false;
        }

        if (m_inputRightStrikeReleased)
        {
            Character.ReleaseRightStrike();
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

        if (!m_inputBlock)
            m_inputBlock = m_rp.GetButtonDown(BTN_BLOCK);

        if (!m_inputBlockReleased)
            m_inputBlockReleased = m_rp.GetButtonUp(BTN_BLOCK);

        if (!m_inputWalk)
            m_inputWalk = m_rp.GetButtonDown(BTN_WALK);

        if (!m_inputWalkReleased)
            m_inputWalkReleased = m_rp.GetButtonUp(BTN_WALK);
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
