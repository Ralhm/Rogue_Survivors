using Godot;
using System;


public enum BuffType
{
    None,
    PhysAttack,
    PhysDefense,
    MagAttack,
    MagDefense,
    Drain
}

//We don't need to make separate buff classes or instances, let each ability handle that
[GlobalClass]
public partial class BuffData : Resource
{
    [Export]
    string BuffName;

    [Export]
    BuffType BuffType;

    //Let this represent buff power as the ability levels up
    [Export]
    float[] PowerLevel = { 0.2f, 0.25f, 0.35f, 0.5f };

    [Export]
    int BaseDuration = 3;

    [Export]
    public bool isDebuff = false;

    public float GetPowerLevel(int level)
    {
        return PowerLevel[level];
    }

    public BuffType GetBuffType() {
        return BuffType;
    }


    public int GetDuration()
    {
        return BaseDuration;
    }




}
