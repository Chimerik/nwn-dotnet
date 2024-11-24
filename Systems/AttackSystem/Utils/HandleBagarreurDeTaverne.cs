using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static int HandleBagarreurDeTaverne(CNWSCreature creature, CNWSItem weapon)
    {
      int bonusDamage = 0;

      if (weapon is null && creature.m_pStats.HasFeat(CustomSkill.BagarreurDeTaverne).ToBool())
      {
        int strMod = GetAbilityModifier(creature, Anvil.API.Ability.Strength);

        if (strMod > 0)
        { 
          bonusDamage = strMod;
          LogUtils.LogMessage($"Bagarreur de taverne dégâts : +{bonusDamage}", LogUtils.LogType.Combat);
        }
      }

      return bonusDamage;
    }
  }
}
