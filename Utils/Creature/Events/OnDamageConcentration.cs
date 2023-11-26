using System;
using Anvil.API.Events;
using Anvil.API;
using NWN.Systems;

namespace NWN
{
  public static partial class CreatureUtils
  {
    public static void OnDamageConcentration(CreatureEvents.OnDamaged onDamage)
    {
      SpellConfig.SavingThrowFeedback feedback = new();
      int advantage = GetCreatureAbilityAdvantage(onDamage.Creature, Spells2da.spellTable[CustomSpell.PoisonSpray]); // je force sur poison spray pour avoir l'ability constitution
      int concentrationDC = 10;
      int totalDamage = onDamage.DamageAmount / 2;
      concentrationDC = concentrationDC > totalDamage ? concentrationDC : totalDamage;

      int totalSave = SpellUtils.GetConcentrationSaveRoll(onDamage.Creature, advantage, feedback);
      if (totalSave < concentrationDC)
      {
        string advantageString = advantage == 0 ? "" : advantage > 0 ? " (Avantage)".ColorString(StringUtils.gold) : " (Désavantage)".ColorString(ColorConstants.Red);
        string hitString = "ECHEC : CONCENTRATION PERDUE".ColorString(ColorConstants.Red);
        Color hitColor = ColorConstants.Red;

        string rollString = $"JDS CONSTITUTION{advantageString} {StringUtils.IntToColor(feedback.saveRoll, hitColor)} + {StringUtils.IntToColor(feedback.dexProficiencyBonus, hitColor)} = {StringUtils.IntToColor(totalSave, hitColor)} vs DD {StringUtils.IntToColor(concentrationDC, hitColor)}";
        string attackerName = "";

        if (onDamage.Damager is NwCreature attacker)
        {
          attacker.LoginPlayer?.SendServerMessage($"{onDamage.Creature.Name.ColorString(ColorConstants.Cyan)} - {rollString} {hitString}".ColorString(ColorConstants.Orange));
          attackerName = attacker.Name;
        }

        if (onDamage.Creature != onDamage.Damager)
          onDamage.Creature.LoginPlayer?.SendServerMessage($"{attackerName.ColorString(ColorConstants.Cyan)} - {advantageString}{rollString} {hitString}".ColorString(ColorConstants.Orange));

        SpellUtils.DispelConcentrationEffects(onDamage.Creature);
      }
    }
  }
}
