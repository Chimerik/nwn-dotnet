﻿using Anvil.API.Events;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static void OnDamageConcentration(CreatureEvents.OnDamaged onDamage)
    {
      if (onDamage.Creature.KnowsFeat((Feat)CustomSkill.InvocationConcentration)
        && NwSpell.FromSpellId(onDamage.Creature.GetObjectVariable<LocalVariableInt>(EffectSystem.ConcentrationSpellIdString)).SpellSchool == SpellSchool.Conjuration)
        return;

      SpellConfig.SavingThrowFeedback feedback = new();
      int advantage = GetCreatureAbilityAdvantage(onDamage.Creature, Ability.Constitution);
      advantage += onDamage.Creature.KnowsFeat((Feat)CustomSkill.MageDeGuerre) ? 1 : 0;
      int concentrationDC = 10;
      int totalDamage = onDamage.DamageAmount / 2;
      concentrationDC = concentrationDC > totalDamage ? concentrationDC : totalDamage;
      
      if(onDamage.Creature.GetObjectVariable<LocalVariableInt>("_CONCENTRATION_DISADVANTAGE").HasValue)
      {
        advantage -= 1;
        onDamage.Creature.GetObjectVariable<LocalVariableInt>("_CONCENTRATION_DISADVANTAGE").Delete();
      }

      int totalSave = SpellUtils.GetSavingThrowRoll(onDamage.Creature, Ability.Constitution, concentrationDC, advantage, feedback);
      if (totalSave < concentrationDC)
      {
        string advantageString = advantage == 0 ? "" : advantage > 0 ? " (Avantage)".ColorString(StringUtils.gold) : " (Désavantage)".ColorString(ColorConstants.Red);
        string hitString = "ECHEC : CONCENTRATION PERDUE".ColorString(ColorConstants.Red);
        Color hitColor = ColorConstants.Red;
        string rollString = $"JDS CONSTITUTION{advantageString} {StringUtils.IntToColor(feedback.saveRoll, hitColor)} + {StringUtils.IntToColor(feedback.proficiencyBonus, hitColor)} = {StringUtils.IntToColor(totalSave, hitColor)} vs DD {StringUtils.IntToColor(concentrationDC, hitColor)}";

        string message = $"{onDamage.Creature.Name.ColorString(ColorConstants.Cyan)} - {advantageString}{rollString} {hitString}".ColorString(ColorConstants.Orange);

        DelayConsoleMessage(onDamage.Creature, NWScript.GetLastDamager(onDamage.Creature).ToNwObject<NwObject>(), message);
        SpellUtils.DispelConcentrationEffects(onDamage.Creature);
      }
    }
    private static async void DelayConsoleMessage(NwCreature target, NwObject oDamager, string message)
    {
      await NwTask.NextFrame();

      if (oDamager is NwCreature attacker)
        attacker.LoginPlayer?.SendServerMessage(message);

      if (target != oDamager)
        target.LoginPlayer?.SendServerMessage(message);
    }
  }
}
