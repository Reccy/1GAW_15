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
    private bool m_inputLeftStrike = false;
    private bool m_inputRightStrike = false;
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

        if (m_inputLeftStrike)
        {
            m_char.PunchLeft();
            m_inputLeftStrike = false;
        }

        if (m_inputRightStrike)
        {
            m_char.PunchRight();
            m_inputRightStrike = false;
        }
    }

    private void Update()
    {
        if (!m_inputLeftStrike)
            m_inputLeftStrike = m_rp.GetButtonDown("LeftStrike");

        if (!m_inputRightStrike)
            m_inputRightStrike = m_rp.GetButtonDown("RightStrike");
    }
}
