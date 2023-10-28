using System;
using System.Linq;
using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public static partial class DamageUtils
  {
    public static void HandleConcentration(OnCreatureDamage onDamage)
    {
      if (onDamage.Target is not NwCreature target || !target.ActiveEffects.Any(e => e.Tag == EffectSystem.concentration.Tag))
        return;

      SpellConfig.SavingThrowFeedback feedback = new();
      int advantage = CreatureUtils.GetCreatureAbilityAdvantage(target, Spells2da.spellTable[CustomSpell.PoisonSpray]); // je force sur poison spray pour avoir l'ability constitution
      int concentrationDC = 10;
      int totalDamage = 0;
      

      foreach (DamageType damageType in (DamageType[])Enum.GetValues(typeof(DamageType)))
        totalDamage += onDamage.DamageData.GetDamageByType(damageType) > -1
          ? onDamage.DamageData.GetDamageByType(damageType) : 0;

      totalDamage /= 2;
      concentrationDC = concentrationDC > totalDamage ? concentrationDC : totalDamage;

      int totalSave = SpellUtils.GetConcentrationSaveRoll(target, advantage, feedback);
      if(totalSave < concentrationDC)
      {
        string advantageString = advantage == 0 ? "" : advantage > 0 ? " (Avantage)".ColorString(StringUtils.gold) : " (Désavantage)".ColorString(ColorConstants.Red);
        string hitString = "ECHEC : CONCENTRATION PERDUE".ColorString(ColorConstants.Red);
        Color hitColor = ColorConstants.Red;

        string rollString = $"JDS CONSTITUTION{advantageString} {StringUtils.IntToColor(feedback.saveRoll, hitColor)} + {StringUtils.IntToColor(feedback.dexProficiencyBonus, hitColor)} = {StringUtils.IntToColor(totalSave, hitColor)} vs DD {StringUtils.IntToColor(concentrationDC, hitColor)}";
        string attackerName = "";

        if (onDamage.DamagedBy is NwCreature attacker)
        {
          attacker.LoginPlayer?.SendServerMessage($"{target.Name.ColorString(ColorConstants.Cyan)} - {rollString} {hitString}".ColorString(ColorConstants.Orange));
          attackerName = attacker.Name;
        }

        if (target != onDamage.DamagedBy)
          target.LoginPlayer?.SendServerMessage($"{attackerName.ColorString(ColorConstants.Cyan)} - {advantageString}{rollString} {hitString}".ColorString(ColorConstants.Orange));

        SpellUtils.DispelConcentrationEffects(target);
      }
    }
  }
}
