﻿using System.Collections.Generic;
using Anvil.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void CharmAnimal(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwGameObject oTarget, NwClass casterClass)
    {
      SpellUtils.SignalEventSpellCast(oTarget, oCaster, spell.SpellType);

      if (oCaster is not NwCreature caster)
        return;

      List<NwGameObject> targets = SpellUtils.GetSpellTargets(caster, oTarget, spellEntry, true);
      int DC = SpellUtils.GetCasterSpellDC(oCaster, spell, casterClass.SpellCastingAbility);

      foreach (var targetObject in targets)
      {
        if (targetObject is not NwCreature target || target.Race.RacialType != RacialType.Animal || EffectSystem.IsCharmeImmune(caster, target)
          || CreatureUtils.GetSavingThrow(caster, target, spellEntry.savingThrowAbility, DC, spellEntry) != SavingThrowResult.Failure)
          continue;

        EffectSystem.ApplyCharme(target, caster, spell, SpellUtils.GetSpellDuration(oCaster, spellEntry));
      }
    }
  }
}
