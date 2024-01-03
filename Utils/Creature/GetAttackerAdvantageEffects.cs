using System.Collections.Generic;
using System.Linq;
using Anvil.API;
using NWN.Systems;

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
        { "boneChillDisadvantage", false },
        { "protectionStyle", false },
        { "pourfendeur", false },
        { EffectSystem.ProvocationEffectTag, false },
      };

      Dictionary<string, bool> advantageDictionary = new()
      {
        { "trueStrikeAdvantage", false },
        { "broyeur", false },
      };

      foreach (var eff in attacker.m_appliedEffects)
      {
        advantageDictionary["trueStrikeAdvantage"] = advantageDictionary["trueStrikeAdvantage"] || eff.m_sCustomTag.CompareNoCase(EffectSystem.trueStrikeEffectExoTag).ToBool();
        advantageDictionary["broyeur"] = advantageDictionary["broyeur"] || eff.m_sCustomTag.CompareNoCase(EffectSystem.BroyeurEffectExoTag).ToBool();

        disadvantageDictionary["armorShield"] = disadvantageDictionary["armorShield"] || GetArmorShieldDisadvantage(eff, attackStat);
        disadvantageDictionary["boneChillDisadvantage"] = disadvantageDictionary["boneChillDisadvantage"] || GetBoneChillDisadvantage(attacker, eff);
        disadvantageDictionary["blinded"] = disadvantageDictionary["blinded"] || GetBlindedDisadvantage(eff);
        disadvantageDictionary["poisoned"] = disadvantageDictionary["poisoned"] || GetPoisonedDisadvantage(eff);
        disadvantageDictionary["frightened"] = disadvantageDictionary["frightened"] || GetFrightenedDisadvantage(eff, targetId.m_idSelf);
        disadvantageDictionary["drowLightSensitivity"] = disadvantageDictionary["drowLightSensitivity"] || GetDrowLightSensitivityDisadvantage(eff);
        disadvantageDictionary["protectionStyle"] = disadvantageDictionary["protectionStyle"] || GetProtectionStyleDisadvantage(eff);
        disadvantageDictionary["pourfendeur"] = disadvantageDictionary["pourfendeur"] || GetPourfendeurDisadvantage(eff);
        disadvantageDictionary[EffectSystem.ProvocationEffectTag] = disadvantageDictionary[EffectSystem.ProvocationEffectTag] || GetProvocationDisadvantage(eff, targetId.m_idSelf);
      }

      return -disadvantageDictionary.Count(v => v.Value) + advantageDictionary.Count(v => v.Value);
    }
  }
}
