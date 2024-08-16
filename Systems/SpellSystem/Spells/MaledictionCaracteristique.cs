using System.Collections.Generic;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static List<NwGameObject> MaledictionCaracteristique(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwGameObject oTarget, NwClass casterClass, int spellId)
    {
      SpellUtils.SignalEventSpellCast(oTarget, oCaster, spell.SpellType);
      List<NwGameObject> targets = SpellUtils.GetSpellTargets(oCaster, oTarget, spellEntry, true);

      if (oCaster is NwCreature caster)
      {
        int spellDC = SpellUtils.GetCasterSpellDC(oCaster, spell, casterClass.SpellCastingAbility);

        foreach (var target in targets)
          if (target is NwCreature targetCreature
            && CreatureUtils.GetSavingThrow(oCaster, targetCreature, spellEntry.savingThrowAbility, spellDC, spellEntry) == SavingThrowResult.Failure)
          {
            targetCreature.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpReduceAbilityScore));
            NWScript.AssignCommand(oCaster, () => targetCreature.ApplyEffect(EffectDuration.Temporary, EffectSystem.GetMaledictionCaracteristique(spellId), SpellUtils.GetSpellDuration(oCaster, spellEntry)));
          }
      }

      return targets;
    }
  }
}
