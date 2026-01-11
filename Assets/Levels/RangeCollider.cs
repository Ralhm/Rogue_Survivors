using Godot;
using System;
using System.Collections.Generic;

public partial class RangeCollider : Area2D
{

    public CircleShape2D Shape;

    public List<Character> TargetList;

    public override void _Ready()
    {
        base._Ready();
        BodyEntered += OnNodeEnter;
        BodyExited += OnNodeExit;
        AreaEntered += OnAreaEnter;
        AreaExited += OnAreaExit;

    }

    public void OnNodeEnter(Node2D node2D)
    {        
        CombatManager.Instance.AddTarget((Character)node2D);   
    }

    public void OnNodeExit(Node2D node2D)
    {
        CombatManager.Instance.RemoveTarget((Character)node2D);
      
    }

    public void OnAreaEnter(Node2D node2D)
    {
        //Nasty ass string literal
        if (node2D.GetParent().Name == "Projection")
        {
            GD.Print(node2D.GetParent().GetParent().Name);

            CombatManager.Instance.AddTarget_Projection((Ally)node2D.GetParent().GetParent());
        }

    }

    public void OnAreaExit(Node2D node2D)
    {
        if (node2D.GetParent().Name == "Projection")
        {
            CombatManager.Instance.RemoveTarget_Projection((Ally)node2D.GetParent().GetParent());
        }
            

    }

    //Let Allies be layer 1
    public void SetCheckForAllies(bool newCheck)
    {
        SetCollisionMaskValue(1, newCheck);
    }

    //Let Enemies be layer 2
    public void SetCheckForEnemies(bool newCheck)
    {
        SetCollisionMaskValue(2, newCheck);
    }


    public void SetRadius(float radius)
    {
        Shape.Radius = radius;
    }

}
