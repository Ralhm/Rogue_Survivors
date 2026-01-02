using Godot;
using System;

public partial class DefendButton : Button
{

    public override void _Ready()
    {
        base._Ready();

        Pressed += OnButtonPressed;
    }

    public void OnButtonPressed()
    {
        PlayerController.Instance.OnButtonPress(StoredAction.Defend);
    }

}
