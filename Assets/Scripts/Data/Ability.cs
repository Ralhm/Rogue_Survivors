using Godot;
using System;


//Split this up into active and passive abilities?
public partial class Ability : Resource
{


    [Export]
    string Name;

    [Export]
    string Description;

    [Export]
    float RangeMult = 1.0f;

    [Export]
    SelectionType AimingType;

    [Export]
    bool IsFriendly;


    public string GetAbilityName()
    {
        return Name;
    }

    public string GetDescription()
    {
        return Description;
    }


    public float GetAbilityRange()
    {
        return RangeMult;
    }

    public void OnActivate()
    {

    }

    public bool GetIsFriendly()
    {
        return IsFriendly;
    }


    public SelectionType GetAimingType()
    {
        return AimingType;
    }

}
