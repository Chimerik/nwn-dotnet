using NWN.Native.API;
using NWN.Systems;

namespace NWN
{
  public static partial class CreatureUtils
  {
    public static bool GetBoneChillDisadvantage(CNWSCreature creature, CGameEffect eff)
    {
      return (RacialType)creature.m_pStats.m_nRace == RacialType.Undead && eff.m_sCustomTag.CompareNoCase(EffectSystem.boneChillEffectExoTag) > 0;
    }
  }
}
