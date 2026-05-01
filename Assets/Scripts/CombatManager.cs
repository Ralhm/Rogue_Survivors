using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Drawing;
using System.Linq;


//This should handle turnflow
public partial class CombatManager : Node2D
{
    private static CombatManager _instance;

    public static CombatManager Instance { get { return _instance; } }

    //Let this contain a list of characters that are within range of an action
    //Strictly for player previewing purposes
    private List<Character> TargetList = new List<Character>();

    //Let this contain a list of projections
    private List<Ally> ProjectionList = new List<Ally>();

    public Character CurrentlyTargetedCharacter;

    public delegate void BeginPhase();

    public BeginPhase BeginAllyPhaseCall;

    public BeginPhase BeginEnemyPhaseCall;

    [Export]
    public Ally[] AllyList;

    [Export]
    public Enemy[] EnemyList;

    [Export]
    public Node2D PreviewArrow;

    [Export]
    RangeIndicatorContainer Indices;

    [Export]
    RangeCollider RangeArea;

    //Hold a number of characters that are currently experiencing a force.
    //As long as this number is greater than 0, do NOT progress to the next action
    //In other words, wait until all characters have stopped moving to begin next action
    [Export]
    public int ShovedCount;

    [Export]
    public int CurrentlySelectedTargetIndex = 0;

    int CurrentActionIndex;

    bool AllyPhase = true;

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


        for (int i = 0; i < EnemyList.Length; i++)
        {
            BeginEnemyPhaseCall += EnemyList[i].OnEndOfPhase;
            BeginEnemyPhaseCall += EnemyList[i].OnBeginningOfPhase;
        }

