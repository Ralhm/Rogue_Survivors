using Godot;
using System;
using System.Collections.Generic;


//TO DO LIST
//Cooler UI
//Ability Descriptions
//Enemy Behavior
//Go for a more complex decision making process
//Perhaps make a decision class that keeps track of the target/targets and ability/action
//And give that decision class a heuristic
//Don't go nuts, we don't need the enemies to be too crazy smart
//Cycle through all potential decisions and pick one with the best heuristic
//And each enemy class should have its own heuristic bias, like supports should prioritize using support abilities
//Remove Projection for enemies
//Replace the range indices with a proper range circle
//Check if enemy is still in range upon finishing movement and attempting to execute
//Preview arrow for directional shove abilities
//Move Projection stuff out of character and into ally
//Damage indicators, like number particles and character shaking 
//DOn't forget to wait on when a character is shoved to stop moving for the next action to activate
//Ailments
//Consider making normal attacks a form of ability

public enum CurrentPlayerState
{
    Moving,
    SelectingAction,
    SelectingAbility,
    SelectingTarget, //For when attack or ability has been chosen, and the player needs to select a target
    WaitingForAction,
    Inactive //Set Inactive during Enemy Turn
}


//Script for handling various non-entity related things
//Will contain a reference to the currently possessed character for the player to control/move
public partial class PlayerController : Node2D
{


    private static PlayerController _instance;

    public static PlayerController Instance { get { return _instance; } }




    [Export]
    CurrentPlayerState PlayerState;

    [Export]
    SelectionType CurrentSelectionType;

    public int CurrentCharacterIndex = 0;

    private Ally CurrentAlly;

    [Export]
    private Ally[] Allies;

    public List<Character> AllyActionOrder = new List<Character>(); //Let this represent the order of actions for allies that play upon execution
    public int CurrentActionIndex;

    [Export]
    Control ActionMenu;

    [Export]
    AbilityMenu AbilitiesMenu;

    [Export]
    Control ExecutionMenu;

    TargetType CurrentTargetType;

    private bool AskingForExecution = false;



    public override void _Ready()
    {
        base._Ready();
        GD.Randomize();
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

        if (PlayerState == CurrentPlayerState.Moving)
        {
            CurrentAlly.SetMoveDir(new Vector2(Input.GetAxis("MoveLeft", "MoveRight"), Input.GetAxis("MoveUp", "MoveDown")));
        }

        if (PlayerState == CurrentPlayerState.SelectingTarget && CurrentSelectionType == SelectionType.Position)
        {
            CombatManager.Instance.SetRangePos();
        }

        
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
                BeginExecuteTurn();
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

        if (Input.IsActionJustPressed("Dash"))
        {
            BeginDash();
        }

        if (Input.IsActionJustPressed("Undo"))
        {
            UndoCommand();
        }
    }

    public void ChangeSelectedTarget(int increment)
    {
        if (CurrentTargetType == TargetType.NotFriendly)
        {
            CombatManager.Instance.SelectEnemyInRange(increment, CurrentAlly.GetStoredOffensiveRange(), CurrentAlly.Position);

        }
        else if (CurrentTargetType == TargetType.Friendly)
        {
            CombatManager.Instance.SelectAllyInRange(increment, CurrentAlly.GetStoredOffensiveRange(), CurrentAlly.Position);
        }

        //If we find enough stuff gets added here, we'll move it to an allies 'OnSwitchTarget' function or some such
        if (CurrentAlly.GetStoredActionType() == StoredAction.Ability)
        {
            if (CurrentAlly.GetShoveType() != ShoveType.None)
            {
                CombatManager.Instance.DisplayPreviewArrow(CurrentAlly.GetShoveType(), CombatManager.Instance.GetCurrentTarget().Position, CurrentAlly.Position);

            }
        }
    }

    public void ConfirmTarget()
    {
        GD.Print("Confirming Target!");
        if (CurrentSelectionType == SelectionType.Target && CombatManager.Instance.HasViableTarget())
        {
            RegisterTarget();

        }
        else
        {
            RegisterTarget();
        }
    }

