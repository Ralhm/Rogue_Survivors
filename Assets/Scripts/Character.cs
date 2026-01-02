using Godot;
using System;


public enum CurrentState
{
    Idling,
    Moving, //Selected and moving to a new position
    Navigating, //Character is executing move towards projected position
    SelectingAction, //Player has selected where they want the character to be, now they must select an action
    SelectingTarget, //Aiming for a target
    SelectingDirection, //Aiming For a directional ability
    SelectingDirectionPos, //Aiming for an AOE
    ActionSet, //If their actions has been selected, and are waiting for the player to execute
    Dead
}

public enum StoredAction
{
    Attack,
    Ability,
    Defend,
    Wait,
    None
}

public partial class Character : CharacterBody2D
{

    public AnimatedSprite2D Sprite;

    [Export]
    private AnimatedSprite2D Projection;

    public NavigationAgent2D NavAgent;

    //Create a number icon that shows the order in which the characters will act
    [Export]
    AnimatedSprite2D OrderIcon;

    [Export]
    public CurrentState CharacterState;

    [Export]
    StoredAction Action;

    [Export]
    Ability StoredAbility;

    //If targeting a specific character
    [Export]
    Character StoredTarget;


    [Export]
    public int CurrentHealth;

    [Export]
    public int Speed = 450;

    [Export]
    public CharacterData BaseData;

    [Export]
    public CharacterUpgradeContainer UpgradeData;


    //If our attack is an AOE aimed at a position
    [Export]
    public Vector2 StoredTargetPos;

    //If our attack is aimed at a certain direction
    [Export]
    public Vector2 StoredTargetDir;

    //The position this Character has started their movement from
    //Use to compare distance travelled and prevent from moving out of range
    [Export]
    public Vector2 StartingLocation;

    [Export]
    public Vector2 EndLocation;


    [Export]
    public Vector2 MoveDir;



    [Export]
    private float DistanceTraveled;

    [Export]
    public bool IsMoving;

    [Export]
    private bool IsDashing;

    public int CharacterIndex;

    [Export]
    public bool Dashed;

    [Export]
    public bool IsTired;



    private string StoredAnimDir = "Front";



    public override void _Ready()
    {
        base._Ready();
        StartingLocation = Position;
        Sprite = GetNode<Godot.AnimatedSprite2D>("Sprite");
        NavAgent = GetNode<Godot.NavigationAgent2D>("NavAgent");
        Projection.Hide();

    }


    public override void _Process(double delta)
    {
        base._Process(delta);


    }



    public void Navigate()
    {
        NavAgent.TargetPosition = EndLocation;
        CharacterState = CurrentState.Navigating;
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        DistanceTraveled = (StartingLocation - Position).Length();

        float Dot = MoveDir.Dot(Vector2.Up);


        if (MoveDir != Vector2.Zero && CharacterState == CurrentState.Moving)
        {
            Velocity = MoveDir * Speed;
            if (DistanceTraveled >= BaseData.GetMoveRange())
            {
                Velocity += (Velocity.Length() * (StartingLocation - Position).Normalized());
            }
            if (Dot > 0.5) //Up
            {
                StoredAnimDir = "Back";
                Sprite.Play("Back_Walk");
            }
            else if (Dot < 0.5 && Dot > -0.5) //Side
            {
                if (MoveDir.X < 0)
                {
                    Sprite.FlipH = false;
                }
                else
                {
                    Sprite.FlipH = true;
                }
                StoredAnimDir = "Side";
                Sprite.Play("Side_Walk");
            }
            else
            {
                StoredAnimDir = "Front";
                Sprite.Play("Front_Walk");
            }
        }
        else
        {
            Velocity = new Vector2(Mathf.MoveToward(Velocity.X, 0, Speed), Mathf.MoveToward(Velocity.Y, 0, Speed));
            Sprite.Play(StoredAnimDir + "_Idle");
            //Sprite.FlipH = true;
        }

        

        MoveAndSlide();

    }

