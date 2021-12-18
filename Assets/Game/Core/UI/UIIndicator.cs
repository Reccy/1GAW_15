using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public abstract class UIIndicator : MonoBehaviour
{
    private TMP_Text m_text;

    protected virtual void Awake()
    {
        m_text = GetComponent<TMP_Text>();

        if (m_text == null)
            Debug.LogError("TMP_Text could not be found!", this);
    }

    private void FixedUpdate()
    {
        m_text.text = GetTextFormat();
    }

    protected abstract string GetTextFormat();
}
