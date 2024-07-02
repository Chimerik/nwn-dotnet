using System.Collections.Generic;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static List<NwGameObject> MaledictionDegats(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwGameObject oTarget, NwClass casterClass)
    {
      List<NwGameObject> targetList = new();

      if (oCaster is not NwCreature caster || oTarget is not NwCreature target)
        return targetList;

      SpellUtils.SignalEventSpellCast(oTarget, oCaster, spell.SpellType);

      SpellConfig.SavingThrowFeedback feedback = new();
      int spellDC = SpellUtils.GetCasterSpellDC(oCaster, spell, casterClass.SpellCastingAbility);
      int advantage = CreatureUtils.GetCreatureAbilityAdvantage(target, spellEntry.savingThrowAbility, spellEntry, SpellConfig.SpellEffectType.Invalid, oCaster);
      int totalSave = SpellUtils.GetSavingThrowRoll(target, spellEntry.savingThrowAbility, spellDC, advantage, feedback, true);
      bool saveFailed = totalSave < spellDC;

      SpellUtils.SendSavingThrowFeedbackMessage(oCaster, target, feedback, advantage, spellDC, totalSave, saveFailed, spellEntry.savingThrowAbility);

      if (saveFailed)
      {
        targetList.Add(oTarget);
        target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpReduceAbilityScore));
        NWScript.AssignCommand(oCaster, () => target.ApplyEffect(EffectDuration.Temporary, EffectSystem.GetMaledictionDegats(target), NwTimeSpan.FromRounds(spellEntry.duration)));
      }

      return targetList;
    }
  }
}
