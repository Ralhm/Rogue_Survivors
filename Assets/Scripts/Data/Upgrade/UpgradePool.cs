using Godot;
using System;

[GlobalClass]
public partial class UpgradePool : Resource
{

    [Export]
    public UpgradeData[] CommonUpgrades;

    [Export]
    public UpgradeData[] UnCommonUpgrades;

    [Export]
    public UpgradeData[] RareUpgrades;

    [Export]
    public UpgradeData[] LegendaryUpgrades;



    public virtual UpgradeData GetCommonUpgrade()
    {
        return CommonUpgrades[GD.RandRange(0, CommonUpgrades.Length - 1)];
    }

    public virtual UpgradeData GetUnCommonUpgrade() {
        return UnCommonUpgrades[GD.RandRange(0, UnCommonUpgrades.Length - 1)];
    }

    public virtual UpgradeData GetRareUpgrade()
    {
        return RareUpgrades[GD.RandRange(0, RareUpgrades.Length - 1)];
    }

    public virtual UpgradeData GetLegendaryUpgrade()
    {
        return LegendaryUpgrades[GD.RandRange(0, LegendaryUpgrades.Length - 1)];
    }

}
