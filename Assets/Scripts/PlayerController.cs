using Godot;
using System;
using System.Collections.Generic;


//TO DO LIST
//UI
//Attacking
//Abilities
//Enemy Behavior
//Shoving
//Countering when shoved into
//Dashing
//If you act right after dashing, become tired
//When tired, movement range is halved, damage and defense are slightly reduced. Can still defend
//Cancelling actions
//Storing action order
//Visual indicator on movement range
//When moving, instead of moving the character, move a silhouette/ghost of the character, which has its own collission (ignores owners collision)
//When moving, create a path for the real body to follow on execute comprised of several same length lines (make it an array)
//The path should be comprised of several points in space. If you get far enough away from the most recent point, create a new point
//If you get to close to a previous point, remove the most recent point/points that come after it
//This will mean doing frequent distance checks. We can minimize number of checks that must be done by making point range longer
//This way we don't need real pathfinding


//Add an input, like shoulder buttons or something, for swapping between which character is currently acting
public enum CurrentPlayerState
{
    Moving,
    SelectingAction,
    SelectingTarget, //For when attack or ability has been chosen, and the player needs to select a target
    WaitingForAction,
    Inactive //Set Inactive during Enemy Turn
}

public enum SelectionType
{
    Target,
    Position,
    Direction
}

//Script for handling various non-entity related things
//Will contain a reference to the currently possessed character for the player to control/move
public partial class PlayerController : Node
{


    private static PlayerController _instance;

    public static PlayerController Instance { get { return _instance; } }

    [Export]
    CurrentPlayerState PlayerState;

    [Export]
    SelectionType CurrentSelectionType;

    public int CurrentCharacterIndex = 0;

    [Export]
    private Character[] Allies;

    public List<Character> AllyActionOrder = new List<Character>(); //Let this represent the order of actions for allies that play upon execution
    public int CurrentActionIndex;

    [Export]
    Control ActionMenu;

    [Export]
    Control AbilityMenu;

    [Export]
    Control ExecutionMenu;

    //True if targeting enemy, false if targeting ally
    bool TargetingEnemy;

    private bool AskingForExecution = false;

    public override void _Ready()
    {
        base._Ready();
        


        if (_instance != null && _instance != this)
        {
            QueueFree();
        }
        else
        {
            _instance = this;
        }
        PossessCharacter();
        ActionMenu.Hide();

        for (int i = 0; i < Allies.Length; i++) {
            Allies[i].CharacterIndex = i;
        }
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        Allies[CurrentCharacterIndex].SetMoveDir(new Vector2(Input.GetAxis("MoveLeft", "MoveRight"), Input.GetAxis("MoveUp", "MoveDown")));
        if (Input.IsActionJustPressed("SelectUp"))
        {
            if (PlayerState == CurrentPlayerState.SelectingTarget && CurrentSelectionType == SelectionType.Target)
            {
                ChangeSelectedTarget(1);
            }
            else
            {
                UnPossessCharacter();
                SwapCurrentCharacter(1);
            }
                
        }

        if (Input.IsActionJustPressed("SelectDown"))
        {
            if (PlayerState == CurrentPlayerState.SelectingTarget && CurrentSelectionType == SelectionType.Target)
            {
                ChangeSelectedTarget(-1);
            }
            else
            {
                UnPossessCharacter();
                SwapCurrentCharacter(-1);
            }
        }

        if (Input.IsActionJustPressed("Confirm"))
        {
            if (AskingForExecution)
            {

            }
            else if (PlayerState == CurrentPlayerState.Moving)
            {
                BeginSelectAction();
            }
            else if (PlayerState == CurrentPlayerState.SelectingAction)
            {

            }
            else if (PlayerState == CurrentPlayerState.SelectingTarget)
            {
                
                ConfirmTarget();


            }
            
        }

        if (Input.IsActionJustPressed("Undo"))
        {
            UndoCommand();
        }
    }

