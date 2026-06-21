using Godot;
using System;

public enum UpgradeRarity
{
    Common,
    Uncommon,
    Rare,
    Legendary
}

public enum AllyType
{
    Knight,
    Devil,
    Angel,
    Mage
}

[GlobalClass]
public partial class UpgradeData : Resource
{

    [Export]
    public string UpgradeName;

    [Export]
    public string UpgradeDescription;

    public virtual void ApplyUpgrade(Ally Target, UpgradeRarity rarity)
    {

    }

    public virtual string GetUpgradeName()
    {
        return UpgradeName;
    }

    public virtual string GetUpgradeDesc(UpgradeRarity rarity)
    {
        return UpgradeDescription;
    }


    public virtual float GetValue(UpgradeRarity rarity)
    {
        return 0;
    }


}
