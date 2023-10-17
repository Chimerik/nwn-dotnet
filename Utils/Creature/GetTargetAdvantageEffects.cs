using System.Collections.Generic;
using System.Linq;
using NWN.Native.API;

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
      };

      foreach (var eff in target.m_appliedEffects)
      {
        advantageDictionary["blinded"] = advantageDictionary["blinded"] || GetTargetBlindedAdvantage(eff);
        advantageDictionary["stunned"] = advantageDictionary["stunned"] || GetTargetStunnedAdvantage(eff);
        advantageDictionary["uncounscious"] = advantageDictionary["uncounscious"] || GetTargetUncounsciousAdvantage(eff);
        advantageDictionary["paralyzed"] = advantageDictionary["paralyzed"] || GetTargetParalyzedAdvantage(eff);
        advantageDictionary["petrified"] = advantageDictionary["petrified"] || GetTargetPetrifiedAdvantage(eff);
      }

      // TODO : si la cible est sous l'effet de l'action "esquivez", l'attaquant a un désavantage

      return advantageDictionary.Count(s => s.Value);
    }
  }
}