    public void ChangeSelectedTarget(int increment)
    {
        if (TargetingEnemy)
        {
            CombatManager.Instance.SelectEnemyInRange(increment, Allies[CurrentCharacterIndex].GetOffensiveRange(), Allies[CurrentCharacterIndex].Position);

        }
        else
        {
            CombatManager.Instance.SelectAllyInRange(increment, Allies[CurrentCharacterIndex].GetOffensiveRange(), Allies[CurrentCharacterIndex].Position);
        }
    }


    public void ConfirmTarget()
    {
        GD.Print("Confirming Target!");
        if (CurrentSelectionType == SelectionType.Target)
        {
            if (CombatManager.Instance.HasViableTarget())
            {
                RegisterTarget();
            }
            
        }
        else if (CurrentSelectionType == SelectionType.Position) { 
            
        
        }
        else if (CurrentSelectionType == SelectionType.Direction)
        {

        }
    }

    public void BeginExecuteTurn()
    {

    }

    public void ExecuteNextAction()
    {

    }

    public void SetPlayerState(CurrentPlayerState state)
    {
        PlayerState = state;
    }

    public void BeginNewTurn()
    {
        AllyActionOrder.Clear();
        for (int i = 0; i < Allies.Length; i++) {
            Allies[i].OnNewTurn();
        }
    }

    public void PossessCharacter()
    {
        Allies[CurrentCharacterIndex].OnPossession();
        CombatManager.Instance.SetRangeIndices(Allies[CurrentCharacterIndex].GetMoveRange(), Allies[CurrentCharacterIndex].StartingLocation);

    }


    public void UnPossessCharacter()
    {
        Allies[CurrentCharacterIndex].CancelPossession();
    }

    public void SwapCurrentCharacter(int val)
    {
        for (int i = 0; i < Allies.Length; i++) {
            CurrentCharacterIndex += val;
            if (CurrentCharacterIndex >= Allies.Length)
            {
                CurrentCharacterIndex = 0;
            }
            else if (CurrentCharacterIndex < 0)
            {
                CurrentCharacterIndex = Allies.Length - 1;
            }
            if (Allies[CurrentCharacterIndex].GetCurrentState() != CurrentState.ActionSet)
            {
                break;
            }

        }
        PossessCharacter();

    }


    public void ShowAbilityMenu()
    {
        AbilityMenu.Show();
    }

    public void BeginSelectAction()
    {
        ActionMenu.Show();
        Allies[CurrentCharacterIndex].SetCurrentState(CurrentState.SelectingAction);
        PlayerState = CurrentPlayerState.SelectingAction;
    }

    public void UndoCommand()
    {

        if (PlayerState == CurrentPlayerState.SelectingAction)
        {
            GD.Print("Undoing Menu!");
            if (AskingForExecution)
            {
                GD.Print("Asking for Execution!");
                AskingForExecution = false;
                ExecutionMenu.Hide();
                AllyActionOrder.Remove(Allies[CurrentCharacterIndex]);
                Allies[CurrentCharacterIndex].CancelAction();

            }
            else
            {
                Allies[CurrentCharacterIndex].CancelMenu();
            }


            PlayerState = CurrentPlayerState.Moving;
            ActionMenu.Hide();
            


            

        }
        else if ((PlayerState == CurrentPlayerState.Moving) && (AllyActionOrder.Count > 0)) //Undo a set command if one has been set
        {
            int index = AllyActionOrder[AllyActionOrder.Count - 1].CharacterIndex;
            AllyActionOrder.RemoveAt(AllyActionOrder.Count - 1);
            Allies[index].CancelAction();
            UnPossessCharacter();
            CurrentCharacterIndex = index;
            PossessCharacter();
        }
        else if (PlayerState == CurrentPlayerState.SelectingTarget)
        {
            GD.Print("Undoing Target Selection!");


            if (Allies[CurrentCharacterIndex].GetStoredActionType() == StoredAction.Attack)
            {
                ActionMenu.Show();
            }
            else if (Allies[CurrentCharacterIndex].GetStoredActionType() == StoredAction.Ability)
            {
                AbilityMenu.Show();
            }

            if (Allies[CurrentCharacterIndex].GetIsDashing())
            {
                CombatManager.Instance.SetRangeIndices(Allies[CurrentCharacterIndex].GetDashRange(), Allies[CurrentCharacterIndex].StartingLocation);
            }
            else
            {
                CombatManager.Instance.SetRangeIndices(Allies[CurrentCharacterIndex].GetMoveRange(), Allies[CurrentCharacterIndex].StartingLocation);
            }
            if (AskingForExecution)
            {
                AskingForExecution = false;
                ExecutionMenu.Hide();
                AllyActionOrder.Remove(Allies[CurrentCharacterIndex]);
                Allies[CurrentCharacterIndex].CancelAction();
                Allies[CurrentCharacterIndex].SetCurrentState(CurrentState.SelectingAction);
            }
            else
            {
                Allies[CurrentCharacterIndex].CancelTargeting();
            }

            PlayerState = CurrentPlayerState.SelectingAction;
            
            CombatManager.Instance.HideCursor();
        }


        
    }

