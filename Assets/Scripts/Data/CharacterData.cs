using Godot;
using System;



//Let this class contain the base, DEFAULT values of a character
[GlobalClass]
public partial class CharacterData : Resource
{

    [Export]
    private String Name;

    [Export]
    private int MaxHealth = 100;

    [Export]
    private float MoveRange = 750.0f;

    [Export]
    public float AttackRange = 375;

    [Export]
    private int Attack = 20;

    [Export]
    private int Magic = 20;

    [Export]
    private int PhysDefense = 10;

    [Export]
    private int MagDefense = 10;



    [Export]
    private Ability[] DefaultAbilityList;

    [Export]
    private ElementType ElementalWeakness;

    [Export]
    private ElementType ElementalResistance;

    [Export]
    private PhysicalType PhysicalWeakness;

    [Export]
    private PhysicalType PhysicalResistance;

    public int GetMaxHealth()
    {
        return MaxHealth;
    }

    public float GetMoveRange()
    {
        return MoveRange;
    }

    public float GetAttackRange()
    {
        return AttackRange;
    }


    public int GetAttack()
    {
        return Attack;
    }

    public int GetMagic()
    {
        return Magic;
    }


    public int GetPhysDefense()
    {
        return PhysDefense;
    }

    public int GetMagDefense()
    {
        return MagDefense;
    }


    public String GetCharacterName()
    {
        return Name;
    }

    public ElementType GetElemWeakness()
    {
        return ElementalWeakness;
    }

    public ElementType GetElemResistance()
    {
        return ElementalResistance;
    }

    public PhysicalType GetPhysWeakness()
    {
        return PhysicalWeakness;
    }

    public PhysicalType GetPhysResistance()
    {
        return PhysicalResistance;
    }

    public Ability[] GetDefaultAbilities() {
        return DefaultAbilityList;
    }



}
