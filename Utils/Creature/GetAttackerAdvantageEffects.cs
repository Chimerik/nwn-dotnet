using System.Collections.Generic;
using System.Linq;
using Anvil.API;

namespace NWN
{
  public static partial class CreatureUtils
  {
    public static int GetAttackerAdvantageEffects(Native.API.CNWSCreature attacker, Native.API.CNWSCreature targetId, Ability attackStat)
    {
      Dictionary<string, bool> disadvantageDictionary = new()
      { 
        { "armorShield", false } ,
        { "blinded", false },
        { "poisoned", false },
        { "frightened", false },
        { "drowLightSensitivity", false },
      };

      foreach (var eff in attacker.m_appliedEffects)
      {
        disadvantageDictionary["armorShield"] = disadvantageDictionary["armorShield"] || GetArmorShieldDisadvantage(eff, attackStat);
        disadvantageDictionary["blinded"] = disadvantageDictionary["blinded"] || GetBlindedDisadvantage(eff);
        disadvantageDictionary["poisoned"] = disadvantageDictionary["poisoned"] || GetPoisonedDisadvantage(eff);
        disadvantageDictionary["frightened"] = disadvantageDictionary["frightened"] || GetFrightenedDisadvantage(eff, targetId.m_idSelf);
        disadvantageDictionary["drowLightSensitivity"] = disadvantageDictionary["drowLightSensitivity"] || GetDrowLightSensitivityDisadvantage(eff);
      }

      return -disadvantageDictionary.Count(v => v.Value);
    }
  }
}
