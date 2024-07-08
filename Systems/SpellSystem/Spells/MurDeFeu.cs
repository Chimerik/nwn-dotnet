﻿using System.Collections.Generic;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static List<NwGameObject> MurDeFeu(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwClass castingClass, Location targetLocation)
    {
      SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);
      if (oCaster is NwCreature caster)
        caster.LoginPlayer?.SendServerMessage("Sort non implémenté pour le moment");
      //NWScript.AssignCommand(oCaster, () => oTarget.ApplyEffect(EffectDuration.Temporary, EffectSystem.SphereDeFeu(castingClass.SpellCastingAbility), NwTimeSpan.FromRounds(spellEntry.duration)));
      
      return new List<NwGameObject>() { oCaster };
    }
  }
}
