using Godot;
using System;


public enum AilmentType
{
    None,
    Poison,
    Sleep,
    Stun,
    Confused,
    Bleed
}

//Ailments only take effect right before the user executes their action
//I GUESS I could make a bunch of ailment classes, one for each ailment...
//ehhh, maybe later
[GlobalClass]
public partial class Ailment : Resource
{

    [Export]
    AilmentType AilmentType;

    [Export]
    int Duration;


    public void AilmentEffect(Character victim)
    {
        switch (AilmentType) { 
            case AilmentType.Poison:
                Effect_Poison(victim); 
                break;
            case AilmentType.Sleep:
                Effect_Sleep(victim);
                break;
            case AilmentType.Bleed:
                Effect_Bleed(victim);
                break;
            case AilmentType.Stun:
                Effect_Stun(victim);
                break;
        }
    }

    public void Effect_Poison(Character victim)
    {
        victim.TakeDamage((int)(victim.BaseData.GetMaxHealth() * 0.05f));
    }

    public void Effect_Sleep(Character victim)
    {
        victim.SetSkipTurn();
    }

    public void Effect_Stun(Character victim) {
        victim.SetSkipTurn();
    }

    public void Effect_Bleed(Character victim) {
        victim.TakeDamage((int)(victim.BaseData.GetMaxHealth() * 0.05f));
    }

}
