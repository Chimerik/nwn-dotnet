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
        { EffectSystem.ShieldArmorDisadvantageEffectTag, false } ,
        { "blinded", false },
        { "poisoned", false },
        { "frightened", false },
        { EffectSystem.lightSensitivityEffectTag, false },
        { EffectSystem.boneChillEffectTag, false },
        { EffectSystem.PourfendeurDisadvantageEffectTag, false },
        { EffectSystem.ProvocationEffectTag, false },
      };

      Dictionary<string, bool> advantageDictionary = new()
      {
        { EffectSystem.trueStrikeEffectTag, false },
        { EffectSystem.BroyeurEffectTag, false },
        { EffectSystem.RecklessAttackEffectTag, false },
      };

      foreach (var eff in attacker.m_appliedEffects)
      {
        advantageDictionary[EffectSystem.trueStrikeEffectTag] = advantageDictionary[EffectSystem.trueStrikeEffectTag] || GetTrueStrikeAdvantage(eff);
        advantageDictionary[EffectSystem.BroyeurEffectTag] = advantageDictionary[EffectSystem.BroyeurEffectTag] || GetBroyeurAdvantage(eff);
        advantageDictionary[EffectSystem.RecklessAttackEffectTag] = advantageDictionary[EffectSystem.RecklessAttackEffectTag] || GetRecklessAttackAdvantage(eff);

        disadvantageDictionary[EffectSystem.ShieldArmorDisadvantageEffectTag] = disadvantageDictionary[EffectSystem.ShieldArmorDisadvantageEffectTag] || GetArmorShieldDisadvantage(eff, attackStat);
        disadvantageDictionary[EffectSystem.boneChillEffectTag] = disadvantageDictionary[EffectSystem.boneChillEffectTag] || GetBoneChillDisadvantage(attacker, eff);
        disadvantageDictionary["blinded"] = disadvantageDictionary["blinded"] || GetBlindedDisadvantage(eff);
        disadvantageDictionary["poisoned"] = disadvantageDictionary["poisoned"] || GetPoisonedDisadvantage(eff);
        disadvantageDictionary["frightened"] = disadvantageDictionary["frightened"] || GetFrightenedDisadvantage(eff, targetId.m_idSelf);
        disadvantageDictionary[EffectSystem.lightSensitivityEffectTag] = disadvantageDictionary[EffectSystem.lightSensitivityEffectTag] || GetDrowLightSensitivityDisadvantage(eff);
        disadvantageDictionary[EffectSystem.PourfendeurDisadvantageEffectTag] = disadvantageDictionary[EffectSystem.PourfendeurDisadvantageEffectTag] || GetPourfendeurDisadvantage(eff);
        disadvantageDictionary[EffectSystem.ProvocationEffectTag] = disadvantageDictionary[EffectSystem.ProvocationEffectTag] || GetProvocationDisadvantage(eff, targetId.m_idSelf);
      }

      return -disadvantageDictionary.Count(v => v.Value) + advantageDictionary.Count(v => v.Value);
    }
  }
}
