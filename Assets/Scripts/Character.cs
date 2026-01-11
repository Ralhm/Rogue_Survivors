using Godot;
using System;
using System.Collections.Generic;


public enum CurrentState
{
    Idling,
    Moving, //Selected and moving to a new position
    Navigating, //Character is executing move towards projected position
    Acting, //Character is executing their stored action
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
    protected AnimatedSprite2D Projection;

    [Export]
    Node2D TargetIndicator;



    public NavigationAgent2D NavAgent;



    [Export]
    public CurrentState CharacterState;

    [Export]
    protected StoredAction CharacterAction;

    protected Ability StoredAbility;

    //If targeting a specific character
    protected Character StoredTarget;

    

    [Export]
    public int CurrentHealth;

    [Export]
    public int Speed = 450;

    [Export]
    public CharacterData BaseData;

    [Export]
    public DamageModifiers DamageData;



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
    protected float DistanceTraveled;

    [Export]
    public bool IsMoving;

    [Export]
    protected bool IsDashing;

    public int CharacterIndex;

    [Export]
    public bool Dashed;

    [Export]
    public bool IsTired;

    protected bool IsEnemy = false;

    protected string StoredAnimDir = "Front";

    public override void _Ready()
    {
        base._Ready();
        StartingLocation = Position;
        Sprite = GetNode<Godot.AnimatedSprite2D>("Sprite");
        NavAgent = GetNode<Godot.NavigationAgent2D>("NavAgent");
        HideProjection();
        NavAgent.VelocityComputed += OnVelocityComputed;
        CurrentHealth = BaseData.GetMaxHealth();
        GD.Print("Max Health: " + BaseData.GetMaxHealth());
        GD.Print("Current Health: " + CurrentHealth);
    }

