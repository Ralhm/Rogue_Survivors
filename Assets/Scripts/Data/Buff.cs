using Godot;
using System;


public enum BuffType
{
    None,
    PhysAttack,
    PhysDefense,
    MagAttack,
    MagDefense
}

//We don't need to make separate buff classes or instances, let each ability handle that
[GlobalClass]
public partial class Buff : Resource
{
    [Export]
    BuffType BuffType;

    [Export]
    float Percentage;

    [Export]
    int Duration;

    //Set to true if the buff targets the user themself
    [Export]
    bool Self = false;

    public float GetPercentage()
    {
        return Percentage;
    }

    public BuffType GetBuffType() {
        return BuffType;
    }


    public int GetDuration()
    {
        return Duration;
    }

    public void SetDuration(int dur)
    {
        Duration = dur;
    }

    public void IncrementDuration(int dur)
    {
        Duration += dur;
    }

    public bool IsSelf()
    {
        return Self;
    }

}
