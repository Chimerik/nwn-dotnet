using NWN.Native.API;
using NWN.Systems;
using BaseItemType = Anvil.API.BaseItemType;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static bool GetSmallCreaturesHeavyWeaponDisadvantage(CNWSCreature attacker, BaseItemType weaponType)
    {
      if ((attacker.m_nCreatureSize < (int)CreatureSize.Medium || attacker.m_pStats.m_nRace == (int)RacialType.Gnome
        || attacker.m_pStats.m_nRace == (int)RacialType.Halfling) && ItemUtils.IsHeavyWeapon(weaponType))
      {
        LogUtils.LogMessage("Désavantage - Créature de petite taille maniant une arme lourde", LogUtils.LogType.Combat);
        return true;
      }
      else
        return false;
    }
  }
}
