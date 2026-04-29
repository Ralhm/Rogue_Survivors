using Godot;
using System;
using System.Collections.Generic;

public partial class AilmentContainer : Resource
{

    List<Ailment> Ailments = new List<Ailment>();

    Character Owner;

    public void AddAilment(Ailment ail)
    {
        Ailments.Add(ail);
    }

    public void AilmentEffects()
    {
        if (Ailments.Count == 0)
        {
            return;
        }

        for (int i = 0; i < Ailments.Count; i++)
        {
            Ailments[i].AilmentEffect(Owner);
        }
    }

    public void ClearAilments()
    {
        Ailments.Clear();
    }

}
