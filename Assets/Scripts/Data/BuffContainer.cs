using Godot;
using System;
using System.Collections.Generic;

public partial class BuffContainer
{

    List<Buff> Buffs = new List<Buff>(); 
    List<Buff> DeBuffs = new List<Buff>(); 

    float MagDefense = 1.0f;
    float MagAttack = 1.0f;
    float PhysAttack = 1.0f;
    float PhysDefense = 1.0f;
    bool Drain = false;


    public void AddBuff(BuffData NewBuff, int PowerLevel)
    {
        //Kinda nasty but whatever, there probably won't ever be more than a few buffs at a time
        if (!NewBuff.isDebuff) {
            //Increment the buff duration if the character already has the buff
            for (int i = 0; i < Buffs.Count; i++)
            {
                if (Buffs[i].Data == NewBuff)
                {
                    Buffs[i].IncrementDuration(NewBuff.GetDuration());
                    return;
                }

            }
        }
        else
        {
            //Increment the Debuff duration if the character already has the buff
            for (int i = 0; i < DeBuffs.Count; i++)
            {
                if (DeBuffs[i].Data == NewBuff)
                {
                    DeBuffs[i].IncrementDuration(NewBuff.GetDuration());
                    return;
                }

            }
        }

        Buff buff = new Buff();
        buff.SetData(NewBuff, PowerLevel);
        int DebuffMult = 1;

        if (NewBuff.isDebuff)
        {
            Buffs.Add(buff);
            DebuffMult = -1;
        }
        else
        {
            DeBuffs.Add(buff);
        }

        switch (NewBuff.GetBuffType())
        {
            case BuffType.PhysAttack:
                PhysAttack += buff.GetBuffPower() * DebuffMult;
                break;
            case BuffType.PhysDefense:
                PhysDefense += buff.GetBuffPower() * DebuffMult;
                break;
            case BuffType.MagAttack:
                MagAttack += buff.GetBuffPower() * DebuffMult;
                break;
            case BuffType.MagDefense:
                MagDefense += buff.GetBuffPower() * DebuffMult;
                break;
            case BuffType.Drain:
                Drain = true;
                break;
        }
    }


    public void RemoveBuff(Buff buff) {
        DecrementBuff(buff);
        if (buff.GetBuffPower() > 0) {
            Buffs.Remove(buff);
        }
        else
        {
            DeBuffs.Remove(buff);
        }

        
        
    }



    public void DecrementBuff(Buff buff) {
        int DebuffMult = 1;
        if (buff.Data.isDebuff)
        {
            DebuffMult = -1;
        }

        switch (buff.Data.GetBuffType())
        {
            case BuffType.PhysAttack:
                PhysAttack -= buff.GetBuffPower() * DebuffMult;
                break;
            case BuffType.PhysDefense:
                PhysDefense -= buff.GetBuffPower() * DebuffMult;
                break;
            case BuffType.MagAttack:
                MagAttack -= buff.GetBuffPower() * DebuffMult;
                break;
            case BuffType.MagDefense:
                MagDefense -= buff.GetBuffPower() * DebuffMult;
                break;
            case BuffType.Drain:
                Drain = false;
                break;
        }
    }

    public void DecreaseBuffDuration()
    {
        for (int i = 0; i < Buffs.Count; i++)
        {
            Buffs[i].IncrementDuration(-1);
            if (Buffs[i].Data.GetDuration() <= 0) { 
                RemoveBuff(Buffs[i]);
            }
        }
    }

    public void ClearBuffs()
    {
        while (Buffs.Count > 0)
        {
            RemoveBuff(Buffs[0]);
        }
    }

    public void ClearDeBuffs()
    {
        while (DeBuffs.Count > 0)
        {
            RemoveBuff(DeBuffs[0]);
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

    public bool GetIsDeBuffed ()
    {
        if (DeBuffs.Count == 0)
        {
            return false;
        }
        return true;
    }
}
