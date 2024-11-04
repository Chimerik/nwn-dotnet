using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static int HandleBagarreurDeTaverne(CNWSCreature creature, CNWSItem weapon)
    {
      if (weapon is not null || !creature.m_pStats.HasFeat(CustomSkill.BagarreurDeTaverne).ToBool())
        return 0;

      int strMod = GetAbilityModifier(creature, Anvil.API.Ability.Strength);

      if (strMod < 1)
        return 0;

      LogUtils.LogMessage($"Bagarreur de taverne dégâts : +{strMod}", LogUtils.LogType.Combat);

      return strMod;
    }
  }
}
