using System.Numerics;
using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static bool GetBlindedDisadvantage(CNWSCreature creature, CNWSCreature target)
    {
      if (!creature.m_pStats.HasFeat(CustomSkill.RangerSensSauvages).ToBool() 
        && (!creature.m_pStats.HasFeat(CustomSkill.FightingStyleCombatAveugle).ToBool() || Vector3.DistanceSquared(creature.m_vPosition.ToManagedVector(), target.m_vPosition.ToManagedVector()) > 16))
      {
        LogUtils.LogMessage("Désavantage - Attaquant aveuglé ou subissant Ténèbres", LogUtils.LogType.Combat);
        return true;
      }
      else
        return false;
    }
  }
}
