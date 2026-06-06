using Godot;
using System;
using System.Collections.Generic;

public partial class AbilityMenu : Control
{
    [Export]
    PackedScene ButtonPrefab = GD.Load<PackedScene>("res://Assets/UI/ability_button.tscn");

    
    List<AbilityButton> ButtonList = new List<AbilityButton>();

    [Export]
    VBoxContainer Box;

    [Export]
    RichTextLabel DescriptionText;

    public void SetAbilities(Ability[] abilities)
    {

        int i;
        for (i = 0; i < abilities.Length; i++)
        {

            if (ButtonList.Count <= i)
            {
                GD.Print("Generating Button!");
                GenerateButton(abilities[i]);
            }
            else
            {
                GD.Print("Button Already Exists!");
                ButtonList[i].Show();
                ButtonList[i].SetAbility(abilities[i]);
            }
        }
        if (i < ButtonList.Count - 1) {
            while (i < ButtonList.Count - 1) { 
                ButtonList[i].Hide();
            
            }
        }

    }

    public AbilityButton GenerateButton(Ability ability)
    {
        //Check and make sure this sets it as a child
        AbilityButton inst = ButtonPrefab.Instantiate() as AbilityButton;
        Box.AddChild(inst);
        ButtonList.Add(inst);
        inst.SetAbility(ability);
        inst.SetMenu(this);

        return inst;
    }

    public void SetDescriptionText(string text)
    {
        DescriptionText.Text = text;
    }


}
