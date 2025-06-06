﻿using System;
using System.Collections.Generic;
using System.Linq;
using Anvil.API;
using Anvil.API.Events;
using NWN.Core;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static List<NwGameObject> MarqueDuChasseur(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwGameObject oTarget, NwFeat feat)
    {
      SpellUtils.SignalEventSpellCast(oTarget, oCaster, spell.SpellType);

      if (oTarget is not NwCreature target)
        return new List<NwGameObject>();

      oTarget.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpHeadEvil));

      Effect freeMarque = oCaster.ActiveEffects.FirstOrDefault(e => e.Tag == EffectSystem.FreeMarqueDuChasseurTag);
      TimeSpan duration = SpellUtils.GetSpellDuration(oCaster, spellEntry);

      if (oCaster is NwCreature caster)
      {
        if (freeMarque is not null)
        {
          duration = TimeSpan.FromSeconds(freeMarque.DurationRemaining);

          if (feat is not null)
            caster.IncrementRemainingFeatUses(feat);
        }

        if (caster.KnowsFeat((Feat)CustomSkill.RangerImplacable))
          caster.ApplyEffect(EffectDuration.Temporary, EffectSystem.ConcentrationAdvantage, duration);
      }

      NWScript.AssignCommand(oCaster, () => oTarget.ApplyEffect(EffectDuration.Temporary, EffectSystem.MarqueDuChasseur, duration));
      
      target.OnDeath -= OnDeathMarqueDuChasseur;
      target.OnDeath += OnDeathMarqueDuChasseur;

      return new List<NwGameObject>() { oTarget };
    }
    public static void OnDeathMarqueDuChasseur(CreatureEvents.OnDeath onDeath)
    {
      foreach (var eff in onDeath.KilledCreature.ActiveEffects)
        if (eff.Tag == EffectSystem.MarqueDuChasseurTag && eff.Creator is NwCreature caster && !caster.ActiveEffects.Any(e => e.Tag == EffectSystem.FreeMarqueDuChasseurTag))
        {
          caster.ApplyEffect(EffectDuration.Temporary, EffectSystem.FreeMarqueDuChasseur, TimeSpan.FromSeconds(eff.DurationRemaining));
          caster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpGoodHelp));
          caster.LoginPlayer?.SendServerMessage("Cible vaincue - Vous pouvez relancer marque du chasseur gratuitement", ColorConstants.Orange);
        }
    }
  }
}
