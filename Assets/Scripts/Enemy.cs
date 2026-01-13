using Godot;
using System;

public partial class Enemy : Character
{


    //public override 

    public override void _Ready()
    {
        base._Ready();
        IsEnemy = true;
    }

    public override int CalculateNormalAttackDamage()
    {
        return base.CalculateNormalAttackDamage();
    }

    public override int CalculateAbilityDamage(DamageType type)
    {
        return base.CalculateAbilityDamage(type);
    }

    public override int CalculateDamageTaken(int Power, DamageType type, ElementType element, PhysicalType physical)
    {
        return base.CalculateDamageTaken(Power, type, element, physical);
    }


}
