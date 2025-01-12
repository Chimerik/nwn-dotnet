using System.Numerics;
using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static bool GetBlindedAdvantage(CNWSCreature attacker, CNWSCreature target)
    {
      if (!target.m_pStats.HasFeat(CustomSkill.RangerSensSauvages).ToBool() 
        && (!target.m_pStats.HasFeat(CustomSkill.FightingStyleCombatAveugle).ToBool() || Vector3.DistanceSquared(attacker.m_vPosition.ToManagedVector(), target.m_vPosition.ToManagedVector()) > 16))
      {
        LogUtils.LogMessage("Désavantage - Cible aveuglée ou subissant Ténèbres", LogUtils.LogType.Combat);
        return true;
      }
      else
        return false;
    }
  }
}
