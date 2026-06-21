using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class Ability_HailMary : Ability
{

    [Export]
    public BuffData[] PotentialBuffs;


    public override void AbilityEffect(Character Target, Character User, int level = 0)
    {
        int rand = GD.RandRange(0, PotentialBuffs.Length - 1);

        switch (rand)
        {
            case 0:
                Target.AddBuff(PotentialBuffs[0], level);
                break;
            case 1:
                Target.AddBuff(PotentialBuffs[1], level);
                break;
            case 2:
                Target.AddBuff(PotentialBuffs[2], level);
                break;
            case 3:
                Target.AddBuff(PotentialBuffs[3], level);
                break;
            case 4:
                Target.AddBuff(PotentialBuffs[4], level);
                break;
        }
    }


}
