using Godot;
using System;


//Let this class contain all of a characters upgrade related content
[GlobalClass]
public partial class CharacterUpgradeContainer : Resource
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

    public void SetMoveRangeMult(float Increment)
    {
        MoveRangeMult += Increment;
    }


    public float GetAttackMult()
    {
        return AttackMult;
    }

    public void SetAttackMult(int Increment)
    {
        AttackMult += Increment;
    }

    public float GetMagicMult()
    {
        return MagicMult;
    }
    public void SetMagicMult(int Increment)
    {
        MagicMult += Increment;
    }

    public float GetPhysDefenseMult()
    {
        return PhysDefenseMult;
    }

    public void SetPhysDefenseMult(int Increment)
    {
        PhysDefenseMult += Increment;
    }

    public float GetMagDefenseMult()
    {
        return MagDefenseMult;
    }

    public void SetMagDefenseMult(int Increment)
    {
        MagDefenseMult += Increment;
    }












}
