using Godot;
using System;



[GlobalClass]
public partial class AbilityUnlockUpgrade : UpgradeData
{

    [Export]
    public Ability NewAbility;




    public override void ApplyUpgrade(Ally Target, UpgradeRarity rarity)
    {
        Target.AddAbility(NewAbility);
    }

    public override string GetUpgradeName()
    {
        return NewAbility.GetAbilityName();
    }

    public override string GetUpgradeDesc(UpgradeRarity rarity)
    {
        return NewAbility.GetDescription();
    }


    public override float GetValue(UpgradeRarity rarity)
    {
        if (rarity == UpgradeRarity.Common)
        {
            return NewAbility.GetPowerLevels()[0];
        }
        else if (rarity == UpgradeRarity.Uncommon)
        {
            return NewAbility.GetPowerLevels()[1];
        }
        else if (rarity == UpgradeRarity.Rare)
        {
            return NewAbility.GetPowerLevels()[2];
        }
        else if (rarity == UpgradeRarity.Legendary)
        {
            return NewAbility.GetPowerLevels()[3];
        }
        return NewAbility.GetPowerLevels()[0];
    }

}
