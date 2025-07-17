using System.Collections.Generic;
using Anvil.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void CeciteSurdite(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwGameObject oTarget, NwClass casterClass)
    {
      SpellUtils.SignalEventSpellCast(oTarget, oCaster, spell.SpellType);

      if (oCaster is not NwCreature caster)
        return;

      List<NwGameObject> targets = SpellUtils.GetSpellTargets(caster, oTarget, spellEntry, true);
      int DC = SpellUtils.GetCasterSpellDC(oCaster, spell, casterClass.SpellCastingAbility);

      foreach (var targetObject in targets)
      {
        if (targetObject is NwCreature target
          && CreatureUtils.GetSavingThrowResult(target, spellEntry.savingThrowAbility, caster, DC, spellEntry) == SavingThrowResult.Failure)
        {
          target.ApplyEffect(EffectDuration.Temporary, EffectSystem.CeciteSurdite, SpellUtils.GetSpellDuration(oCaster, spellEntry));
        }
      }
    }
  }
}
