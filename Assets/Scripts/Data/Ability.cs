using Godot;
using System;
using System.Collections.Generic;

//While rather monolithic in structure, this Abilty class allows abilities to have multiple factors to them
//Sure, some many things will go unused for most abilities
//But it gives the designer (me) the power to mix and match ability possibilities
//I could make a skill that buffs AND heals, or does damage and applies an ailment


public enum SelectionType
{
    Target,
    Position,
    Aim //Reserve Direction specifically for Ability_Aim subclass
}


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
    Bash 

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
    DamageType Damage_Type;

    [Export]
    ElementType Element_Type;

    [Export]
    PhysicalType Physical_Type;


    [Export]
    Ailment AbilityAilment;

    [Export]
    Buff AbilityBuff;

    [Export]
    RestorativeType Restore;

    [Export]
    AI_Priority DecisionType;

    public virtual void AbilityEffect(Character Target, Character User)
    {
        if (Damage_Type != DamageType.None)
        {
            int TotalDamage = CalculateAbilityDamage(Target, User);
            if (Restore == RestorativeType.Drain)
            {
                //Drain here
            }
            Target.TakeDamage(TotalDamage);
            if (Restore == RestorativeType.Drain)
            {
                User.Heal(TotalDamage / 2);
            }
        }

        if (AbilityAilment != null)
        {

        }

        if (AbilityBuff != null) { 
            if (AbilityBuff.IsSelf())
            {
                User.AddBuff(AbilityBuff);
            }
            else
            {
                Target.AddBuff(AbilityBuff);
            }
            
        }

        //if (AbilityBuff != BuffType.None) 
        //{ 
        //    Target
        //}

        if (Restore != RestorativeType.None && Restore != RestorativeType.Drain)
        {
            RestorationEffect(Target, User);
        }
    }

    public void OnActivate_Multiple(List<Character> targets, Character User)
    {
        for (int i = 0; i < targets.Count; i++)
        {
            AbilityEffect(targets[i], User);
        }
        //
    }

    public void OnActivate(Character User)
    {
        if (AimingType == SelectionType.Target)
        {
            AbilityEffect(User.GetStoredTarget(), User);
        }
        else
        {
            OnActivate_Multiple(User.GetStoredTargetList(), User);
        }

    }

    public int GetHealthRestored(Character User)
    {
        return (int)(Power * User.BaseData.GetMagic());
    }

    public int CalculateAbilityDamage(Character Target, Character User)
    {
        int TotalDamage = User.GetAbilityDamage(Damage_Type);
        //GD.Print("Total Base Damage: " + TotalDamage);
        TotalDamage = Target.GetDamageTaken(TotalDamage, Damage_Type, Element_Type, Physical_Type);
        //GD.Print("Final Ability Damage: " + TotalDamage);
        return TotalDamage;
    }


    public void RestorationEffect(Character User, Character Target)
    {
        switch (Restore)
        {
            case RestorativeType.Health: //Heal
                Target.Heal(GetHealthRestored(User));
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

    public virtual ShoveType GetShoveType()
    {
        return ShoveType.None;
    }

    public DamageType GetDamageType()
    {
        return Damage_Type;
    }

    public RestorativeType GetRestorativeType()
    {
        return Restore;
    }

    public AI_Priority GetAIType()
    {
        return DecisionType;
    }

}
