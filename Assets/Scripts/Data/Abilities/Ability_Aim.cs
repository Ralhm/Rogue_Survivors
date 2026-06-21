using Godot;
using System;


//Similar to Ability_Directional, but the player must manually aim where the ability is shot out from
//Aim using the mouse, draw a line from the aimer to the mouse pos
[GlobalClass]
public partial class Ability_Aim : Ability
{



    [Export]
    float Length;

    [Export]
    float Width;


    ShapeCast2D ShapeCast;

    public override void AbilityEffect(Character Target, Character User, int level = 1)
    {
        base.AbilityEffect(Target, User, level);
        //You can either
        //Do a series of Raytraces
        //Do a Sphere Trace
        //Spawn a collider temporarily
    }


}
