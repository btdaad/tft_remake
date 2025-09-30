using UnityEngine;
using System;

public class MoveUnitEventArgs : EventArgs
{
    public Transform unit;
    public MoveUnitEventArgs(Transform unit)
    {
        this.unit = unit;
    }
}
