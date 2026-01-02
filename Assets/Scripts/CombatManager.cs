using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Drawing;
using System.Linq;

//This should handle turnflow
public partial class CombatManager : Node
{
    private static CombatManager _instance;

    public static CombatManager Instance { get { return _instance; } }

    [Export]
    private Character[] AllyList;

    [Export]
    private Character[] EnemyList;

    [Export]
    public int CurrentlySelectedTargetIndex = 0;

    public Character CurrentlyTargetedCharacter;

    [Export]
    RangeIndicatorContainer Indices;

    [Export]
    Node2D Cursor;

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
        Cursor.Hide();
    }

    public void OnNewTurn()
    {

    }


    public void SelectEnemyInRange(int increment, float Range, Vector2 Position)
    {
        
        int i = 0;
        while (i < EnemyList.Length) {
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
                    SetCursorLocation(EnemyList[CurrentlySelectedTargetIndex].Position);
                    ShowCursor();
                    return;
                }

                
            }
            i++;
        }

        GD.Print("NO ENEMIES IN RANGE");

        
    }


    public void SelectAllyInRange(int increment, float Range, Vector2 Position)
    {
        int i = 0;
        while (i < AllyList.Length) {
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

            if (EnemyList[CurrentlySelectedTargetIndex] != null)
            {
                if (CheckTargetInRange(AllyList[CurrentlySelectedTargetIndex], Range, Position)) //Check if their default position is in range
                {
                    CurrentlyTargetedCharacter = AllyList[CurrentlySelectedTargetIndex];
                    SetCursorLocation(AllyList[CurrentlySelectedTargetIndex].StartingLocation);
                    ShowCursor();
                    return;
                }
                if (AllyList[CurrentlySelectedTargetIndex].GetProjectionIsInRange(Range, Position)) //Check if their 
                {
                    CurrentlyTargetedCharacter = AllyList[CurrentlySelectedTargetIndex];
                    SetCursorLocation(AllyList[CurrentlySelectedTargetIndex].EndLocation);
                    ShowCursor();
                    return;
                }
            }

            i++;
        }

        CurrentlyTargetedCharacter = null;
        GD.Print("NO ALLIES IN RANGE");

    }

    public Character GetCurrentTarget()
    {
        return CurrentlyTargetedCharacter;
    }

    public void NullifyTargetedCharacter()
    {
        CurrentlyTargetedCharacter = null;
    }

    public Vector2 GetTargetPosition()
    {
        return Vector2.Zero;
    }


    public void SetCursorLocation(Vector2 Pos)
    {
        if (Cursor != null) {
            Cursor.Position = Pos;
        }
        
    }

    public void HideCursor()
    {
        Cursor.Hide();
    }

    public void ShowCursor()
    {
        Cursor.Show();
    }

    public bool CheckTargetInRange(Character character, float Range, Vector2 Position)
    {
        if ((character.Position - Position).Length() <= Range)
        {
            return true;

        }
        return false;
    }

    public void SetRangeVisible()
    {
        
    }

    public void SetRangeIndices(float Range, Vector2 Position)
    {
        Indices.SetRangeIndices(Range, Position);
    }

    public bool HasViableTarget()
    {
        if (CurrentlyTargetedCharacter == null)
        {
            return false;
        }
        return true;
    }
}
