using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPIndicator : UIIndicator
{
    [SerializeField] private Character m_character;

    protected override string GetTextFormat() => $"HP: {m_character.HPCurrent}/{m_character.HPMax}";
}