    public void BeginExecuteTurn()
    {
        ExecutionMenu.Hide();
        PlayerState = CurrentPlayerState.Inactive;
        for (int i = 0; i < AllyActionOrder.Count; i++) {
            Allies[i].HideProjection();
            Allies[i].HideOrderIcon();
        }
        AllyActionOrder[0].BeginNavigation();
    }


    public void SetPlayerState(CurrentPlayerState state)
    {
        PlayerState = state;
    }

    public void BeginAllyPhase()
    {
        AllyActionOrder.Clear();
        AskingForExecution = false;
        CurrentCharacterIndex = 0;
        PlayerState = CurrentPlayerState.Moving;
        CombatManager.Instance.BeginAllyPhase();
        PossessCharacter();
    }

    

    public void PossessCharacter()
    {
        CurrentAlly = Allies[CurrentCharacterIndex];
        CurrentAlly.OnPossession();
        CombatManager.Instance.SetRangeIndices(CurrentAlly.GetMoveRange(), CurrentAlly.StartingLocation);

    }


    public void UnPossessCharacter()
    {
        CurrentAlly.CancelPossession();
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
        PlayerState = CurrentPlayerState.SelectingAbility;
        ActionMenu.Hide();
        AbilitiesMenu.Show();
        AbilitiesMenu.SetAbilities(CurrentAlly.GetAbilities());
    }

    public void BeginSelectAction()
    {
        ActionMenu.Show();
        CurrentAlly.SetCurrentState(CurrentState.SelectingAction);
        PlayerState = CurrentPlayerState.SelectingAction;
    }



    public void SetSelectionType(SelectionType type)
    {
        PlayerState = CurrentPlayerState.SelectingTarget;
        CurrentSelectionType = type;
        if (CurrentSelectionType == SelectionType.Target)
        {
            GD.Print("Setting Target Selection!");
            CombatManager.Instance.SetRangeIndices(CurrentAlly.GetStoredOffensiveRange(), CurrentAlly.Position);
            ChangeSelectedTarget(1);
        }
        else if (CurrentSelectionType == SelectionType.Position) {
            CombatManager.Instance.DisplayTargetRange(CurrentAlly.GetAbilityTargetType(), CurrentAlly.GetStoredAbilityRange());
        }
    }

    public void BeginDash()
    {
        GD.Print("Received Dash Command!");
        CurrentAlly.Dash();
        CombatManager.Instance.SetRangeIndices(CurrentAlly.GetMoveRange(), CurrentAlly.StartingLocation);
    }


