using Godot;
using System;
using System.Collections.Generic;


public enum AilmentType
{
    None,
    Poison,
    Sleep,
    Stun,
    Confused, //IDK if I wann do confusion, maybe later when more of the game is done
    Bleed
}

//A simple implementation
//I tried a more complicated implementation and it was too much
public partial class AilmentContainer : Resource
{

    int SleepDuration;
    int StunDuration;
    int PoisonDuration;
    int BleedDuration;

    public Character Owner;

    public void AddAilment(AilmentType type, int dur)
    {

        switch (type)
        {
            case AilmentType.Poison:
                PoisonDuration += dur;
                break;
            case AilmentType.Sleep:
                if (SleepDuration == 0)
                {
                    SleepDuration += dur;
                }
                break;
            case AilmentType.Bleed:
                BleedDuration += dur;
                break;
            case AilmentType.Stun:
                if (StunDuration == 0)
                {
                    StunDuration += dur;
                }
                break;
        }
    }

    public void AilmentEffects()
    {
        GD.Print("Stun Duration: " + StunDuration);
        if (PoisonDuration > 0) {
            Effect_Poison();
        }
        if (StunDuration > 0 ) {
            Effect_Stun();
        }
        if (BleedDuration > 0) { 
            Effect_Bleed();
        }
        if (SleepDuration > 0)
        {
            Effect_Sleep();
        }
        DecrementAilmentTimers();
    }

    public void ClearAilments()
    {
        PoisonDuration = 0;
        StunDuration = 0;
        BleedDuration = 0;
        SleepDuration = 0;
    }

    public void DecrementAilmentTimers()
    {
        if (PoisonDuration > 0)
        {
            PoisonDuration -= 1;

        }
        if (SleepDuration > 0)
        {
            SleepDuration -= 1;
        }
        if (BleedDuration > 0)
        {
            BleedDuration -= 1;          
        }
        if (StunDuration > 0)
        {
            StunDuration -= 1;
        }
    }



    public void Effect_Poison()
    {
        //victim.TakeDamage((int)(victim.BaseData.GetMaxHealth() * 0.05f));
        Owner.TakeDamage(5);
    }

    public void Effect_Sleep()
    {
        GD.Print("Asleep! Skipping Turn!");
        Owner.SetSkipTurn();
    }

    public void Effect_Stun()
    {
        GD.Print("Stunned! Skipping Turn!");
        Owner.SetSkipTurn();
    }

    public void Effect_Bleed()
    {
        //victim.TakeDamage((int)(victim.BaseData.GetMaxHealth() * 0.05f));
        Owner.TakeDamage(5);
    }

}
