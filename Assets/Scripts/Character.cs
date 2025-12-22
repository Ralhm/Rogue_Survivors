using Godot;
using System;


public enum CurrentState
{
    Idling,
    Moving,
    Dashing,
    Attacking,
    Acted,
    Dead
}


public partial class Character : CharacterBody2D
{

    public AnimatedSprite2D Sprite;

    [Export]
    public CurrentState CurrentState;

    [Export]
    public int CurrentHealth;

    [Export]
    public int Speed = 450;

    [Export]
    public CharacterData BaseData;

    [Export]
    public CharacterUpgradeContainer UpgradeData;

    //The position this Character has started their movement from
    //Use to compare distance travelled and prevent from moving out of range
    [Export]
    private Vector2 StartingLocation;

    [Export]
    private float DistanceTraveled;

    [Export]
    public bool IsMoving;

    [Export]
    public Vector2 MoveDir;

    private string StoredAnimDir = "Front";

    public override void _Ready()
    {
        base._Ready();
        StartingLocation = Position;
        Sprite = GetNode<Godot.AnimatedSprite2D>("Sprite");
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





        if (MoveDir != Vector2.Zero)
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
        CurrentState = CurrentState.Idling;
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
        CurrentState = CurrentState.Moving;
    }


    public void CancelPossession()
    {
        CurrentState = CurrentState.Idling;
        Position = StartingLocation;
    }

    public void FinishAction()
    {
        CurrentState = CurrentState.Acted;
    }

    //Forgo doing an action in exchange for doubling movement range
    public void Dash()
    {

    }

    //Reset position to starting position
    public void CancelDash()
    {
        CurrentState = CurrentState.Moving;
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


    //Get sent in a direction
    public void Shoved(Vector2 Direction)
    {

    }

    //Strike and enemy that's been sent your way
    public void Counter(Character Enemy)
    {

    }

}
