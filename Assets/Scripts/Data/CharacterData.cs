using Godot;
using System;



//Let this class contain the base values of a character
[GlobalClass]
public partial class CharacterData : Resource
{

    [Export]
    private String Name;

    [Export]
    private int MaxHealth = 100;

    [Export]
    private float MoveRange = 500.0f;

    [Export]
    private int Attack = 20;

    [Export]
    private int Magic = 20;

    [Export]
    private int PhysDefense = 10;

    [Export]
    private int MagDefense = 10;

    public int GetMaxHealth()
    {
        return MaxHealth;
    }

    public float GetMoveRange()
    {
        return MoveRange;
    }

    public int GetAttack()
    {
        return Attack;
    }

    public int GetMagic()
    {
        return Magic;
    }


    public float GetPhysDefense()
    {
        return PhysDefense;
    }

    public float GetMagDefense()
    {
        return MagDefense;
    }


    public String GetCharacterName()
    {
        return Name;
    }

}
