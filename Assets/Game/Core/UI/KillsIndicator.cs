using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillsIndicator : UIIndicator
{
    private LevelSession m_levelSession;

    protected override void Awake()
    {
        base.Awake();
        m_levelSession = FindObjectOfType<LevelSession>();
    }

    protected override string GetTextFormat() => $"{m_levelSession.Kills} KOs";
}
