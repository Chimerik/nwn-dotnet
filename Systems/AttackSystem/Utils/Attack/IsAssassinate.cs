using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static bool IsAssassinate(CNWSCreature attacker)
    {
      if (attacker.m_pStats.HasFeat(CustomSkill.AssassinAssassinate).ToBool())
      {
        foreach (var eff in attacker.m_appliedEffects)
          if (CreatureUtils.GetAssassinateAdvantage(eff))
            return true;
      }

      return false;
    }
  }
}
