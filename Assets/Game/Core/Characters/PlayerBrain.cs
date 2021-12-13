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
    }
}
