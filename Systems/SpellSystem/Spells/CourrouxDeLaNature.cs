using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void CourrouxDeLaNature(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwGameObject oTarget)
    {
      if (oCaster is not NwCreature caster || oTarget is not NwCreature target) 
        return;

      SpellUtils.SignalEventSpellCast(oTarget, oCaster, spell.SpellType);
      SpellConfig.SavingThrowFeedback feedback = new();
      int spellDC = SpellUtils.GetCasterSpellDC(oCaster, spell, Ability.Charisma);
      Ability saveAbility = target.GetAbilityModifier(Ability.Strength) > target.GetAbilityModifier(Ability.Dexterity) ? Ability.Strength : Ability.Dexterity;
      int advantage = CreatureUtils.GetCreatureAbilityAdvantage(target, saveAbility, spellEntry, SpellConfig.SpellEffectType.Invalid);

      if (advantage < -900)
        return;

      int totalSave = SpellUtils.GetSavingThrowRoll(target, saveAbility, spellDC, advantage, feedback, true);
      bool saveFailed = totalSave < spellDC;

      SpellUtils.SendSavingThrowFeedbackMessage(oCaster, target, feedback, advantage, spellDC, totalSave, saveFailed, saveAbility);
      
      if(saveFailed)
        NWScript.AssignCommand(caster, () => target.ApplyEffect(EffectDuration.Temporary, EffectSystem.CourrouxDeLaNature, NwTimeSpan.FromRounds(10)));

      PaladinUtils.ConsumeOathCharge(caster);
    }
  }
}
