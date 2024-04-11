using System.Collections.Generic;
using System.Linq;
using Anvil.API;
using Anvil.API.Events;
using NWN.Core;
using NWN.Native.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void RegardHypnotique(NwCreature caster, SpellEvents.OnSpellCast onSpellCast, SpellEntry spellEntry)
    {
      if (onSpellCast.TargetObject is not NwCreature target || EffectSystem.IsCharmeImmune(target))
        return;

      var previousTargetList = caster.GetObjectVariable<LocalVariableString>(CreatureUtils.RegardHypnotiqueTargetListVariable).Value.Split("_");

      if (previousTargetList is not null && previousTargetList.Contains(caster.ToString()))
      {
        caster.LoginPlayer?.SendServerMessage("Cette capacité n'aura plus d'effet sur cette créature jusqu'à votre prochain repos long", ColorConstants.Red);
        return;
      }

      SpellUtils.SignalEventSpellCast(onSpellCast.TargetObject, caster, onSpellCast.Spell.SpellType);
      SpellConfig.SavingThrowFeedback feedback = new();
      int spellDC = SpellUtils.GetCasterSpellDC(caster, onSpellCast.SpellCastClass.SpellCastingAbility);
      int advantage = CreatureUtils.GetCreatureAbilityAdvantage(target, spellEntry.savingThrowAbility, spellEntry, SpellConfig.SpellEffectType.Charm, caster);
      int totalSave = SpellUtils.GetSavingThrowRoll(target, spellEntry.savingThrowAbility, spellDC, advantage, feedback, true);
      bool saveFailed = totalSave < spellDC;

      SpellUtils.SendSavingThrowFeedbackMessage(caster, target, feedback, advantage, spellDC, totalSave, saveFailed, spellEntry.savingThrowAbility);
      
      if(saveFailed)
      {
        NWScript.AssignCommand(caster, () => target.ApplyEffect(EffectDuration.Permanent, EffectSystem.RegardHypnotique));
        EffectSystem.ApplyConcentrationEffect(caster, onSpellCast.Spell.Id, new List<NwGameObject> { caster }, spellEntry.duration);

        target.OnDamaged -= EffectSystem.OnDamageRegardHypnotique;
        target.OnDamaged += EffectSystem.OnDamageRegardHypnotique;
      }

      caster.GetObjectVariable<LocalVariableString>(CreatureUtils.RegardHypnotiqueTargetListVariable).Value += $"{caster}_";
    }
  }
}