    //Specifically for Attack, wait, and defend, 
    //NOT For abilities
    public void OnButtonPress(StoredAction action)
    {
        ActionMenu.Hide();
        CurrentAlly.SetStoredAction(action);
        switch (action) {
            case StoredAction.Attack:
                CurrentTargetType = TargetType.NotFriendly;
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

    //Specifically for when the ability button has been pressed
    public void OnAbilityButtonPressed(Ability ability)
    {
           
        AbilitiesMenu.Hide();
        CurrentAlly.SetStoredAbility(ability);
        CurrentAlly.SetStoredAction(StoredAction.Ability);
        CurrentTargetType = CurrentAlly.GetAbilityTargetType();
        SetSelectionType(ability.GetAimingType());


    }

    //UNDO STUFF
    #region UndoStuff
    public void UndoCommand()
    {
        if (PlayerState == CurrentPlayerState.SelectingAction)
        {
            Undo_ActionMenu();
        }
        else if ((PlayerState == CurrentPlayerState.Moving) && (AllyActionOrder.Count > 0)) //Undo a set command if one has been set
        {
            Undo_ActionSet();
        }
        else if ((PlayerState == CurrentPlayerState.Moving) && (CurrentAlly.GetIsDashing()))
        {
            Undo_Dash();
        }
        else if (PlayerState == CurrentPlayerState.SelectingTarget)
        {
            Undo_TargetSelection();
        }
        else if (PlayerState == CurrentPlayerState.SelectingAbility)
        {
            Undo_AbilitySelect();
        }

    }

    public void Undo_ActionMenu()
    {
        GD.Print("Undoing Menu!");
        if (AskingForExecution)
        {
            GD.Print("Asking for Execution!");
            AskingForExecution = false;
            ExecutionMenu.Hide();
            CombatManager.Instance.SetRangeVisible(true);
            AllyActionOrder.Remove(CurrentAlly);
            CurrentAlly.CancelAction();

        }
        else
        {
            CurrentAlly.CancelMenu();
        }

        PlayerState = CurrentPlayerState.Moving;
        ActionMenu.Hide();
    }

    public void Undo_ActionSet()
    {
        int index = AllyActionOrder[AllyActionOrder.Count - 1].CharacterIndex;
        AllyActionOrder.RemoveAt(AllyActionOrder.Count - 1);
        Allies[index].CancelAction();
        UnPossessCharacter();
        CurrentCharacterIndex = index;
        PossessCharacter();
    }

    public void Undo_TargetSelection()
    {
        GD.Print("Undoing Target Selection!");
        if (CurrentAlly.GetStoredActionType() == StoredAction.Attack)
        {
            ActionMenu.Show();
            PlayerState = CurrentPlayerState.SelectingAction;
        }
        else if (CurrentAlly.GetStoredActionType() == StoredAction.Ability)
        {
            AbilitiesMenu.Show();
            PlayerState = CurrentPlayerState.SelectingAbility;
        }

        CombatManager.Instance.SetRangeIndices(CurrentAlly.GetMoveRange(), CurrentAlly.StartingLocation);
        if (AskingForExecution)
        {
            AskingForExecution = false;
            CombatManager.Instance.SetRangeVisible(true);
            ExecutionMenu.Hide();
            AllyActionOrder.Remove(CurrentAlly);
            CurrentAlly.HideProjection();

        }

        CurrentAlly.CancelTargeting();

        if (CurrentSelectionType == SelectionType.Target)
        {
            CombatManager.Instance.HideCurrentTarget();
        }
        else if (CurrentSelectionType == SelectionType.Position)
        {
            CombatManager.Instance.HideTargets();
        }
        else
        {

        }
        CombatManager.Instance.HidePreviewArrow();
    }

    public void Undo_AbilitySelect()
    {
        AbilitiesMenu.Hide();
        ActionMenu.Show();
        PlayerState = CurrentPlayerState.SelectingAction;
    }

    public void Undo_Dash()
    {
        GD.Print("Cancelling Dash!");
        CurrentAlly.CancelDash();
        CombatManager.Instance.SetRangeIndices(CurrentAlly.GetMoveRange(), CurrentAlly.StartingLocation);
    }

    #endregion



    //COMMAND SETTERS
    #region CommandSetters

    public void FinalizeCommand()
    {
        GD.Print("Finalizing Command!");
        ActionMenu.Hide();
        CurrentAlly.SetActionSet();
        AllyActionOrder.Add(CurrentAlly);
        CurrentAlly.SetOrderIcon(AllyActionOrder.Count);
        CombatManager.Instance.HidePreviewArrow();
        if (AllyActionOrder.Count < 3)
        {
            PlayerState = CurrentPlayerState.Moving;
            SwapCurrentCharacter(1);
        }
        else
        {
            //Ask player if they want to execute here
            GD.Print("Execute?");
            CombatManager.Instance.SetRangeVisible(false);
            ExecutionMenu.Show();
            AskingForExecution = true;
        }

    }



    public void RegisterTarget()
    {
        GD.Print("Registering Target");
        switch (CurrentSelectionType)
        {
            case SelectionType.Target:
                CurrentAlly.SetStoredTarget(CombatManager.Instance.GetCurrentTarget());
                CombatManager.Instance.HideCurrentTarget();
                CombatManager.Instance.NullifyTargetedCharacter();
                break;
            case SelectionType.Position:
                CurrentAlly.SetStoredTargetPos(GetGlobalMousePosition());
                CombatManager.Instance.HideTargets();
                break;
            case SelectionType.Aim:
                //CurrentAlly.SetStoredDir(Vector2.Zero);
                break;
        }
        FinalizeCommand();
    }


    #endregion

}
