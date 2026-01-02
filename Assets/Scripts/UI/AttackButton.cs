using Godot;
using System;

public partial class AttackButton : Button
{

    [Export]
    public int TestVal = 5;

    public override void _Ready()
    {
        base._Ready();

        Pressed += OnButtonPressed;
    }

    public void OnButtonPressed()
    {
        PlayerController.Instance.OnButtonPress(StoredAction.Attack);
    }


}
