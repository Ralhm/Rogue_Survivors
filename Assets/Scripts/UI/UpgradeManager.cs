using Godot;
using System;
using System.Collections.Generic;


//Let this script handle creating Upgrades to give to the player
public partial class UpgradeManager : Control
{

    //When an upgrade is selected, what are the odds of it being common/uncommon/rare/legendary
    [Export]
    public int[] RarityOdds = [60, 89, 99, 100];

    //What are the odds you'll get a generic upgrade or a character specific Upgrade?
    [Export]
    public int GenericUpgradeOdds = 5;

    [Export]
    public UpgradePanel[] UpgradeOptions;

    [Export]
    public UpgradePool GenericUpgrades;

    [Export]
    public UpgradePool MageUpgradePool;

    [Export]
    public UpgradePool KnightUpgradePool;

    [Export]
    public UpgradePool DevilUpgradePool;

    [Export]
    public UpgradePool AngelUpgradePool;

    //Pool of upgrades that have already been received and thus should not be shown 
    //Primarly for ability unlock upgrades
    public UpgradeData[] NoShowPool;

    //Let this or some other value influence how common Legendary upgrades show up based on various to be implemented factors
    public int RarityBias;

    UpgradeRarity SelectedRarity;

    UpgradeData SelectedUpgrade;

    public override void _Ready()
    {
        base._Ready();
        SelectUpgradesRandom();
    }


    public void SelectUpgradesRandom()
    {
        //AllyType type = SelectRandomAlly();
        AllyType type = AllyType.Knight;

        int rand;

        for (int i = 0;i < UpgradeOptions.Length;i++)
        {
            rand = GD.RandRange(0, 100);
            if (rand < GenericUpgradeOdds)
            {
                FindUpgradeFromPool(GenericUpgrades);
                rand = GD.RandRange(0, 2);
                type = CombatManager.Instance.AllyList[rand].AllyType;


            }
            else
            {
                rand = GD.RandRange(0, 2);
                type = CombatManager.Instance.AllyList[rand].AllyType;
                SelectUpgradeByCharacter(type);
            }



            

            UpgradeOptions[i].SetUpgrade(SelectedUpgrade, SelectedRarity, type);

        }
    }

    public void SelectUpgradeByCharacter(AllyType type)
    {
        GD.Print("Ally Type is: " + type);
        switch (type) {
            case AllyType.Knight:
                FindUpgradeFromPool(KnightUpgradePool);
                break;
            case AllyType.Mage: 
                FindUpgradeFromPool(MageUpgradePool);
                break;
            case AllyType.Devil:
                FindUpgradeFromPool(DevilUpgradePool);
                break;
            case AllyType.Angel:
                FindUpgradeFromPool(AngelUpgradePool);
                break;

        
        }
    }

    public void FindUpgradeFromPool(UpgradePool Pool)
    {
        int rand = GD.RandRange(0, RarityOdds[3] + RarityBias);

        GD.Print("Random Upgrade Number: " + rand);
        if (rand < RarityOdds[0])
        {
            SelectedRarity = UpgradeRarity.Common;
            SelectedUpgrade = Pool.GetCommonUpgrade();
            GD.Print("Common Upgrade!");
        }
        else if (rand < RarityOdds[1])
        {
            SelectedRarity = UpgradeRarity.Uncommon;
            SelectedUpgrade = Pool.GetUnCommonUpgrade();
            GD.Print("Uncommon Upgrade!");
        }
        else if (rand < RarityOdds[2])
        {
            SelectedRarity = UpgradeRarity.Rare;
            SelectedUpgrade = Pool.GetRareUpgrade();
            GD.Print("Rare Upgrade!");
        }
        else
        {
            SelectedRarity = UpgradeRarity.Legendary;
            SelectedUpgrade = Pool.GetLegendaryUpgrade();
            GD.Print("LEGENDARY Upgrade!");
        }
    }





}
