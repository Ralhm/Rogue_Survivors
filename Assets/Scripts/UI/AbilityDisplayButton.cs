using Godot;
using System;

public partial class AbilityDisplayButton : Button
{



    public override void _Ready()
    {
        base._Ready();

        Pressed += OnButtonPressed;
    }

    public void OnButtonPressed()
    {
        PlayerController.Instance.ShowAbilityMenu();
    }
}
