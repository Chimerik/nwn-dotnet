using System.Collections.Generic;
using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static int GetHunterMarqueBonusDamage(CNWSCreature creature, uint effCreator, uint effTarget, List<string> noStack)
    {
      if (effCreator == creature.m_idSelf || effCreator == creature.m_oidMaster)
      {
        int bonus = NwRandom.Roll(Utils.random, creature.m_pStats.HasFeat(CustomSkill.RangerPourfendeur).ToBool() ? 10 : 6);
        LogUtils.LogMessage($"Marque du chasseur : +{bonus} dégâts", LogUtils.LogType.Combat);

        return bonus;
      }

      noStack.Add(EffectSystem.MarqueDuChasseurTag);

      return 0;
    }
  }
}
