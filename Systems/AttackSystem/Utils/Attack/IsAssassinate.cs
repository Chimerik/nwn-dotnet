using System.Linq;
using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static bool IsAssassinate(CNWSCreature attacker, CNWSCreature target)
    {
      if (target is not null && attacker.m_pStats.HasFeat(CustomSkill.AssassinAssassinate).ToBool()
        && attacker.m_appliedEffects.Any(e => e.m_sCustomTag.ToString() == EffectSystem.AssassinateEffectTag)
        && attacker.m_nInitiativeRoll > target.m_nInitiativeRoll)
        return true;

      return false;
    }
  }
}
