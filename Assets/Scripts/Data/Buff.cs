using Godot;
using System;

public partial class Buff
{


    int Duration;

    public BuffData Data;

    int Level;

    public void SetDuration(int dur)
    {
        Duration = dur;
    }

    public void IncrementDuration(int dur)
    {
        Duration += dur;
    }

    public void SetData(BuffData data, int level)
    {
        Data = data;
        Level = level;
        SetDuration(Data.GetDuration());
    }

    public float GetBuffPower()
    {
        return Data.GetPowerLevel(Level);
    }

}
