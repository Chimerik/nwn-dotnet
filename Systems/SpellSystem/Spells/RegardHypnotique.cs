using System.Collections.Generic;
using System.Linq;
using Anvil.API;
using Anvil.API.Events;
using NWN.Core;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void RegardHypnotique(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwGameObject oTarget, NwClass castingClass)
    {
      if (oCaster is not NwCreature caster || oTarget is not NwCreature target || EffectSystem.IsCharmeImmune(target))
        return;

      var previousTargetList = caster.GetObjectVariable<LocalVariableString>(CreatureUtils.RegardHypnotiqueTargetListVariable).Value.Split("_");

      if (previousTargetList is not null && previousTargetList.Contains(caster.ToString()))
      {
        caster.LoginPlayer?.SendServerMessage("Cette capacité n'aura plus d'effet sur cette créature jusqu'à votre prochain repos long", ColorConstants.Red);
        return;
      }

      SpellUtils.SignalEventSpellCast(oTarget, caster, spell.SpellType);
      SpellConfig.SavingThrowFeedback feedback = new();
      int spellDC = SpellUtils.GetCasterSpellDC(caster, spell, castingClass.SpellCastingAbility);
      int advantage = CreatureUtils.GetCreatureAbilityAdvantage(target, spellEntry.savingThrowAbility, spellEntry, SpellConfig.SpellEffectType.Charm, caster);
      int totalSave = SpellUtils.GetSavingThrowRoll(target, spellEntry.savingThrowAbility, spellDC, advantage, feedback, true);
      bool saveFailed = totalSave < spellDC;

      SpellUtils.SendSavingThrowFeedbackMessage(caster, target, feedback, advantage, spellDC, totalSave, saveFailed, spellEntry.savingThrowAbility);
      
      if(saveFailed)
      {
        NWScript.AssignCommand(caster, () => target.ApplyEffect(EffectDuration.Permanent, EffectSystem.RegardHypnotique));
        EffectSystem.ApplyConcentrationEffect(caster, spell.Id, new List<NwGameObject> { caster }, spellEntry.duration);

        target.OnDamaged -= EffectSystem.OnDamageRegardHypnotique;
        target.OnDamaged += EffectSystem.OnDamageRegardHypnotique;
      }

      caster.GetObjectVariable<LocalVariableString>(CreatureUtils.RegardHypnotiqueTargetListVariable).Value += $"{caster}_";
    }
  }
}
