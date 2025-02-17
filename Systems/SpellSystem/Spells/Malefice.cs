using System;
using System.Collections.Generic;
using System.Linq;
using Anvil.API;
using Anvil.API.Events;
using NWN.Core;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static List<NwGameObject> Malefice(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwGameObject oTarget)
    {
      SpellUtils.SignalEventSpellCast(oTarget, oCaster, spell.SpellType);

      if (oCaster is not NwCreature caster || oTarget is not NwCreature target)
        return new List<NwGameObject>();

      oTarget.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpHeadEvil));

      Effect freeMarque = oCaster.ActiveEffects.FirstOrDefault(e => e.Tag == EffectSystem.FreeMaleficeTag);
      TimeSpan duration = freeMarque is null ? SpellUtils.GetSpellDuration(oCaster, spellEntry) : TimeSpan.FromSeconds(freeMarque.DurationRemaining);

      NWScript.AssignCommand(oCaster, () => oTarget.ApplyEffect(EffectDuration.Temporary, EffectSystem.Malefice(caster, spell), duration));
      
      target.OnDeath -= OnDeathMalefice;
      target.OnDeath += OnDeathMalefice;

      return new List<NwGameObject>() { oTarget };
    }
    public static void OnDeathMalefice(CreatureEvents.OnDeath onDeath)
    {
      foreach (var eff in onDeath.KilledCreature.ActiveEffects)
        if (eff.Tag == EffectSystem.MaleficeTag && eff.Creator is NwCreature caster && !caster.ActiveEffects.Any(e => e.Tag == EffectSystem.FreeMaleficeTag))
        {
          caster.ApplyEffect(EffectDuration.Temporary, EffectSystem.FreeMalefice, TimeSpan.FromSeconds(eff.DurationRemaining));
          caster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpGoodHelp));
          caster.LoginPlayer?.SendServerMessage("Cible vaincue - Vous pouvez relancer maléfice gratuitement", ColorConstants.Orange);
        }
    }
  }
}
