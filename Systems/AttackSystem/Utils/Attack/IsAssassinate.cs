using System.Linq;
using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static bool IsAssassinate(CNWSCreature attacker)
    {
      if (attacker.m_pStats.HasFeat(CustomSkill.AssassinAssassinate).ToBool()
        && attacker.m_appliedEffects.Any(e => e.m_sCustomTag.ToString() == EffectSystem.AssassinateEffectTag))
        return true;

      return false;
    }
  }
}
