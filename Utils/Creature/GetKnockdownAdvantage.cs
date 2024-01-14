using System.Linq;
using Anvil.API;
using NWN.Native.API;

namespace NWN
{
  public static partial class CreatureUtils
  {
    public static int GetKnockdownAdvantage(int rangedAttack, CNWSCreature target)
    {
      if (target.m_appliedEffects.Any(e => (EffectTrueType)e.m_nType == EffectTrueType.Knockdown))
      {
        if(rangedAttack.ToBool())
        {
          LogUtils.LogMessage($"Désavantage - Attaque à distance sur une cible à terre", LogUtils.LogType.Combat);
          return -1;
        }
        else 
        {
          LogUtils.LogMessage($"Avantage - Attaque de mêlée sur une cible à terre", LogUtils.LogType.Combat);
          return 1;
        }
      }
      else
        return 0;      
    }
  }
}
