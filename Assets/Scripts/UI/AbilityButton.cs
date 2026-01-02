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

        AbilityMenu Menu = GetParent<AbilityMenu>();
    }

    public void OnButtonPressed()
    {
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
        Text = StoredAbility.GetName();
    }

    public void SetDescriptionText()
    {

    }
}
