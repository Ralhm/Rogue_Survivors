using Godot;
using System;

public partial class UpgradePanel : Control
{

    [Export]
    public UpgradeButton UpgradeButton;

    public UpgradeData Data;

    [Export]
    public UpgradeManager UpgradeManager;


    [Export]
    RichTextLabel DescriptionText;

    [Export]
    RichTextLabel NameText;

    [Export]
    Texture2D[] CharacterIcons;

    [Export]
    TextureRect CharacterIcon;



    AllyType ChosenAlly;


    public UpgradeRarity RarityIndicator;

    public override void _Ready()
    {
        base._Ready();

    }

    public void SetUpgrade(UpgradeData data, UpgradeRarity rarity, AllyType type)
    {
        Data = data;
        RarityIndicator = rarity;
        DescriptionText.Text = Data.GetUpgradeDesc(RarityIndicator);
        NameText.Text = Data.GetUpgradeName();

        ChosenAlly = type;
        switch (ChosenAlly)
        {
            case AllyType.Knight:
                CharacterIcon.Texture = CharacterIcons[3];
                break;
            case AllyType.Mage:
                CharacterIcon.Texture = CharacterIcons[0];
                break;
            case AllyType.Angel:
                CharacterIcon.Texture = CharacterIcons[2];
                break;
            case AllyType.Devil:
                CharacterIcon.Texture = CharacterIcons[1];
                break;
        }  
    }

    public void ApplyUpgrade()
    {
        Data.ApplyUpgrade(CombatManager.Instance.AllyMap[ChosenAlly], RarityIndicator);
        UpgradeManager.Hide();
    }

}
