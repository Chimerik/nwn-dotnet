using System.Collections.Generic;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static int GetAssassinateBonusDamage(CNWSCreature attacker, CNWSCreature target, List<string> noStack)
    {
      int bonusDamage = 0;
      noStack.Add(EffectSystem.AssassinateEffectTag);

      if (attacker.m_nInitiativeRoll > target.m_nInitiativeRoll)
      {
        bonusDamage = attacker.m_pStats.GetNumLevelsOfClass(CustomClass.Rogue);
        LogUtils.LogMessage($"Assassinat dégats bonus : {bonusDamage}", LogUtils.LogType.Combat);
      }
      return bonusDamage;
    }
  }
}