    public void OnAbilityButtonPressed(Ability ability)
    {
        TargetingEnemy = !ability.GetIsFriendly();
        ActionMenu.Hide();
        SetSelectionType(ability.GetAimingType());
        Allies[CurrentCharacterIndex].SetStoredAbility(ability);
        Allies[CurrentCharacterIndex].SetStoredAction(StoredAction.Ability);

    }
    public void SetSelectionType(SelectionType type)
    {
        PlayerState = CurrentPlayerState.SelectingTarget;
        CurrentSelectionType = type;
        if (CurrentSelectionType == SelectionType.Target)
        {
            CombatManager.Instance.SetRangeIndices(Allies[CurrentCharacterIndex].GetOffensiveRange(), Allies[CurrentCharacterIndex].Position);
            ChangeSelectedTarget(1);
        }
    }

    public void BeginDash()
    {
        Allies[CurrentCharacterIndex].Dash();
        CombatManager.Instance.SetRangeIndices(Allies[CurrentCharacterIndex].GetDashRange(), Allies[CurrentCharacterIndex].StartingLocation);
    }


    public void OnButtonPress(StoredAction action)
    {
        ActionMenu.Hide();
        Allies[CurrentCharacterIndex].SetStoredAction(action);
        switch (action) {
            case StoredAction.Attack:
                TargetingEnemy = true;
                SetSelectionType(SelectionType.Target);              
                break;
            case StoredAction.Wait:
                FinalizeCommand();
                break;
            case StoredAction.Defend:
                FinalizeCommand();
                break;

        }
    }

    #region CommandSetters

    public void FinalizeCommand()
    {
        GD.Print("Finalizing Command!");
        ActionMenu.Hide();
        Allies[CurrentCharacterIndex].SetActionSet();
        AllyActionOrder.Add(Allies[CurrentCharacterIndex]);
        CombatManager.Instance.HideCursor();
        if (AllyActionOrder.Count < 3)
        {
            PlayerState = CurrentPlayerState.Moving;
            SwapCurrentCharacter(1);
        }
        else
        {
            //Ask player if they want to execute here
            GD.Print("Execute?");
            ExecutionMenu.Show();
            AskingForExecution = true;
        }

    }



    public void RegisterTarget()
    {
        switch (CurrentSelectionType)
        {
            case SelectionType.Target:
                Allies[CurrentCharacterIndex].SetStoredTarget(CombatManager.Instance.GetCurrentTarget());
                CombatManager.Instance.NullifyTargetedCharacter();
                break;
            case SelectionType.Position:
                //Allies[CurrentCharacterIndex].SetStoredTargetPos(CombatManager.Instance.GetTargetPosition());
                break;
            case SelectionType.Direction:
                //Allies[CurrentCharacterIndex].SetStoredDir(Vector2.Zero);
                break;
        }
        FinalizeCommand();
    }


    #endregion

}
