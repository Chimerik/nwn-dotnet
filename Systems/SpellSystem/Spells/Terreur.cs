using System.Collections.Generic;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static List<NwGameObject> Terreur(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwGameObject oTarget, NwClass casterClass, Location targetLocation)
    {
      List<NwGameObject> targetList = new();

      if (oCaster is not NwCreature caster)
        return targetList;

      SpellUtils.SignalEventSpellCast(oTarget, oCaster, spell.SpellType);
      SpellConfig.SavingThrowFeedback feedback = new();
      int spellDC = SpellUtils.GetCasterSpellDC(oCaster, spell, casterClass.SpellCastingAbility);

      foreach (NwCreature target in targetLocation.GetObjectsInShapeByType<NwCreature>(Shape.SpellCone, 9, false, oCaster.Location.Position))
      {
        if (EffectSystem.IsFrightImmune(target, caster))
          continue;

        int advantage = CreatureUtils.GetCreatureAbilityAdvantage(target, spellEntry.savingThrowAbility, spellEntry, SpellConfig.SpellEffectType.Fear, oCaster);
        int totalSave = SpellUtils.GetSavingThrowRoll(target, spellEntry.savingThrowAbility, spellDC, advantage, feedback, true);
        bool saveFailed = totalSave < spellDC;

        SpellUtils.SendSavingThrowFeedbackMessage(oCaster, target, feedback, advantage, spellDC, totalSave, saveFailed, spellEntry.savingThrowAbility);

        if (saveFailed)
        {
          NWScript.AssignCommand(oCaster, () => target.ApplyEffect(EffectDuration.Temporary, EffectSystem.GetTerreurEffect(casterClass.SpellCastingAbility), NwTimeSpan.FromRounds(spellEntry.duration)));
          targetList.Add(target);  
        }
      }

      return targetList;
    }
  }
}
