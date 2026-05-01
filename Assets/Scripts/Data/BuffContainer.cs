using Godot;
using System;
using System.Collections.Generic;

public partial class BuffContainer
{
    //Consider turning this into a map so we don't have to find the buff if it's already there?
    //But there will only ever be so many buffs, so maybe later
    List<Buff> Buffs = new List<Buff>(); 

    float MagDefense = 1.0f;
    float MagAttack = 1.0f;
    float PhysAttack = 1.0f;
    float PhysDefense = 1.0f;


    public void AddBuff(Buff buff)
    {
        if (Buffs.Contains(buff))
        {
            for (int i = 0; i < Buffs.Count; i++)
            {
                Buffs[i].IncrementDuration(buff.GetDuration());
            }
        }

        Buffs.Add(buff);
        switch (buff.GetBuffType()) {
            case BuffType.PhysAttack:
                PhysAttack += buff.GetPercentage();
                break;
            case BuffType.PhysDefense:
                PhysDefense += buff.GetPercentage();
                break;
            case BuffType.MagAttack:
                MagAttack += buff.GetPercentage();
                break;
            case BuffType.MagDefense:
                MagDefense += buff.GetPercentage();
                break;
        }
    }

    public void RemoveBuff(Buff buff) {
        DecrementBuff(buff);
        Buffs.Remove(buff);
        
    }

    public void RemoveBuffAt(int index)
    {
        DecrementBuff(Buffs[index]);
        Buffs.RemoveAt(index);
        
    }


    public void DecrementBuff(Buff buff) {
        switch (buff.GetBuffType())
        {
            case BuffType.PhysAttack:
                PhysAttack -= buff.GetPercentage();
                break;
            case BuffType.PhysDefense:
                PhysDefense -= buff.GetPercentage();
                break;
            case BuffType.MagAttack:
                MagAttack -= buff.GetPercentage();
                break;
            case BuffType.MagDefense:
                MagDefense -= buff.GetPercentage();
                break;
        }
    }

    public void DecreaseBuffDuration()
    {
        for (int i = 0; i < Buffs.Count; i++)
        {
            Buffs[i].IncrementDuration(-1);
            if (Buffs[i].GetDuration() <= 0) { 
                RemoveBuff(Buffs[i]);
            }
        }
    }

    public float GetMagDefense()
    {
        return MagDefense;
    }
    public float GetMagAttack()
    {
        return MagAttack;
    }
    public float GetPhysDefense()
    {
        return PhysDefense;
    }
    public float GetPhysAttack()
    {
        return PhysAttack;
    }

    public bool GetIsBuffed()
    {
        if (Buffs.Count == 0)
        {
            return false;
        }
        return true;
    }
}
