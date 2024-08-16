using System.Collections.Generic;
using Anvil.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void MauvaisAugure(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwGameObject oTarget)
    {
      SpellUtils.SignalEventSpellCast(oTarget, oCaster, spell.SpellType);
      List<NwGameObject> targets = SpellUtils.GetSpellTargets(oCaster, oTarget, spellEntry, true);
      int spellDC = SpellUtils.GetCasterSpellDC(oCaster, spell, Ability.Wisdom);

      foreach (var target in targets)
      {
        if (target is NwCreature targetCreature)
        {
          SavingThrowResult saveResult = CreatureUtils.GetSavingThrow(oCaster, targetCreature, spellEntry.savingThrowAbility, spellDC);

          if (saveResult == SavingThrowResult.Failure)
          {
            target.ApplyEffect(EffectDuration.Temporary, EffectSystem.MauvaisAugure, NwTimeSpan.FromRounds(2));
            target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpPulseNegative));
          }
          else
            target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ComHitNegative));

          SpellUtils.DealSpellDamage(target, oCaster.CasterLevel, spellEntry, SpellUtils.GetSpellDamageDiceNumber(oCaster, spell), oCaster, 0, saveResult);
        }
      }
    }
  }
}
