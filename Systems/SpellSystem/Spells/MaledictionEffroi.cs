using System.Collections.Generic;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static List<NwGameObject> MaledictionEffroi(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwGameObject oTarget, NwClass casterClass)
    {
      SpellUtils.SignalEventSpellCast(oTarget, oCaster, spell.SpellType);
      List<NwGameObject> targets = SpellUtils.GetSpellTargets(oCaster, oTarget, spellEntry, true);

      if (oCaster is not NwCreature caster)
      {
        int spellDC = SpellUtils.GetCasterSpellDC(oCaster, spell, casterClass.SpellCastingAbility);

        foreach (var target in targets)
          if (target is NwCreature targetCreature
            && CreatureUtils.GetSavingThrow(oCaster, targetCreature, spellEntry.savingThrowAbility, spellDC, spellEntry, SpellConfig.SpellEffectType.Fear) == SavingThrowResult.Failure)
        {
          target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpReduceAbilityScore));
          NWScript.AssignCommand(oCaster, () => targetCreature.ApplyEffect(EffectDuration.Temporary, EffectSystem.GetMaledictionEffroi(casterClass.SpellCastingAbility), SpellUtils.GetSpellDuration(oCaster, spellEntry)));
        }
      }

      return targets;
    }
  }
}
