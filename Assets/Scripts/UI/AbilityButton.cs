using Godot;
using System;

public partial class AbilityButton : Button
{


    [Export]
    Ability StoredAbility;

    AbilityMenu Menu;

    public override void _Ready()
    {
        base._Ready();

        Pressed += OnButtonPressed;
        
        //GetParent<>
        VisibilityChanged += OnEnabled;

        //Select
        FocusEntered += OnButtonHovered;
        MouseEntered += OnButtonHovered;
        //AddThemeStyleboxOverride(GetThemeStylebox("focus", "Button"));


    }

    public void OnButtonPressed()
    {
        //GD.Print("Ability Button Pressed!");
        PlayerController.Instance.OnAbilityButtonPressed(StoredAbility);
    }

    public void OnButtonHovered()
    {
        //GD.Print("Focused Button!");
        Menu.SetDescriptionText(StoredAbility.GetDescription());
    }

    public void OnEnabled()
    {
        if (IsVisibleInTree())
        {

        }
    }

    public void SetAbility(Ability NewAbility)
    {
        StoredAbility = NewAbility;
        SetNameText();
    }

    public void SetNameText()
    {
        if (StoredAbility != null)
        {
            Text = StoredAbility.GetAbilityName();
        }
        else
        {
            GD.Print("ABILTY NOT SET");
        }
    }


    public void SetMenu(AbilityMenu NewMenu)
    {
        Menu = NewMenu;
    }
}
