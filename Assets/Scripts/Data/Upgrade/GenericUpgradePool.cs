using Godot;
using System;


[GlobalClass]
public partial class GenericUpgradePool : UpgradePool
{

    public override UpgradeData GetCommonUpgrade()
    {
        return CommonUpgrades[GD.RandRange(0, CommonUpgrades.Length - 1)];
    }

    public override UpgradeData GetUnCommonUpgrade()
    {
        return CommonUpgrades[GD.RandRange(0, CommonUpgrades.Length - 1)];
    }

    public override UpgradeData GetRareUpgrade()
    {
        return CommonUpgrades[GD.RandRange(0, CommonUpgrades.Length - 1)];
    }

    public override UpgradeData GetLegendaryUpgrade()
    {
        return CommonUpgrades[GD.RandRange(0, CommonUpgrades.Length - 1)];
    }
}
