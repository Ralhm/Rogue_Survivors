using Godot;
using System;

[GlobalClass]
public partial class Ability_Sacrifice : Ability
{

    [Export]
    int HealthLoss;

    public override void AbilityEffect(Character Target, Character User, int level = 1)
    {
        base.AbilityEffect(Target, User, level);

        User.TakeDamage(HealthLoss);
    }


}
