using Godot;
using System;
using System.Collections.Generic;



public enum DamageType
{
    None,
    Physical,
    Magic,
    Untyped //Untyped ignores defenses

}

public enum PhysicalType
{
    None,
    Cut,
    Stab,
    Bash //Untyped ignores defenses

}

public enum ElementType
{
    None,
    Fire,
    Ice,
    Electric,
    Demonic,
    Angelic
}


public enum AilmentType
{
    None,
    Poison,
    Sleep,
    Stun,
    Bleed
}

public enum BuffType
{
    None,
    PhysAttack,
    PhysDefense,
    MagAttack,
    MagDefense
}

public enum RestorativeType
{
    None,
    Health,
    Life,   
    Ailment,
    Buff,
    Magic,
    Drain
}

public enum TargetType
{
    NotFriendly,
    Friendly,
    Self,
    Indiscriminate
}

//Split this up into active and passive abilities?
[GlobalClass]
public partial class Ability : Resource
{


    [Export]
    string Name;

    [Export]
    string Description;


    //If this is a single target ability, let it be a multiplier
    //If this is a rangetype ability, let it be AOE
    [Export]
    float Range = 1.0f;

    [Export]
    float Power = 1.0f;

    [Export]
    TargetType TargetingType;

    [Export]
    SelectionType AimingType;

    [Export]
    DamageType Damage;

    [Export]
    ElementType Element;

    [Export]
    PhysicalType Physical;

    [Export]
    AilmentType Ailment;

    [Export]
    BuffType Buff;

    [Export]
    RestorativeType Restore;


    public void AbilityEffect(Character Target, Character User)
    {
        if (Damage != DamageType.None)
        {
            int TotalDamage = ApplyDamage(Target, User);
            if (Restore == RestorativeType.Drain)
            {
                //Drain here
            }
            Target.TakeDamage(TotalDamage);
        }

        if (Ailment != AilmentType.None)
        {

        }

        if (Buff != BuffType.None) 
        { 
        
        }

        if (Restore != RestorativeType.None)
        {

        }
    }

    //Consider abstracting abilities based on aiming type
    public void OnActivate_Single(Character Target, Character User)
    {
        AbilityEffect(Target, User);
    }


    public void OnActivate_Multiple(List<Character> targets, Character User)
    {
        for (int i = 0; i < targets.Count; i++)
        {
            AbilityEffect(targets[i], User);
        }
        //
    }

    public void OnActivate()
    {

    }

    public int ApplyDamage(Character Target, Character User)
    {
        int TotalDamage = User.CalculateAbilityDamage(Damage);
        TotalDamage = Target.CalculateDamageTaken(TotalDamage, Damage, Element, Physical);

        return TotalDamage;
    }

    public void Restoration()
    {
        switch (Restore)
        {
            case RestorativeType.Health: //Heal
                break;
            case RestorativeType.Life: //Revive
                break;
            case RestorativeType.Ailment: //Dispel Ailments
                break;
            case RestorativeType.Buff: //Dispel negative Buffs
                break;
            case RestorativeType.Magic: //Restore MP
                break;
        }
    }

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
        return Range;
    }



    public TargetType GetTargetingType()
    {
        return TargetingType;
    }

    public SelectionType GetAimingType()
    {
        return AimingType;
    }



}
