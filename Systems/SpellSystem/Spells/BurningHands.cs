using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void BurningHands(SpellEvents.OnSpellCast onSpellCast, SpellEntry spellEntry)
    {
      if (onSpellCast.Caster is not NwCreature oCaster)
        return;
      
      SpellUtils.SignalEventSpellCast(onSpellCast.TargetObject, oCaster, onSpellCast.Spell.SpellType);
      SpellConfig.SavingThrowFeedback feedback = new();
      int spellDC = SpellUtils.GetCasterSpellDC(oCaster, onSpellCast.SpellCastClass.SpellCastingAbility);

      bool evocateur = oCaster.KnowsFeat((Feat)CustomSkill.EvocateurFaconneurDeSorts);

      foreach (NwCreature target in onSpellCast.TargetLocation.GetObjectsInShapeByType<NwCreature>(Shape.SpellCone, 5, false, oCaster.Location.Position))
      {
        if (evocateur && !oCaster.IsReactionTypeHostile(target))
          continue;

        int advantage = CreatureUtils.GetCreatureAbilityAdvantage(target, spellEntry.savingThrowAbility, spellEntry, SpellConfig.SpellEffectType.Invalid, oCaster);

        if (advantage < -900)
          continue;

        int totalSave = SpellUtils.GetSavingThrowRoll(target, spellEntry.savingThrowAbility, spellDC, advantage, feedback, true);
        bool saveFailed = totalSave < spellDC;

        SpellUtils.SendSavingThrowFeedbackMessage(oCaster, target, feedback, advantage, spellDC, totalSave, saveFailed, spellEntry.savingThrowAbility);
        SpellUtils.DealSpellDamage(target, oCaster.CasterLevel, spellEntry, SpellUtils.GetSpellDamageDiceNumber(oCaster, onSpellCast.Spell), oCaster, onSpellCast.Spell.GetSpellLevelForClass(onSpellCast.SpellCastClass), saveFailed);
      }
    }
  }
}
