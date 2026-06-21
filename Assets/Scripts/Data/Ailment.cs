using Godot;
using System;



[GlobalClass]
public partial class Ailment : Resource
{

    [Export]
    public AilmentType AilmentType;

    [Export]
    public int Duration;

    //Let this represent probability
    [Export]
    public float[] PowerLevel = {65, 70, 80, 95 };



}
