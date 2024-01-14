using Anvil.API;
using System.Numerics;
using NWN.Native.API;
using NWN.Systems;
using BaseItemType = Anvil.API.BaseItemType;

namespace NWN
{
  public static partial class CreatureUtils
  {
    public static int GetRangedWeaponDistanceDisadvantage(CNWSCreature attacker, BaseItemType weaponType, CNWSCreature target)
    {
      if(target.m_pStats.HasFeat(CustomSkill.TireurDelite).ToBool()
        || ItemUtils.IsRangedWeaponInOptimalRange(weaponType, Vector3.Distance(attacker.m_vPosition.ToManagedVector(), target.m_vPosition.ToManagedVector())))
      {
        return 0;
      }
      else
      {
        LogUtils.LogMessage($"Désavantage - Attaque à distance hors de la portée optimale", LogUtils.LogType.Combat);
        return -1;
      }
    }
  }
}
