using Godot;
using System;

public partial class UpgradeButton : Button
{

    [Export]
    UpgradePanel Panel;



    public override void _Ready()
    {
        base._Ready();
        Pressed += ApplyUpgrade;

        FocusEntered += OnButtonHovered;
        MouseEntered += OnButtonHovered;
    }



    public void OnButtonHovered()
    {
        GD.Print("Upgrade Button Hovered!");
    }

    public void ApplyUpgrade()
    {
        GD.Print("Applying Upgrade!");
        Panel.ApplyUpgrade();

    }

}
