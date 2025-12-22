using Godot;
using System;

//Add an input, like shoulder buttons or something, for swapping between which character is currently acting
public enum CurrentPlayerState
{
    Moving,
    SelectingAction,
    SelectingTarget,
    WaitingForAction,
    Inactive //Set Inactive during Enemy Turn
}


//Script for handling various non-entity related things
//Will contain a reference to the currently possessed character for the player to control/move
public partial class PlayerController : Node
{


    public int CurrentCharacterIndex = 0;

    [Export]
    public Character[] Allies;

    public override void _Ready()
    {
        base._Ready();
        PossessCharacter();
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        Allies[CurrentCharacterIndex].SetMoveDir(new Vector2(Input.GetAxis("MoveLeft", "MoveRight"), Input.GetAxis("MoveUp", "MoveDown")));
        if (Input.IsActionJustPressed("SelectUp"))
        {
            SwapCurrentCharacter(1);
        }

        if (Input.IsActionJustPressed("SelectDown"))
        {
            SwapCurrentCharacter(-1);
        }
    }


    public void PossessCharacter()
    {
        Allies[CurrentCharacterIndex].OnPossession();

    }

    public void UnPossessCharacter()
    {
        Allies[CurrentCharacterIndex].CancelPossession();
    }

    public void SwapCurrentCharacter(int val)
    {

        UnPossessCharacter();
        CurrentCharacterIndex += val;
        if (CurrentCharacterIndex >= Allies.Length)
        {
            CurrentCharacterIndex = 0;
        }
        else if (CurrentCharacterIndex < 0)
        {
            CurrentCharacterIndex = Allies.Length - 1;
        }


        PossessCharacter();

    }
}
