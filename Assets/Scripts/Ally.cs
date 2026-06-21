using Godot;
using System;
using System.Collections;
using System.Collections.Generic;

public partial class Ally : Character
{

    //Create a number icon that shows the order in which the characters will act
    [Export]
    Control OrderIcon;

    //Create a number icon that shows the order in which the characters will act
    [Export]
    RichTextLabel OrderNum;

    [Export]
    Node2D ProjectionTargetIndicator;

    [Export]
    CollisionShape2D Collider;


    [Export]
    protected AnimatedSprite2D ProjectionSprite;

    //This Exists so we can set the Collider to the projection location,
    //so players can't move characters onto the same position
    private Vector2 ColliderOrigin;

    [Export]
    public AllyType AllyType;


    public override void _Ready()
    {
        base._Ready();
        ColliderOrigin = Collider.Position;
        

        

        //UpgradeContainer.Owner = this;
        
    }



    public void SetOrderIcon(int num)
    {
        OrderIcon.Show();
        OrderNum.Text = num.ToString();
    }

    public void HideOrderIcon()
    {
        OrderIcon.Hide();
    }


    public override void OnBeginningOfPhase()
    {
        base.OnBeginningOfPhase();
    }

    public override void FinishNavigation()
    {
        base.FinishNavigation();
        
    }

    public override void ExecuteAction()
    {
        base.ExecuteAction();
        CombatManager.Instance.ExecuteNextAction();
    }

    public override void BeginAction()
    {
        AilmentContainer.AilmentEffects();
        if (SkippingTurn)
        {
            SkippingTurn = false;
            CombatManager.Instance.ExecuteNextAction();
            return;
        }

        BeginNavigation();
    }

    public override float GetStoredOffensiveRange()
    {
        if (CharacterAction == StoredAction.Attack)
        {
            GD.Print("Getting Normal Attack Range!");
            return BaseData.GetAttackRange() * UpgradeContainer.GetAttackRangeMult();
        }
        else if (CharacterAction == StoredAction.Ability)
        {
            //GD.Print("Getting Ability Range!");
            if (StoredAbility == null)
            {
                GD.Print("NO STORED ABILITY SET!)");
                return 0;
            }
            return BaseData.GetAttackRange() * UpgradeContainer.GetAttackRangeMult() * StoredAbility.GetAbilityRange();
        }
        return 0f;

    }

    public override int CalculateNormalAttackDamage()
    {
        return (int)(BaseData.GetAttack() * UpgradeContainer.GetAttackMult());
    }

    public override int GetAbilityDamage(DamageType type)
    {

        int Damage = 0;
        switch (type) {
            case DamageType.Physical:
                Damage = (int)(BaseData.GetAttack() * UpgradeContainer.GetAttackMult());
                Damage = (int)(Damage * BuffContainer.GetPhysAttack());
                break;
            case DamageType.Magic:
                Damage = (int)(BaseData.GetMagic() * UpgradeContainer.GetMagicMult());
                GD.Print("Base Calculated Damage: " + Damage);
                GD.Print("Buff Container Multiplier: " + BuffContainer.GetMagAttack());


                Damage = (int)(Damage * BuffContainer.GetMagAttack());
                GD.Print("Final Calculated Damage: " + Damage);
                break;
        
        }
        return Damage;
    }

    public override int GetDamageTaken(int Power, DamageType type, ElementType element, PhysicalType physical)
    {
        return base.GetDamageTaken(Power, type, element, physical);
    }

    public void SetProjectionArrowVisibility(bool visible) {
        if (!Projection.Visible)
        {
            return;
        }
        if (ProjectionTargetIndicator == null) {
            GD.Print("PROJECTION TARGET IS NOT SET");
            return;
        }

        if (visible) {
            ProjectionTargetIndicator.Show();

        }
        else
        {
            ProjectionTargetIndicator.Hide();
        }
    }

    public override void BeginNavigation()
    {
        base.BeginNavigation();
        Collider.Position = ColliderOrigin;
    }

    public override void SetActionSet()
    {
        base.SetActionSet();
        Collider.GlobalPosition = EndLocation + ColliderOrigin;
    }

    public void CancelAction() //If action has been set, compelete reset
    {
        Position = StartingLocation;
        CharacterState = CurrentState.Moving;
        StoredAbility = null;
        CharacterAction = StoredAction.None;
        OrderIcon.Hide();
        HideProjection();
        Collider.Position = ColliderOrigin;

    }



    public override float GetMoveRange()
    {
        float range = BaseData.GetMoveRange() * UpgradeContainer.GetMoveRangeMult();


        if (IsDashing)
        {
            range *= 2.0f;
        }
        if (IsTired)
        {
            range *= 0.5f;
        }
        return range;
    }

    
    public void AddAbility(Ability ability)
    {
        if (!Abilities.Contains(ability)) {
            Abilities.Add(ability);
            UpgradeContainer.AbilityLevels.Add(ability, 0);
        }
        else
        {
            UpgradeContainer.UpgradeAbility(ability);
        }
        
    }

    public override void ActivateAbility()
    {
        base.ActivateAbility();

        StoredAbility.OnActivate(this, UpgradeContainer.AbilityLevels[StoredAbility]);
    }

}