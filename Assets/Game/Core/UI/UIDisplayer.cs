using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIDisplayer : MonoBehaviour
{
    public void Display()
    {
        var ls = FindObjectOfType<LevelSession>();
        ls.Canvas.SetActive(true);
        ls.BeginTimer();
    }
}
