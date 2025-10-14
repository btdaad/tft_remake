using UnityEngine;
using System;

public class OnDeathEventArgs : EventArgs
{
    public Transform unit;

    public OnDeathEventArgs(Transform unit)
    {
        this.unit = unit;
    }
}
