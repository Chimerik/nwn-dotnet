using System.Numerics;
using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static bool GetBlindedDisadvantage(CNWSCreature creature, CNWSCreature target, CGameEffect eff)
    {
      if (EffectUtils.In((EffectTrueType)eff.m_nType, EffectTrueType.Blindness, EffectTrueType.Darkness)
        && !creature.m_pStats.HasFeat(CustomSkill.RangerSensSauvages).ToBool() 
        && (!creature.m_pStats.HasFeat(CustomSkill.FightingStyleCombatAveugle).ToBool() || Vector3.Distance(creature.m_vPosition.ToManagedVector(), target.m_vPosition.ToManagedVector()) > 4))
      {
        LogUtils.LogMessage("Désavantage - Attaquant aveuglé ou subissant Ténèbres", LogUtils.LogType.Combat);
        return true;
      }
      else
        return false;
    }
  }
}
