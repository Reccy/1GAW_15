using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class STAMIndicator : UIIndicator
{
    [SerializeField] private Character m_character;

    protected override string GetTextFormat() => $"STAM: {m_character.StaminaCurrent}/{m_character.StaminaMax}";
}
