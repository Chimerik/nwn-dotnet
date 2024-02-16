using System.Collections.Generic;
using System.Linq;
using NWN.Native.API;
using NWN.Systems;

namespace NWN
{
  public static partial class CreatureUtils
  {
    public static int GetTargetAdvantageEffects(CNWSCreature target)
    {
      Dictionary<string, bool> advantageDictionary = new()
      {
        { "blinded", false },
        { "stunned", false },
        { "uncounscious", false },
        { "paralyzed", false },
        { "petrified", false },
        { EffectSystem.faerieFireEffectTag, false },
        { EffectSystem.RecklessAttackEffectTag, false },
        { EffectSystem.WolfTotemEffectTag, false },
      };

      Dictionary<string, bool> disadvantageDictionary = new()
      {
        { EffectSystem.DodgeEffectTag, false },
        { EffectSystem.ProtectionStyleEffectTag, false },
        { EffectSystem.JeuDeJambeEffectTag, false },
      };

      foreach (var eff in target.m_appliedEffects)
      {
        disadvantageDictionary[EffectSystem.DodgeEffectTag] = disadvantageDictionary[EffectSystem.DodgeEffectTag] || GetTargetDodgingDisadvantage(eff);
        disadvantageDictionary[EffectSystem.ProtectionStyleEffectTag] = disadvantageDictionary[EffectSystem.ProtectionStyleEffectTag] || GetProtectionStyleDisadvantage(eff);
        disadvantageDictionary[EffectSystem.JeuDeJambeEffectTag] = disadvantageDictionary[EffectSystem.JeuDeJambeEffectTag] || GetJeuDeJambeDisadvantage(eff);

        advantageDictionary["blinded"] = advantageDictionary["blinded"] || GetTargetBlindedAdvantage(eff);
        advantageDictionary["stunned"] = advantageDictionary["stunned"] || GetTargetStunnedAdvantage(eff);
        advantageDictionary["uncounscious"] = advantageDictionary["uncounscious"] || GetTargetUncounsciousAdvantage(eff);
        advantageDictionary["paralyzed"] = advantageDictionary["paralyzed"] || GetTargetParalyzedAdvantage(eff);
        advantageDictionary["petrified"] = advantageDictionary["petrified"] || GetTargetPetrifiedAdvantage(eff);
        advantageDictionary[EffectSystem.faerieFireEffectTag] = advantageDictionary[EffectSystem.faerieFireEffectTag] || GetTargetFaerieFireAdvantage(eff);
        advantageDictionary[EffectSystem.RecklessAttackEffectTag] = advantageDictionary[EffectSystem.RecklessAttackEffectTag] || GetAgainstRecklessAttackAdvantage(eff);
        advantageDictionary[EffectSystem.WolfTotemEffectTag] = advantageDictionary[EffectSystem.WolfTotemEffectTag] || GetWolfTotemAttackAdvantage(eff);
      }

      return -disadvantageDictionary.Count(v => v.Value) + advantageDictionary.Count(v => v.Value);
    }
  }
}
