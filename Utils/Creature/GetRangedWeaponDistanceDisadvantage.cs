using Anvil.API;
using System.Numerics;
using NWN.Native.API;
using BaseItemType = Anvil.API.BaseItemType;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static bool GetRangedWeaponDistanceDisadvantage(CNWSCreature attacker, BaseItemType weaponType, CNWSCreature target)
    {
      if(target.m_pStats.HasFeat(CustomSkill.TireurDelite).ToBool()
        || ItemUtils.IsRangedWeaponInOptimalRange(weaponType, Vector3.Distance(attacker.m_vPosition.ToManagedVector(), target.m_vPosition.ToManagedVector())))
      {
        return false;
      }
      else
      {
        LogUtils.LogMessage($"Désavantage - Attaque à distance hors de la portée optimale", LogUtils.LogType.Combat);
        return true;
      }
    }
  }
}
