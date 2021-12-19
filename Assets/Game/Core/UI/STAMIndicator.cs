using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class STAMIndicator : UIIndicator
{
    private Character m_character;

    private void Start()
    {
        m_character = FindObjectOfType<LevelSession>().PlayerCharacter;
    }

    protected override string GetTextFormat() => $"STAM: {m_character.StaminaCurrent}/{m_character.StaminaMax}";
}
