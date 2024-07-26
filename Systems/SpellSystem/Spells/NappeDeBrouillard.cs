﻿using System.Collections.Generic;
using Anvil.API;
using NWN.Core.NWNX;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static List<NwGameObject> NappeDeBrouillard(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, Location targetLocation)
    {
      List<NwGameObject> concentrationList = new List<NwGameObject>();

      if (oCaster is NwCreature caster)
      {
        SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);

        targetLocation.ApplyEffect(EffectDuration.Temporary, EffectSystem.NappeDeBrouillard(caster), NwTimeSpan.FromRounds(spellEntry.duration));  
        concentrationList.Add(UtilPlugin.GetLastCreatedObject(11).ToNwObject<NwAreaOfEffect>());
      }

      return concentrationList;
    }
  }
}
