using Godot;
using System;

[GlobalClass]
public partial class Ability_DoubleBuff : Ability
{

    [Export]
    BuffData[] BonusBuffs;

    public override void AbilityEffect(Character Target, Character User, int level = 0)
    {
        base.AbilityEffect(Target, User, level);

        for (int i = 0; i < BonusBuffs.Length; i++) {
            if (TargetingType == TargetType.Self)
            {
                User.AddBuff(BonusBuffs[i], level);
            }
            else
            {
                Target.AddBuff(BonusBuffs[i], level);
            }
        }



    }


}
