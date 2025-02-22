using System.Linq;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static int GetPeauDecorceAC(CNWSCreature creature, int AC)
    {
      if (AC < 17 && creature.m_appliedEffects.Any(e => e.m_sCustomTag.ToString() == EffectSystem.PeauDecorceEffectTag))
      {
        AC = 17;
        LogUtils.LogMessage("Peau d'écorce: CA fixée à 17", LogUtils.LogType.Combat);
      }

      return AC;
    }
  }
}
