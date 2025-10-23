using System;

public class Shield
{
    public float strength;
    public float duration; // negative duration means the shield last as long as it have strength left
    public Shield(float strength, float duration = -1.0f)
    {
        this.strength = strength;
        this.duration = duration;
    }
}