        for (int i = 0; i < AllyList.Length; i++)
        {
            BeginAllyPhaseCall += AllyList[i].OnEndOfPhase;
            BeginAllyPhaseCall += AllyList[i].OnBeginningOfPhase;
        }
    }



    public void ExecuteNextAction()
    {
        if (AllyPhase)
        {
            CurrentActionIndex++;
            if (CurrentActionIndex >= PlayerController.Instance.AllyActionOrder.Count)
            {
                BeginEnemyPhase();
                return;
            }
            PlayerController.Instance.AllyActionOrder[CurrentActionIndex].BeginAction();
        }      
        else
        {
            CurrentActionIndex++;
            if (CurrentActionIndex >= EnemyList.Length)
            {
                PlayerController.Instance.BeginAllyPhase();
                return;
            }
            EnemyList[CurrentActionIndex].BeginAction();
        }

    }

    public void OnNewTurn()
    {
        Indices.Position = Vector2.Zero;
    }

    public void BeginEnemyPhase()
    {
        GD.Print("Beginning Enemy Phase!");
        CurrentActionIndex = 0;
        AllyPhase = false;
        BeginEnemyPhaseCall?.Invoke();
        EnemyList[0].BeginAction();

    }

    public void BeginAllyPhase()
    {
        CurrentActionIndex = 0;
        AllyPhase = true;
        BeginAllyPhaseCall?.Invoke();
        SetRangeVisible(true);
        OnNewTurn();
    }



    public void AdjustShoveCount(int increment)
    {
        ShovedCount += increment;
        if (ShovedCount == 0)
        {
            //NEXT ACTION HERE
        }
    }


    public void HideCurrentTarget()
    {
        if (CurrentlyTargetedCharacter != null)
        {
            CurrentlyTargetedCharacter.SetTargetArrowVisibility(false);
        }
        else
        {
            GD.Print("TARGET CHARACTER IS NULL");
        }
        
    }

    public void AddTarget(Character character)
    {
        character.SetTargetArrowVisibility(true);
        TargetList.Add(character);
    }

    public void AddTarget_Projection(Ally character)
    {
        character.SetProjectionArrowVisibility(true);
        ProjectionList.Add(character);
    }


    public void RemoveTarget(Character character)
    {
        character.SetTargetArrowVisibility(false);
        TargetList.Add(character);
    }

    public void RemoveTarget_Projection(Ally character)
    {
        character.SetProjectionArrowVisibility(false);
        ProjectionList.Add(character);
    }


    public Character GetCurrentTarget()
    {
        return CurrentlyTargetedCharacter;
    }

    public void NullifyTargetedCharacter()
    {
        CurrentlyTargetedCharacter = null;
    }

    public void HideTargets()
    {
        RangeArea.Monitoring = false;
        Indices.Position = Vector2.Zero;
        if (TargetList.Count > 0)
        {
            for (int i = 0; i < TargetList.Count; i++)
            {
                TargetList[i].SetTargetArrowVisibility(false);
            }
            TargetList.Clear();
        }



        if (ProjectionList.Count > 0) {
            for (int i = 0; i < ProjectionList.Count; i++)
            {
                ProjectionList[i].SetProjectionArrowVisibility(false);
            }
            ProjectionList.Clear();
        }

        
    }

    public Ally[] GetAllies()
    {
        return AllyList;
    }


    public Enemy[] GetEnemies()
    {
        return EnemyList;
    }

    public void SetRangePos()
    {
        RangeArea.Position = GetGlobalMousePosition();
        Indices.Position = GetGlobalMousePosition();
    }

    public bool HasViableTarget()
    {
        if (CurrentlyTargetedCharacter == null)
        {
            return false;
        }
        return true;
    }

    public void DisplayPreviewArrow(ShoveType type, Vector2 Target, Vector2 Origin)
    {

        Vector2 Dir = CalculateDirection(type, Target, Origin);
        PreviewArrow.Show();
        PreviewArrow.Rotation = Dir.Angle();
        PreviewArrow.GlobalPosition = Target + (Dir * 100);

    }

    public void HidePreviewArrow()
    {
        PreviewArrow.Hide();
    }

    public static Vector2 CalculateDirection(ShoveType type, Vector2 Target, Vector2 Origin)
    {
        
        Vector2 Dir = Vector2.Zero;

        switch (type)
        {
            case ShoveType.Back:
                Dir = (Target - Origin).Normalized();
                break;
            case ShoveType.Right:
                Dir = (Target - Origin).Normalized();
                Vector3 FaceDirRight = new Vector3(Dir.X, Dir.Y, 0);
                Vector3 CrossRight = FaceDirRight.Cross(Vector3.Forward).Normalized();
                Dir = (new Vector2(CrossRight.X, CrossRight.Y));
                break;
            case ShoveType.Left:
                Dir = (Target - Origin).Normalized();
                Vector3 FaceDirLeft = new Vector3(Dir.X, Dir.Y, 0);
                Vector3 CrossLeft = FaceDirLeft.Cross(Vector3.Back).Normalized();
                Dir = (new Vector2(CrossLeft.X, CrossLeft.Y));
                break;
            case ShoveType.Pull:
                Dir = (Origin - Target).Normalized();
                break;

        }

        return Dir;
    }

    //RANGE CHECKING
    #region RangeChecking

    public List<Character> GetCharactersInRange(Vector2 Pos, float Range, TargetType TargetingType, bool isEnemy = false)
    {
       
        if (TargetingType == TargetType.Indiscriminate)
        {
            List<Character> result = new List<Character>();
            result = GetAlliesInRange(Pos, Range);
            result.AddRange(GetEnemiesInRange(Pos, Range));
            return result;
        }

        if (isEnemy)
        {
            if (TargetingType == TargetType.Friendly)
            {
                return GetEnemiesInRange(Pos, Range);
            }
            else if (TargetingType == TargetType.NotFriendly)
            {
                return GetAlliesInRange(Pos, Range);
            }
        }
        else
        {
            if (TargetingType == TargetType.Friendly)
            {
                return GetAlliesInRange(Pos, Range);
            }
            else if (TargetingType == TargetType.NotFriendly)
            {
                return GetEnemiesInRange(Pos, Range);
            }
        }
        GD.Print("FAILED TO RETURN TARGET ARRAY");
        return null;
    }

    public List<Character> GetEnemiesInRange(Vector2 Pos, float Range)
    {
        List<Character> result = new List<Character>();
        for (int i = 0; i < EnemyList.Length; i++)
        {
            if (CheckTargetInRange(EnemyList[i].Position, Range, Pos))
            {
                result.Add(EnemyList[i]);
            }

        }
        return result;
    }

    public List<Character> GetAlliesInRange(Vector2 Pos, float Range)
    {
        List<Character> result = new List<Character>();
        for (int i = 0; i < AllyList.Length; i++)
        {
            if (CheckTargetInRange(AllyList[i].Position, Range, Pos))
            {
                result.Add(AllyList[i]);
            }

        }
        return result;
    }

    public bool CheckTargetInRange(Vector2 targetPos, float Range, Vector2 Position)
    {
        //float dist = (character.Position - Position).Length();
        //GD.Print("Distance: " + dist + " Range: " + Range);
        if ((targetPos - Position).Length() <= Range)
        {
            return true;

        }

        return false;
    }

    #endregion

    //RANGE INDICATION
    #region RangeIndication

    //Specifically for Positional type abilities
    public void DisplayTargetRange(TargetType type, float Range)
    {
        RangeArea.Monitoring = true;
        if (type == TargetType.Indiscriminate)
        {
            CheckForAll();
        }
        else if (type == TargetType.Friendly)
        {
            CheckForAllies();
        }
        else if (type == TargetType.NotFriendly)
        {
            CheckForEnemies();
        }
        else if (type == TargetType.Self)
        {
            GD.Print("Can only target Self!");
            return;
        }

        SetRangeIndices(Range, Vector2.Zero);

    }

    public void SetRangeVisible(bool visible)
    {
        if (visible)
        {
            Indices.Show();
        }
        else
        {
            Indices.Hide();
        }

    }
    public void SetRangeIndices(float Range, Vector2 Position)
    {
        Indices.SetRangeIndices(Range, Position);
    }


    public void CheckForAllies()
    {
        RangeArea.SetCheckForAllies(true);
        RangeArea.SetCheckForEnemies(false);
    }

    public void CheckForEnemies()
    {
        RangeArea.SetCheckForAllies(false);
        RangeArea.SetCheckForEnemies(true);
    }

    public void CheckForAll()
    {
        RangeArea.SetCheckForAllies(true);
        RangeArea.SetCheckForEnemies(true);
    }


    #endregion

    //SINGLE TARGET SELECTION SPECIFICALLY FOR PLAYER UI PURPOSES
    #region SingleTargetSelection

    public void SelectEnemyInRange(int increment, float Range, Vector2 Position)
    {
        GD.Print("Attack Range:" + Range);
        GD.Print("Enemy Count:" + EnemyList.Length);
        int i = 0;
        while (i < EnemyList.Length)
        {
            EnemyList[CurrentlySelectedTargetIndex].SetTargetArrowVisibility(false);
            CurrentlySelectedTargetIndex += increment;
            if (CurrentlySelectedTargetIndex < 0)
            {
                CurrentlySelectedTargetIndex = EnemyList.Length - 1;
            }
            else if (CurrentlySelectedTargetIndex >= EnemyList.Length)
            {
                CurrentlySelectedTargetIndex = 0;
            }

            GD.Print("Current Target Index: " + CurrentlySelectedTargetIndex);
            if (EnemyList[CurrentlySelectedTargetIndex] != null)
            {

                if (CheckTargetInRange(EnemyList[CurrentlySelectedTargetIndex].Position, Range, Position))
                {
                    GD.Print("Found Target!");
                    CurrentlyTargetedCharacter = EnemyList[CurrentlySelectedTargetIndex];
                    CurrentlyTargetedCharacter.SetTargetArrowVisibility(true);
                    return;
                }
            }
            i++;
        }
        CurrentlyTargetedCharacter = null;
        GD.Print("NO ENEMIES IN RANGE");
    }

    public void SelectAllyInRange(int increment, float Range, Vector2 Position)
    {
        int i = 0;
        while (i < AllyList.Length)
        {
            AllyList[CurrentlySelectedTargetIndex].SetTargetArrowVisibility(false);
            CurrentlySelectedTargetIndex += increment;
            if (CurrentlySelectedTargetIndex < 0)
            {
                CurrentlySelectedTargetIndex = AllyList.Length - 1;
            }
            else if (CurrentlySelectedTargetIndex >= AllyList.Length)
            {
                CurrentlySelectedTargetIndex = 0;
            }

            if (AllyList[CurrentlySelectedTargetIndex].GetCurrentState() == CurrentState.Dead)
            {
                i++;
                continue;
            }

            if (AllyList[CurrentlySelectedTargetIndex] != null)
            {
                if (CheckTargetInRange(AllyList[CurrentlySelectedTargetIndex].Position, Range, Position)) //Check if their default position is in range
                {
                    CurrentlyTargetedCharacter = AllyList[CurrentlySelectedTargetIndex];
                    AllyList[CurrentlySelectedTargetIndex].SetTargetArrowVisibility(true);
                    return;
                }
                if (CheckTargetInRange(AllyList[CurrentlySelectedTargetIndex].GetProjectionPos(), Range, Position)) //Check if their projection is in range
                {
                    GD.Print("Projection is within range!");
                    CurrentlyTargetedCharacter = AllyList[CurrentlySelectedTargetIndex];
                    AllyList[CurrentlySelectedTargetIndex].SetTargetArrowVisibility(true);
                    return;
                }
            }

            i++;
        }

        CurrentlyTargetedCharacter = null;
        GD.Print("NO ALLIES IN RANGE");

    }
    #endregion
}
