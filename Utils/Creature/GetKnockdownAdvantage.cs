using System.Linq;
using Anvil.API;
using NWN.Native.API;

namespace NWN
{
  public static partial class CreatureUtils
  {
    public static int GetKnockdownAdvantage(int rangedAttack, CNWSCreature target)
    {
      return target.m_appliedEffects.Any(e => (EffectTrueType)e.m_nType == EffectTrueType.Knockdown) 
        ? rangedAttack.ToBool() ? -1 : 1
        : 0;
    }
  }
}
