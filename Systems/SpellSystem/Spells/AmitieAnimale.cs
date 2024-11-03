using System.Collections.Generic;
using System.Linq;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void AmitieAnimale(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwGameObject oTarget, NwClass castingClass)
    {
      SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);

      if (oCaster is not NwCreature caster)
        return;

      List<NwGameObject> targets = SpellUtils.GetSpellTargets(caster, oTarget, spellEntry, true);
      int DC = SpellUtils.GetCasterSpellDC(oCaster, spell, castingClass.SpellCastingAbility);

      foreach (var targetObject in targets)
      {
        if (targetObject is NwCreature target && target.Race.RacialType == RacialType.Animal && target.Master is null 
          && caster.IsReactionTypeHostile(target) && !EffectSystem.IsCharmeImmune(caster, target)
          && CreatureUtils.GetSavingThrow(caster, target, spellEntry.savingThrowAbility, DC, spellEntry) == SavingThrowResult.Failure)
        {
          target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpHeadMind));
          NWScript.AssignCommand(caster, () => target.ApplyEffect(EffectDuration.Temporary, Effect.Pacified(), SpellUtils.GetSpellDuration(oCaster, spellEntry)));
        }
      }
    }
  }
}
