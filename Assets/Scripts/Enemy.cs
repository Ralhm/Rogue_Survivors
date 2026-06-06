using Godot;
using System;

public partial class Enemy : Character
{


    DecisionMaker AIEngine;


    //Let this be the deciding factor on how the Enemy should behave
    //Is it a support type, an offensive type, a defensive type, or maybe something else?
    [Export]
    AI_Priority MainPriority;


    float MinDistToTarget;
    //public override 

    public override void _Ready()
    {
        base._Ready();
        IsEnemy = true;
        AIEngine = new DecisionMaker();
        AIEngine.SetOwner(this);
    }

    public override void BeginAction()
    {
        
        AilmentContainer.AilmentEffects();
        if (SkippingTurn)
        {
            SkippingTurn = false;
            CombatManager.Instance.ExecuteNextAction();
        }
        
        if (AIEngine == null)
        {
            GD.Print("NO AI ENGINE SET");
            return;
        }
        AIEngine.MakeDecision();

        StoredTarget = AIEngine.GetTarget();
        BeginNavigation();
    }

    //Rather than set a specific point in space that we want the enemy AI to go to
    //We navigate to the targets position until we reach the minimum distance required to execute the action
    //That distance being the ability/attack range
    public override void BeginNavigation()
    {
        if (AIEngine.GetTarget() == null)
        {
            GD.Print("----NO TARGET SET----");
            CombatManager.Instance.ExecuteNextAction();
        }
        EndLocation = AIEngine.GetTarget().Position;
        base.BeginNavigation();
    }

    public override void Navigate()
    {

        //Check if we're within range of the target
        MinDistToTarget = (EndLocation - Position).Length();
        if (MinDistToTarget <= AIEngine.GetMinDistanceToTarget())
        {
            FinishNavigation();
            return;
        }

        //Fail-Safe. Check if we've reached the limit of move range
        if (DistanceTraveled >= BaseData.GetMoveRange())
        {
            FinishNavigation();
            return;
        }

        NavAgent.Velocity = GlobalPosition.DirectionTo(NavAgent.GetNextPathPosition()) * MoveSpeed * 1.5f;
    }

    public override void FinishNavigation()
    {

        //Failsafe Check, maybe remove if deemed unneccsary
        //I've added a little addition to give the enemies a little wiggle room just in case they only barely made it
        //Again, consider removing this check entirely 
        if ((MinDistToTarget - 10) <= AIEngine.GetMinDistanceToTarget())
        {
            GD.Print(Name + ": I'm Within Range, Executing my action!");
            ExecuteAction();
        }
        else
        {
            GD.Print(Name + ": I'M NOT WITHIN RANGE OF MY TARGET");
        }
        AIEngine.ClearDecision();

        CharacterState = CurrentState.Acting;
        CharacterState = CurrentState.Idling;
        NavAgent.AvoidanceEnabled = false;
        Obstacle.AvoidanceEnabled = true;

        CombatManager.Instance.ExecuteNextAction();
    }

    public override void ExecuteAction()
    {
        base.ExecuteAction();
    }



    public AI_Priority GetPriority() { 
        return MainPriority;
    }

}
