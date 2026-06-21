using Godot;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;

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
public enum ShoveType
{
    None,
    Back,
    Right,
    Left,
    Pull
}


//Split this up into active and passive abilities?
[GlobalClass]
public partial class Ability : Resource
{


    [Export]
    protected string Name;

    [Export]
    protected string Description;


    //If this is a single target ability, let it be a multiplier 
    //If this is a rangetype ability, let it be AOE
    [Export]
    protected float Range = 1.0f;

    //Specifically for Positional type abilities
    [Export]
    protected float AimRange = 0.0f;

    [Export]
    protected float[] PowerLevels = { 1.1f, 1.2f, 1.4f, 1.7f };

    [Export]
    protected bool IsUpgradeable = true;

    [Export]
    protected TargetType TargetingType;

    [Export]
    protected SelectionType AimingType;

    [Export]
    protected DamageType Damage_Type;

    [Export]
    protected ElementType Element_Type;

    [Export]
    protected PhysicalType Physical_Type;


    [Export]
    protected Ailment AbilityAilment;

    //Consider turning this into a list so we can have multiple buffs in one ability
    [Export]
    protected BuffData AbilityBuff;

    [Export]
    protected RestorativeType Restore;


    [Export]
    protected ShoveType ShoveDirection;

    //3200 is a good default
    [Export]
    protected float ShoveForce = 0;

    //If true, push/pull from designated point rather than from the User
    [Export]
    protected bool Point;


    [Export]
    protected AI_Priority DecisionType;





    public virtual void AbilityEffect(Character Target, Character User, int level = 0)
    {
        if (Damage_Type != DamageType.None)
        {
            int TotalDamage = CalculateAbilityDamage(Target, User, level);
            //This can potentially heal more damage than is done
            Target.TakeDamage(TotalDamage);
            if (Restore == RestorativeType.Drain)
            {
                User.Heal(TotalDamage / 2);
            }
        }
        if (AbilityBuff != null) { 
            if (TargetingType == TargetType.Self)
            {
                User.AddBuff(AbilityBuff, level);
            }
            else
            {
                Target.AddBuff(AbilityBuff, level);
            }
            
        }
        if (AbilityAilment != null)
        {
            int rand = GD.RandRange(0, 100);
            if (rand < AbilityAilment.PowerLevel[level])
            {
                GD.Print("Successfully applied Ailment: " + AbilityAilment.AilmentType + " Duration: " + AbilityAilment.Duration);
                Target.AddAilment(AbilityAilment.AilmentType, AbilityAilment.Duration);
            }
            else
            {
                GD.Print("Failed to apply ailment");
            }
                
        }


        if (Restore != RestorativeType.None && Restore != RestorativeType.Drain)
        {
            RestorationEffect(Target, User);
        }

        if (ShoveForce != 0) {
            Vector2 Dir = CombatManager.CalculateDirection(ShoveDirection, Target.Position, User.Position);


            Target.Shoved(Dir * ShoveForce);
        }

    }

    public void OnActivate_Multiple(List<Character> targets, Character User, int level = 0)
    {
        for (int i = 0; i < targets.Count; i++)
        {
            AbilityEffect(targets[i], User, level);
        }
        //
    }

    public void OnActivate(Character User, int level = 1)
    {
        if (AimingType == SelectionType.Target)
        {
            AbilityEffect(User.GetStoredTarget(), User, level);
        }
        else
        {
            OnActivate_Multiple(User.GetStoredTargetList(), User, level);
        }

    }

    public int GetHealthRestored(Character User, int level = 0)
    {
        return (int)(PowerLevels[level] * User.BaseData.GetMagic());
    }

    public int CalculateAbilityDamage(Character Target, Character User, int level = 0)
    {
        int TotalDamage = (int)(User.GetAbilityDamage(Damage_Type) * PowerLevels[level]);
        //GD.Print("Ability Level Power Multiplier: " + PowerLevels[level]);
        //GD.Print("Total " + GetAbilityName() +  " Base Damage: " + TotalDamage);
        TotalDamage = Target.GetDamageTaken(TotalDamage, Damage_Type, Element_Type, Physical_Type);
        //GD.Print("Final " + GetAbilityName() +  " Damage: " + TotalDamage);
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
                Target.AilmentContainer.ClearAilments();

                break;
            case RestorativeType.Buff: //Dispel negative Buffs
                Target.BuffContainer.ClearBuffs();
                break;
            case RestorativeType.Magic: //Restore MP
                break;
        }
    }

    public float GetCurrentAbilityPowerLevel(Character User)
    {
        return PowerLevels[User.GetUpgradeLevelPower(this)];
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

    public ShoveType GetShoveType()
    {
        return ShoveDirection;
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

    public float[] GetPowerLevels()
    {
        return PowerLevels;
    }

    public float GetAimRange()
    {
        return AimRange;
    }

}
