using Godot;
using System;
using System.Collections.Generic;

public partial class AbilityMenu : VBoxContainer
{
    [Export]
    PackedScene ButtonPrefab = GD.Load<PackedScene>("res://Assets/UI/ability_button.tscn");

    
    List<AbilityButton> ButtonList = new List<AbilityButton>();




    public void SetAbilities(Ability[] abilities)
    {
        if (ButtonList.Count > abilities.Length) { //Hide Extra Buttons
            for (int i = 0; i < abilities.Length; i++)
            {
                if (i < ButtonList.Count)
                {
                    ButtonList[i].Show();
                }
                else
                {
                    ButtonList[i].Hide();
                }
            }
        
        }
        else if (ButtonList.Count < abilities.Length) //Generate New Buttons
        {
            for (int i = 0; i < (abilities.Length - ButtonList.Count); i++)
            {
                GenerateButton();
            }

        }
    }

    public void GenerateButton()
    {
        //Check and make sure this sets it as a child
        AbilityButton inst = ButtonPrefab.Instantiate() as AbilityButton;
        AddChild(inst);
        ButtonList.Add(inst);
    }


}
