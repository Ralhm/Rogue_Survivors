using Godot;
using System;

[GlobalClass]
public partial class Ability_BloodyRestoration : Ability
{
    [Export]
    int SelfDamage;

    public override void AbilityEffect(Character Target, Character User, int level = 1)
    {
        for (int i = 0; i < CombatManager.Instance.AllyList.Length; i++) { 
            if (CombatManager.Instance.AllyList[i].AllyType != AllyType.Devil)
            {
                CombatManager.Instance.AllyList[i].Heal(1000);
                CombatManager.Instance.AllyList[i].HealAilments();
                CombatManager.Instance.AllyList[i].HealDeBuffs();
            }

        }
        User.TakeDamage(User.GetMaxHealth() / 2);

    }


}
