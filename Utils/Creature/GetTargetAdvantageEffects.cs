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
        { "faerieFire", false },
      };

      Dictionary<string, bool> disadvantageDictionary = new()
      {
        { "targetDodge", false },
      };

      foreach (var eff in target.m_appliedEffects)
      {
        disadvantageDictionary["targetDodge"] = disadvantageDictionary["targetDodge"] || GetTargetDodgingDisadvantage(eff);

        advantageDictionary["blinded"] = advantageDictionary["blinded"] || GetTargetBlindedAdvantage(eff);
        advantageDictionary["stunned"] = advantageDictionary["stunned"] || GetTargetStunnedAdvantage(eff);
        advantageDictionary["uncounscious"] = advantageDictionary["uncounscious"] || GetTargetUncounsciousAdvantage(eff);
        advantageDictionary["paralyzed"] = advantageDictionary["paralyzed"] || GetTargetParalyzedAdvantage(eff);
        advantageDictionary["petrified"] = advantageDictionary["petrified"] || GetTargetPetrifiedAdvantage(eff);
        advantageDictionary["faerieFire"] = advantageDictionary["faerieFire"] || GetTargetFaerieFireAdvantage(eff);
      }

      // TODO : si la cible est sous l'effet de l'action "esquivez", l'attaquant a un désavantage

      return -disadvantageDictionary.Count(v => v.Value) + advantageDictionary.Count(v => v.Value);
    }
  }
}
