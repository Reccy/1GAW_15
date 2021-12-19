using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPIndicator : UIIndicator
{
    private Character m_character;

    private void Start()
    {
        m_character = FindObjectOfType<LevelSession>().PlayerCharacter;
    }

    protected override string GetTextFormat() => $"HP: {m_character.HPCurrent}/{m_character.HPMax}";
}
