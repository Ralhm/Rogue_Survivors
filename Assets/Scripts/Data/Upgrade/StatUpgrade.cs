using Godot;
using System;

public enum StatUpgradeType
{
    MaxHealth,
    MoveRange,
    AttackRange,
    PhysAttack,
    MagAttack,
    PhysDefense,
    MagDefense,
    Counter
}

[GlobalClass]
public partial class StatUpgrade : UpgradeData
{
    [Export]
    StatUpgradeType StatType;


    //Let this contain data for how much is upgraded per rarity
    [Export]
    public float[] RarityUpgradeValues = [0.1f, 0.2f, 0.35f, 0.6f];



    public override void ApplyUpgrade(Ally Target, UpgradeRarity rarity)
    {
        switch (StatType)
        {
            case StatUpgradeType.MaxHealth:
                Target.UpgradeContainer.SetMaxHealth((int)GetValue(rarity));
                break;
            case StatUpgradeType.MoveRange:
                Target.UpgradeContainer.SetMoveRangeMult(GetValue(rarity));
                break;
            case StatUpgradeType.AttackRange:
                Target.UpgradeContainer.SetAttackRangeMult(GetValue(rarity));
                break;
            case StatUpgradeType.PhysAttack:
                Target.UpgradeContainer.SetAttackMult(GetValue(rarity));
                break;
            case StatUpgradeType.MagAttack:
                Target.UpgradeContainer.SetMagicMult(GetValue(rarity));
                break;
            case StatUpgradeType.PhysDefense:
                Target.UpgradeContainer.SetPhysDefenseMult(GetValue(rarity));
                break;
            case StatUpgradeType.MagDefense:
                Target.UpgradeContainer.SetMagDefenseMult(GetValue(rarity));
                break;

        }
    }

    public override float GetValue(UpgradeRarity rarity)
    {
        if (rarity == UpgradeRarity.Common)
        {
            return RarityUpgradeValues[0];
        }
        else if (rarity == UpgradeRarity.Uncommon)
        {
            return RarityUpgradeValues[1];
        }
        else if (rarity == UpgradeRarity.Rare)
        {
            return RarityUpgradeValues[2];
        }
        else if (rarity == UpgradeRarity.Legendary)
        {
            return RarityUpgradeValues[3];
        }
        return RarityUpgradeValues[0];
    }

    public override string GetUpgradeDesc(UpgradeRarity rarity)
    {
        if (StatType == StatUpgradeType.MaxHealth)
        {
            return (UpgradeDescription + GetValue(rarity));
        }
        
        return (UpgradeDescription + (int)(GetValue(rarity) * 100) + "%");

    }

}
