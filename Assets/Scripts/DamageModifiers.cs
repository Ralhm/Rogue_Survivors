using Godot;
using System;

[GlobalClass]
public partial class DamageModifiers : Resource
{

    [Export]
    float ResistanceMultiplier = 1.5f;

    [Export]
    float WeaknessMultiplier = 0.5f;

    [Export]
    float DefendMultiplier = 0.4f;

    public float GetWeakness()
    {
        return WeaknessMultiplier;
    }

    public float GetResistance()
    {
        return WeaknessMultiplier;
    }

    public float GetDefend()
    {
        return DefendMultiplier;
    }

}
