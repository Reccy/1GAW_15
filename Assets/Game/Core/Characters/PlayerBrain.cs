using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class PlayerBrain : MonoBehaviour
{
    private const int PLAYER_ID = 0;
    private Player m_rp;
    private Character m_char;

    private MouseCursorWorldPosition m_mousePos;

    #region INPUT
    private const string BTN_LEFT_STRIKE = "LeftStrike";
    private const string BTN_RIGHT_STRIKE = "RightStrike";

    private bool m_inputLeftStrike = false;
    private bool m_inputLeftStrikeReleased = false;

    private bool m_inputRightStrike = false;
    private bool m_inputRightStrikeReleased = false;
    #endregion

    private void Awake()
    {
        m_rp = ReInput.players.GetPlayer(PLAYER_ID);
        m_char = GetComponent<Character>();
        m_mousePos = FindObjectOfType<MouseCursorWorldPosition>();
    }

    private void FixedUpdate()
    {
        Vector3 move = m_rp.GetAxis2D("MoveHorizontal", "MoveVertical");

        m_char.Move(move);
        m_char.LookAt(m_mousePos.transform.position, -90);

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
    }
}
