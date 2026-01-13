using Godot;
using System;

public partial class CounterBox : Area2D
{

    bool AllyAligned = false;

    Character Parent;

    public override void _Ready()
    {
        base._Ready();

        Parent = GetParent() as Character;

        AllyAligned = !Parent.GetIsEnemy();
        //BodyEntered += OnBodyEntered;
        AreaEntered += OnBodyEntered;
    }

    public void OnBodyEntered(Node2D node2D)
    {
        //GD.Print("Countering Box");
        //If we're the one being shoved, return
        if (Parent.GetIsShoved())
        {
            return;
        }

        Character target = node2D.GetParent() as Character;

        if (target == null || !target.GetIsShoved()) {
            GD.Print(node2D.Name + ": Shoving is NOT Occurring!");
            return;
        }



        //If they're the same allegiance, they both take damage
        if (target.GetIsEnemy() == Parent.GetIsEnemy())
        {
            target.OnShovedInto();
            Parent.OnShovedInto();
        }
        else //If they're the opposite allegiance, this one should attack the other.
        {
            
            Parent.Counter(target);
        }



    }

}
