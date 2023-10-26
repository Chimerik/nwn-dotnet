using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void AcidSplash(SpellEvents.OnSpellCast onSpellCast)
    {
      if (onSpellCast.Caster is not NwCreature oCaster)
        return;

      SpellUtils.SignalEventSpellCast(onSpellCast.TargetObject, oCaster, onSpellCast.Spell.SpellType);
      SpellEntry spellEntry = Spells2da.spellTable[onSpellCast.Spell.Id];
      SpellConfig.SavingThrowFeedback feedback = new();
      int spellDC = SpellUtils.GetCasterSpellDC(oCaster);
      
      onSpellCast.TargetLocation.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.FnfGasExplosionAcid));

      foreach (NwCreature target in onSpellCast.TargetLocation.GetObjectsInShapeByType<NwCreature>(Shape.Sphere, spellEntry.aoESize, false))
      {
        int advantage = 0;
        bool targetHandled = false;

        foreach (var eff in target.ActiveEffects)
        {
          targetHandled = SpellUtils.HandleSpellTargetIncapacitated(oCaster, target, eff.EffectType, spellEntry);

          if (targetHandled)
            break;

          advantage += SpellUtils.GetAbilityAdvantageFromEffect(spellEntry.savingThrowAbility, eff.Tag);
        }

        if (targetHandled)
          continue;

        int totalSave = SpellUtils.GetSavingThrowRoll(target, spellEntry, advantage, feedback);
        bool saveFailed = totalSave < spellDC;

        SpellUtils.SendSavingThrowFeedbackMessage(oCaster, target, feedback, advantage, spellDC, totalSave, saveFailed, spellEntry.savingThrowAbility);

        if (saveFailed) 
          SpellUtils.DealSpellDamage(target, oCaster.LastSpellCasterLevel, Spells2da.spellTable[onSpellCast.Spell.Id].damageDice, SpellUtils.GetSpellDamageDiceNumber(oCaster, onSpellCast.Spell), DamageType.Acid, VfxType.ImpAcidS);
      }
    }
  }
}
