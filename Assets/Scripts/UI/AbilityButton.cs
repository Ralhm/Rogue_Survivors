using Godot;
using System;

public partial class AbilityButton : Button
{


    [Export]
    Ability StoredAbility;


    public override void _Ready()
    {
        base._Ready();

        Pressed += OnButtonPressed;
        
        //GetParent<>
        VisibilityChanged += OnEnabled;

    }

    public void OnButtonPressed()
    {
        GD.Print("Ability Button Pressed!");
        PlayerController.Instance.OnAbilityButtonPressed(StoredAbility);
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
        SetDescriptionText();
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

    public void SetDescriptionText()
    {

    }
}
