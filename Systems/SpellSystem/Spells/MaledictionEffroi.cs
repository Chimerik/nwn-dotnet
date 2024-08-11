using System.Collections.Generic;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static List<NwGameObject> MaledictionEffroi(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwGameObject oTarget, NwClass casterClass)
    {
      List<NwGameObject> targetList = new();

      if (oCaster is not NwCreature caster || oTarget is not NwCreature target)
        return targetList;

      SpellUtils.SignalEventSpellCast(oTarget, oCaster, spell.SpellType);

      int spellDC = SpellUtils.GetCasterSpellDC(oCaster, spell, casterClass.SpellCastingAbility);

      if (CreatureUtils.GetSavingThrow(oCaster, target, spellEntry.savingThrowAbility, spellDC, spellEntry, SpellConfig.SpellEffectType.Fear) == SavingThrowResult.Failure)
      {
        targetList.Add(oTarget);
        target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpReduceAbilityScore));
        NWScript.AssignCommand(oCaster, () => target.ApplyEffect(EffectDuration.Temporary, EffectSystem.GetMaledictionEffroi(casterClass.SpellCastingAbility), SpellUtils.GetSpellDuration(oCaster, spellEntry)));
      }

      return targetList;
    }
  }
}
