using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class MouseCursorWorldPosition : MonoBehaviour
{
    private const int PLAYER_ID = 0;
    private Player m_rp;
    private Mouse m_mouse;
    private Camera m_cam;

    [SerializeField] private float m_zPosition = 0.0f;

    private void Awake()
    {
        m_rp = ReInput.players.GetPlayer(PLAYER_ID);
        m_mouse = m_rp.controllers.Mouse;

        if (m_mouse == null)
            Debug.LogError("Could not get mouse!");

        m_cam = Camera.main;

        if (m_cam == null)
            Debug.LogError("Could not get cam!");
    }

    private void Update()
    {
        var v = m_cam.ScreenToWorldPoint(m_mouse.screenPosition);
        transform.position = new Vector3(v.x, v.y, m_zPosition);
    }
}
