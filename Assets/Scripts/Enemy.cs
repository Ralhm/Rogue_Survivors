using Godot;
using System;

public partial class Enemy : Character
{


    DecisionMaker AIEngine = new DecisionMaker();


    //Let this be the deciding factor on how the Enemy should behave
    //Is it a support type, an offensive type, a defensive type, or maybe something else?
    [Export]
    AI_Priority MainPriority;

    //public override 

    public override void _Ready()
    {
        base._Ready();
        IsEnemy = true;
        
        AIEngine.SetOwner(this);
    }

    public override void _PhysicsProcess(double delta)
    {

    }

    //Rather than set a specific point in space that we want the enemy AI to go to
    //We navigate to the targets position until we reach the minimum distance required to execute the action
    //That distance being the ability/attack range
    public override void BeginNavigation()
    {
        EndLocation = AIEngine.GetTarget().Position;
        base.BeginNavigation();
    }

    public override void FinishNavigation()
    {
        base.FinishNavigation();
        ExecuteAction();
        CombatManager.Instance.ExecuteNextAction();
    }

    public override void ExecuteAction()
    {
        base.ExecuteAction();
    }

    public override int CalculateNormalAttackDamage()
    {
        return base.CalculateNormalAttackDamage();
    }

    public override int GetAbilityDamage(DamageType type)
    {
        return base.GetAbilityDamage(type);
    }

    public override int GetDamageTaken(int Power, DamageType type, ElementType element, PhysicalType physical)
    {
        return base.GetDamageTaken(Power, type, element, physical);
    }

    public AI_Priority GetPriority() { 
        return MainPriority;
    }

}
