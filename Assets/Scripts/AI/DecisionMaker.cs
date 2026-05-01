using Godot;
using System;
using System.Collections.Generic;


//NOTE TO SELF FOR TOUHOU FIGHTER
//HAVE THE AI'S SUBSCRIBE TO ACTION EVENTS SO THEY CAN RESPOND ACCORDINGLY????
public enum AI_Priority
{
    Offensive,
    Defensive,
    Support,
    Wildcard,
    Pattern //Create a custom DecisionMaker class, one that follows a pattern instead
}


public partial class DecisionMaker
{
    Enemy Owner;

    Decision BestDecision = new Decision();

    int F;

    public void SetOwner(Enemy owner)
    {
        Owner = owner;
    }

    public void MakeDecision()
    {
        GD.Print("Making a Decision...");
        Ability[] abilities = Owner.GetAbilities();
        for (int i = 0; i < abilities.Length; i++) {
            CalculateAbilityHeuristic(abilities[i]);
        }
        //Then, check the normal attack
        CalculateAttackHeuristic();

        if (BestDecision.GetAbility() != null)
        {
            GD.Print("Selected an Ability: " + BestDecision.GetAbility().GetAbilityName());
        }
        else
        {
            GD.Print("Selected an Attack!");
        }

    }

    void CalculateAttackHeuristic()
    {
        int H = 0, G = 0;
        if (Owner.GetPriority() == AI_Priority.Offensive)
        {
            H = 5; //Magic Number, replace later
        }
        float AttackRange = Owner.GetTotalAttackRange();
        List<Character> PotentialTargets = CombatManager.Instance.GetCharactersInRange(Owner.Position, AttackRange, TargetType.NotFriendly, true);
        for (int i = 0; i < PotentialTargets.Count; i++)
        {
            //GD.Print("Checking Target: " + PotentialTargets[i].Name);
            G = Owner.GetTotalNormalAttackDamage(PotentialTargets[i]);
            CheckDecision_Attack(G + H, PotentialTargets[i]);
        }
    }

    void CalculateAbilityHeuristic(Ability ability)
    {
        //First, find list of targets within range
        int H = 0, G = 0;
        if (ability.GetAIType() == Owner.GetPriority())
        {
            H = 50; //Magic Number, replace later
        }
        float AbilityRange;
        List<Character> PotentialTargets;
        if (ability.GetAimingType() == SelectionType.Target) 
        {
            AbilityRange = Owner.GetTotalAbilityRange(ability);
            PotentialTargets = CombatManager.Instance.GetCharactersInRange(Owner.Position, AbilityRange, ability.GetTargetingType(), true);
        }
        else //Positional and aim abilities don't care about where the user is
        {
            PotentialTargets = CombatManager.Instance.GetCharactersInRange(Owner.Position, 100000, ability.GetTargetingType(), true);
        }
        //GD.Print("Found Potential Targets...");
        
        //Then, for each target, calculate potential damage done/healing 
        if (ability.GetAIType() == AI_Priority.Offensive)
        {
            for (int i = 0; i < PotentialTargets.Count; i++)
            {
                if (PotentialTargets[i] == null)
                {
                    GD.Print("TARGET IS NULL");
                    continue;
                }
                //Don't bother checking enemies for indiscriminate attacks
                if (PotentialTargets[i].GetIsEnemy())
                {
                    GD.Print("Skipping enemy!");
                    continue;
                }
                G = ability.CalculateAbilityDamage(PotentialTargets[i], Owner);
                //GD.Print("Checking Target: " + PotentialTargets[i].Name);

                CheckDecision_Ability(G + H, PotentialTargets[i], ability);
            }
        }
        else if (ability.GetAIType() == AI_Priority.Support)
        {

            if (ability.GetRestorativeType() == RestorativeType.Health)
            {
                for (int i = 0; i < PotentialTargets.Count; i++)
                {
                    G = ability.GetHealthRestored(Owner);
                    float factor = PotentialTargets[i].GetHealthPercent();
                    if (factor >= 0.9f) { //If the target is at or near max health, don't bother healing
                        G = -200;
                    }
                    else
                    {
                        G = (int)(G / factor);
                    }

                    CheckDecision_Ability(G + H, PotentialTargets[i], ability);
                }

            }
            else if (ability.GetRestorativeType() == RestorativeType.Buff)
            {
                G = 25; //Magic Number, Needs Replacing
                for (int i = 0; i < PotentialTargets.Count; i++)
                {
                    //Check if the target is not buffed, so we prioritize unbuffed targets
                    if (!PotentialTargets[i].GetIsBuffed())
                    {
                        G += 50; //Magic Number, Needs Replacing
                    }
                    else
                    {
                        G += 15; //Magic Number, Needs Replacing
                    }

                    CheckDecision_Ability(G + H, PotentialTargets[i], ability);
                }
                
            }
        }

    }

    


    //Checl if this decision is the best possible decision
    public void CheckDecision_Ability(int f, Character target, Ability ability)
    {
        //Let 50 represent an element of randomness
        //Perhaps we could remove the randomness entirely if we wanted to make the game harder or for higher difficulty options
        //GD.Print("Checking Ability: " + ability.GetAbilityName() + " = " + f);

        f += (int)(GD.Randi() % 50);
        if (f > F)
        {
            F = f;
            BestDecision.SetValues(F, target, StoredAction.Ability, Owner.GetAbilityRange(ability), ability);
            GD.Print("Found a better ability: " + ability.GetAbilityName() + " = " + F);
        }

    }

    public void CheckDecision_Attack(int f, Character target)
    {
        //Let 50 represent an element of randomness
        //Perhaps we could remove the randomness entirely if we wanted to make the game harder or for higher difficulty options

        //GD.Print("Checking Attack: " + " = " + f);

        f += (int)(GD.Randi() % 50);
        if (f > F)
        {
            F = f;
            BestDecision.SetValues(F, target, StoredAction.Ability, Owner.GetAttackRange());
        }

    }

    public void ClearDecision()
    {
        F = 0;
        BestDecision.ClearDecision();
    }

    public Decision GetBestDecision()
    {
        return BestDecision;
    }

    public Character GetTarget()
    {
        return BestDecision.GetTarget();
    }

    public StoredAction GetStoredAction()
    {
        return BestDecision.GetStoredAction();
    }

    public Ability GetAbility()
    {
        return BestDecision.GetAbility();
    }

    public float GetMinDistanceToTarget()
    {
        return BestDecision.GetMinDistanceToTarget();
    }
}
