using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowOnBeat : DoOnBeat
{
    protected override void OnBeat(float beatOffset)
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }
}
