using Godot;
using System;
using System.Collections.Generic;


//Let G Be the the damage/healing done
//Let H be a bias effected by what this DecisionMakerlikes to do
//Let F be the final say in the matter
//Add a randomly generated number so enemies don't always make the same decision
//To keep things simple, lets specifically target characters and not position
public partial class Decision
{
    int F;
    Character Target;
    StoredAction Action;
    Ability Ability;
    float MinDistanceToTarget;

    public void SetValues(int f, Character target, StoredAction action, float minDist, Ability ability = null)
    {
        F = f;
        Target = target;
        Action = action;
        MinDistanceToTarget = minDist;
        Ability = ability;
    }

    public Character GetTarget()
    {
        return Target;
    }

    public StoredAction GetStoredAction()
    {
        return Action;
    }

    public Ability GetAbility() { 
        return Ability;
    }

    public float GetMinDistanceToTarget() { 
        return MinDistanceToTarget;
    }


    public void ClearDecision()
    {
        F = 0;
        Target = null;
        Action = StoredAction.None;
        Ability = null;
        MinDistanceToTarget = 0;
    }


}
