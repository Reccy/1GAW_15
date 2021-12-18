using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaytimeIndicator : UIIndicator
{
    private LevelSession m_level;

    protected override void Awake()
    {
        m_level = FindObjectOfType<LevelSession>();

        if (m_level == null)
            Debug.LogError("LevelSession not found!", this);

        base.Awake();
    }

    protected override string GetTextFormat() {
        string original = m_level.ElapsedTimeSeconds.ToString();

        var dotIndex = original.IndexOf(".");

        if (dotIndex == -1)
        {
            return $"{original}.00 seconds";
        }

        int substringEnd = Mathf.Min(dotIndex + 3, original.Length - 1);

        return $"{original.Substring(0, substringEnd)} seconds";
    }
}
