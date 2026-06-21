using Godot;
using System;
using System.Collections.Generic;


//Let this class contain all of a characters upgrade related content
public partial class CharacterUpgradeContainer
{


    [Export]
    private int MaxHealthMod = 0;

    [Export]
    private float MoveRangeMult = 1.0f;

    [Export]
    private float AttackRangeMult = 1.0f;

    [Export]
    private float AttackMult = 1.0f;

    [Export]
    private float MagicMult = 1.0f;

    [Export]
    private float PhysDefenseMult = 1.0f;

    [Export]
    private float MagDefenseMult = 1.0f;

    public Dictionary<Ability, int> AbilityLevels = new Dictionary<Ability, int>();

    public int GetMaxHealth()
    {
        return MaxHealthMod;
    }

    public void SetMaxHealth(int Increment)
    {
        MaxHealthMod += Increment;
    }


    public float GetMoveRangeMult()
    {
        return MoveRangeMult;
    }


    public float GetAttackRangeMult()
    {
        return AttackRangeMult;
    }

    public float SetAttackRangeMult(float Increment)
    {
        return AttackRangeMult += Increment;
    }


    public void SetMoveRangeMult(float Increment)
    {
        MoveRangeMult += Increment;
    }


    public float GetAttackMult()
    {
        return AttackMult;
    }

    public void SetAttackMult(float Increment)
    {
        AttackMult += Increment;
    }


    public float GetMagicMult()
    {
        return MagicMult;
    }
    public void SetMagicMult(float Increment)
    {
        MagicMult += Increment;
    }

    public float GetPhysDefenseMult()
    {
        return PhysDefenseMult;
    }

    public void SetPhysDefenseMult(float Increment)
    {
        PhysDefenseMult += Increment;
    }

    public float GetMagDefenseMult()
    {
        return MagDefenseMult;
    }

    public void SetMagDefenseMult(float Increment)
    {
        MagDefenseMult += Increment;
    }


    public void UpgradeAbility(Ability ability)
    {
        AbilityLevels[ability] += 1;
    }









}
