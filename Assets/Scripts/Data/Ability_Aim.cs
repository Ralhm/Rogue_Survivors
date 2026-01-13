using Godot;
using System;


//Similar to Ability_Directional, but the player must manually aim where the ability is shot out from
[GlobalClass]
public partial class Ability_Aim : Ability
{



    [Export]
    float ShoveForce;

    [Export]
    float Length;

    [Export]
    float Width;


    ShapeCast2D ShapeCast;

    public override void AbilityEffect(Character Target, Character User)
    {
        base.AbilityEffect(Target, User);
        //You can either
        //Do a series of Raytraces
        //Do a Sphere Trace
        //Spawn a collider temporarily
    }


}
