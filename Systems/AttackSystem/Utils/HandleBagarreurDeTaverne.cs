using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static int HandleBagarreurDeTaverne(CNWSCreature creature, CNWSItem weapon, int strBonus)
    {
      if (weapon is not null || strBonus < 1 || !creature.m_pStats.HasFeat(CustomSkill.BagarreurDeTaverne).ToBool())
        return 0;

      LogUtils.LogMessage($"Bagarreur de taverne dégâts : +{strBonus}", LogUtils.LogType.Combat);

      return strBonus;
    }
  }
}
