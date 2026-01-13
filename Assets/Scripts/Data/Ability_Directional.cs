using Godot;
using System;


public enum ShoveType
{
    None,
    Back,
    Right,
    Left,
    Pull
}

[GlobalClass]
public partial class Ability_Directional : Ability
{

    [Export]
    ShoveType ShoveDirection;

    [Export]
    float ShoveForce = 3200.0f;

    //If true, push/pull from designated point rather than from the User
    [Export]
    bool Point;

         
    public override void AbilityEffect(Character Target, Character User)
    {
        base.AbilityEffect(Target, User);

        Vector2 Dir = CombatManager.CalculateDirection(ShoveDirection, Target.Position, User.Position);


        Target.Shoved(Dir * ShoveForce);
    }

    public override ShoveType GetShoveType()
    {
        return ShoveDirection;
    }

}
