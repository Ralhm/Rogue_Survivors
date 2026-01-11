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

    [Export]
    public Ally[] AllyList;

    [Export]
    public Enemy[] EnemyList;

    //Let this contain a list of characters that are within range of an action
    //Strictly for player previewing purposes
    private List<Character> TargetList = new List<Character>();

    //Let this contain a list of projections
    private List<Ally> ProjectionList = new List<Ally>();

    [Export]
    public int CurrentlySelectedTargetIndex = 0;

    public Character CurrentlyTargetedCharacter;

    [Export]
    RangeIndicatorContainer Indices;

    [Export]
    RangeCollider RangeArea;



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
        //Cursor.Hide();
    }

    public void OnNewTurn()
    {
        Indices.Position = Vector2.Zero;
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
            if (CheckTargetInRange(EnemyList[i], Range, Pos))
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
            if (CheckTargetInRange(AllyList[i], Range, Pos))
            {
                result.Add(AllyList[i]);
            }

        }
        return result;
    }

    public bool CheckTargetInRange(Character character, float Range, Vector2 Position)
    {
        if ((character.Position - Position).Length() <= Range)
        {
            return true;

        }
        return false;
    }

    #endregion

    //RANGE INDICATION
    #region RangeIndication


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

    //SINGLE TARGET SELECTION
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

                if (CheckTargetInRange(EnemyList[CurrentlySelectedTargetIndex], Range, Position))
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
                if (CheckTargetInRange(AllyList[CurrentlySelectedTargetIndex], Range, Position)) //Check if their default position is in range
                {
                    CurrentlyTargetedCharacter = AllyList[CurrentlySelectedTargetIndex];
                    AllyList[CurrentlySelectedTargetIndex].SetTargetArrowVisibility(true);
                    return;
                }
                if (AllyList[CurrentlySelectedTargetIndex].GetProjectionIsInRange(Range, Position)) //Check if their projection is in range
                {
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