    public override void _Process(double delta)
    {
        base._Process(delta);


    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        DistanceTraveled = (StartingLocation - Position).Length();

        float Dot = MoveDir.Dot(Vector2.Up);


 
        if (MoveDir != Vector2.Zero && CharacterState == CurrentState.Moving)
        {
            Velocity = MoveDir * Speed;

            if (DistanceTraveled >= GetMoveRange())
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

        if (CharacterState == CurrentState.Navigating)
        {
            //GD.Print("Navigating...");
            Navigate();
        }
        else
        {
            MoveAndSlide();
        }
            

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

    public void BeginNavigation()
    {
        GD.Print("Beginning Navigation...");
        if (IsDashing)
        {
            Dashed = true;
            IsDashing = false;
        }
        
        NavAgent.TargetPosition = EndLocation;
        CharacterState = CurrentState.Navigating;


        //NavAgent.Velocity = Speed;
    }

    public void Navigate()
    {
        if (NavAgent.IsNavigationFinished())
        {
            FinishNavigation();
            return;
        }
        NavAgent.Velocity = GlobalPosition.DirectionTo(NavAgent.GetNextPathPosition()) * Speed * 1.5f;

    }

    public void OnVelocityComputed(Vector2 vel)
    {
        Position += vel * (float)GetPhysicsProcessDeltaTime();
    }

    public void FinishNavigation()
    {
        CharacterState = CurrentState.Acting;
        switch (CharacterAction)
        {
            case StoredAction.Attack:
                GD.Print("Attacking...");
                NormalAttack();
                //Sprite.Play("Attack");
                break;
            case StoredAction.Ability:
                GD.Print("Using Ability...");
                ActivateAbility();
                break;
            case StoredAction.Defend:
                GD.Print("Defending...");
                break;
            case StoredAction.Wait:
                GD.Print("Waiting...");
                break;

        }

        PlayerController.Instance.ExecuteNextAction();
        
    }

    public void OnPossession()
    {
        CharacterState = CurrentState.Moving;
    }



    //Forgo doing an action in exchange for doubling movement range
    public void Dash()
    {
        GD.Print("Player Beginning Dash!");
        IsDashing = true;
    }



    public void HideProjection()
    {
        
        Projection.Hide();
        Projection.Position = Vector2.Zero;
    }

    public void Aim()
    {

    }

    public void TakeDamage(int Damage)
    {
        
        if (IsTired)
        {
            Damage = (int)(Damage * 2.0f);
        }
        GD.Print(Name + " Taking Damage: " + Damage);
        CurrentHealth -= Damage;
        GD.Print(Name + " Current Health: " + CurrentHealth);
    }

    public void Heal(int Amount) 
    {
        CurrentHealth += Amount;
    }

    public virtual int CalculateNormalAttackDamage()
    {
        return BaseData.GetAttack();
    }

    public virtual int CalculateDamageTaken(int Power, DamageType type, ElementType element, PhysicalType physical)
    {
        if (type == DamageType.Physical)
        {
            Power -= BaseData.GetPhysDefense();
        }
        else if (type == DamageType.Magic)
        {
            Power -= BaseData.GetMagDefense();
        }

        if (element != ElementType.None)
        {
            if (BaseData.GetElemResistance() == element)
            {
                Power = (int)(Power * DamageData.GetResistance());
            }

            if (BaseData.GetElemWeakness() == element)
            {
                Power = (int)(Power * DamageData.GetWeakness());
            }
        }
        if (physical != PhysicalType.None) {
            if (BaseData.GetPhysResistance() == physical)
            {
                Power = (int)(Power * DamageData.GetResistance());
            }

            if (BaseData.GetPhysWeakness() == physical)
            {
                Power = (int)(Power * DamageData.GetWeakness());
            }
        }


        if (CharacterAction == StoredAction.Defend)
        {
            Power = (int)(Power * DamageData.GetDefend());
        }

        return Power;
    }

    public virtual int CalculateAbilityDamage(DamageType type)
    {
        int Damage = 0;
        switch (type)
        {
            case DamageType.Physical:
                Damage = BaseData.GetAttack();
                break;
            case DamageType.Magic:
                Damage = BaseData.GetMagic();
                break;

        }
        return Damage;
    }

    public virtual void NormalAttack()
    {
        int Damage = CalculateNormalAttackDamage();
        GD.Print("Calculated Normal Attack Damage:" + Damage);
        Damage = StoredTarget.CalculateDamageTaken(Damage, DamageType.Physical, ElementType.None, PhysicalType.Cut);
        GD.Print("Calculated Final Attack Damage:" + Damage);
        StoredTarget.TakeDamage(Damage);
    }

    //The user must be the one to decide this, so that they can distinguish between friend and foe when necessary
    //And so they can pass in the desired position, since the ability doesn't know what that is
    public void ActivateAbility()
    {
        if (StoredAbility.GetAimingType() == SelectionType.Target)
        {
            StoredAbility.OnActivate_Single(StoredTarget, this);
        }
        else if (StoredAbility.GetAimingType() == SelectionType.Position)
        {
            StoredAbility.OnActivate_Multiple(
                CombatManager.Instance.GetCharactersInRange(StoredTargetPos, GetAbilityRange(), GetAbilityTargetType(), IsEnemy), 
                this);
            //StoredAbility.OnActivate_Range(CombatManager.Instance.GetEnemiesWithinRange(StoredTargetPos, StoredAbility.GetAbilityRange()), this);
        }
        
    }

    //Get sent in a direction
    public void Shoved(Vector2 Direction)
    {

    }

    //Strike and enemy that's been sent your way
    public void Counter(Character Enemy)
    {

    }




    #region Setters

    public void SetTargetArrowVisibility(bool vis)
    {  
        if (vis)
        {
            TargetIndicator.Show();
        }
        else
        {
            TargetIndicator.Hide();
        }

    }

    public void SetCurrentState(CurrentState state)
    {
        CharacterState = state;
    }

    public void SetStoredAction(StoredAction action)
    {
        CharacterAction = action;
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
        //Play Animation
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

    public void SetMoveDir(Vector2 dir)
    {
        MoveDir = dir;
    }

    public void SetIsMoving(bool isMoving)
    {
        IsMoving = isMoving;
    }


    #endregion

    #region Undoers
    public void CancelPossession()
    {
        CharacterState = CurrentState.Idling;
        Position = StartingLocation;
    }


    //Reset position to starting position
    public void CancelDash()
    {
        IsDashing = false;
        Position = StartingLocation;
    }



    public void CancelTargeting() //If we're selecting a target
    {
        CharacterAction = StoredAction.None;
        CharacterState = CurrentState.SelectingAction;
        StoredAbility = null;
    }


    public void CancelMenu() //If we're selecting an action to take
    {

        CharacterState = CurrentState.Moving;
    }

    #endregion

    #region Getters


    public virtual float GetMoveRange()
    {
        float range = BaseData.GetMoveRange();


        if (IsDashing)
        {
            range *= 2.0f;
        }
        if (IsTired)
        {
            range *= 0.5f;
        }
        return range;

    }



    public virtual float GetOffensiveRange()
    {
        GD.Print("Getting Default Range!");
        if (CharacterAction == StoredAction.Attack)
        {
            return BaseData.GetAttackRange();
        }
        else if (CharacterAction == StoredAction.Ability)
        {
            if (StoredAbility == null)
            {
                GD.Print("NO ABILITY SET");
                return 0;
            }

            return BaseData.GetAttackRange() * StoredAbility.GetAbilityRange();
        }
        return 0f;
        
    }

    public virtual float GetAbilityRange()
    {
        return StoredAbility.GetAbilityRange();
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
        return CharacterAction;
    }

    public bool GetIsDashing()
    {
        return IsDashing;
    }

    public Ability[] GetAbilities() { 
        return BaseData.GetAbilities();
    }

    public TargetType GetAbilityTargetType()
    {
        return StoredAbility.GetTargetingType();
    }



    #endregion

}
