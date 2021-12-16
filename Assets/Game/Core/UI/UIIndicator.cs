using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public abstract class UIIndicator : MonoBehaviour
{
    private TMP_Text m_text;

    private void Awake()
    {
        m_text = GetComponent<TMP_Text>();
    }

    private void FixedUpdate()
    {
        m_text.text = GetTextFormat();
    }

    protected abstract string GetTextFormat();
}
