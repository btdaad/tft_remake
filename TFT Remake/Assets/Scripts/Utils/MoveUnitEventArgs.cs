using UnityEngine;
using System;

public class MoveUnitEventArgs : EventArgs
{
    public enum Zone
    {
        Battlefield,
        Bench
    }

    public Transform unit;
    public Zone toZone;

    public MoveUnitEventArgs(Transform unit, Zone toZone)
    {
        this.unit = unit;
        this.toZone = toZone;
    }
}