    public void OnNewTurn()
    {
        StartingLocation = Position;
        CharacterState = CurrentState.Idling;
        HideProjection();
        if (IsTired)
        {
            IsTired = false;
        }
        if (Dashed)
        {
            IsTired = true;
            Dashed = false;
            
        }
    }

    public void SetMoveDir(Vector2 dir)
    {
        MoveDir = dir;
    }

    public void SetIsMoving(bool isMoving)
    {
        IsMoving = isMoving;
    }

    public void OnPossession()
    {
        CharacterState = CurrentState.Moving;
    }




    public void SetCurrentState(CurrentState state)
    {
        CharacterState = state;
    }

    //Forgo doing an action in exchange for doubling movement range
    public void Dash()
    {
        IsDashing = true;
    }

    public void CancelPossession()
    {
        CharacterState = CurrentState.Idling;
        Position = StartingLocation;
    }


    //Reset position to starting position
    public void CancelDash()
    {
        CharacterState = CurrentState.Moving;
        Position = StartingLocation;
    }

    public void CancelAction() //If action has been set, compelete reset
    {
        Position = StartingLocation;
        CharacterState = CurrentState.Moving;
        StoredAbility = null;
        Action = StoredAction.None;
        HideProjection();

    }

    public void CancelTargeting() //If we're selecting a target
    {
        Action = StoredAction.None;
        CharacterState = CurrentState.SelectingAction;
        StoredAbility = null;
    }
    public void HideProjection()
    {
        Projection.Hide();
        Projection.Position = Vector2.Zero;
    }


    public void CancelMenu() //If we're selecting an action to take
    {

        CharacterState = CurrentState.Moving;
    }


    public void Aim()
    {

    }

    public void TakeDamage(int Damage)
    {
        CurrentHealth -= Damage;
    }

    public void Heal(int Amount) 
    {
        CurrentHealth += Amount;
    }

    public void Attack()
    {

    }

    public void ActivateAbility()
    {
        StoredAbility.OnActivate();
    }

    //Get sent in a direction
    public void Shoved(Vector2 Direction)
    {

    }

    //Strike and enemy that's been sent your way
    public void Counter(Character Enemy)
    {

    }

    public void SetStoredAction(StoredAction action)
    {
        Action = action;
    }

    public void SetActionSet()
    {
        CharacterState = CurrentState.ActionSet;
        EndLocation = Position;
        Position = StartingLocation;
        //Sprite.
        //Projection.Play
        Projection.Show();
        Projection.GlobalPosition = EndLocation;
        //Play Animation and set Order Icon Here
    }

    public void SetStoredAbility(Ability ability)
    {
        StoredAbility = ability;
    }

    public void SetStoredDir(Vector2 Direction)
    {
        StoredTargetDir = Direction;
    }

    public void SetStoredTarget(Character target)
    {
        StoredTarget = target;
    }

    public void SetStoredTargetPos(Vector2 pos)
    {
        StoredTargetPos = pos;
    }


    public float GetMoveRange()
    {
        return BaseData.GetMoveRange() * UpgradeData.GetMoveRangeMult();
    }

    public float GetDashRange()
    {
        return BaseData.GetMoveRange() * UpgradeData.GetMoveRangeMult() * 2;
    }

    public float GetOffensiveRange()
    {
        if (Action == StoredAction.Attack)
        {
            return BaseData.GetAttackRange() * UpgradeData.GetAttackRangeMult();
        }
        else if (Action == StoredAction.Ability)
        {
            return BaseData.GetAttackRange() * UpgradeData.GetAttackRangeMult() * StoredAbility.GetAbilityRange();
        }
        return 0f;
        
    }

    //This takes the projection into account as well. 
    public bool GetProjectionIsInRange(float Range, Vector2 Position)
    {
        if ((EndLocation - Position).Length() <= Range)
        {
            return true;
        }
        return false;
    }

    public CurrentState GetCurrentState()
    {
        return CharacterState;
    }

    public StoredAction GetStoredActionType()
    {
        return Action;
    }

    public bool GetIsDashing()
    {
        return IsDashing;
    }
}
