using System.Linq;
using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static int HandleDefensesEnjoleuses(CNWSCreature target)
    {
      if (target.m_appliedEffects.Any(e => e.m_sCustomTag.CompareNoCase(EffectSystem.defensesEnjoleusesEffectExoTag).ToBool()))
      {
        EffectUtils.RemoveTaggedEffect(target, EffectSystem.defensesEnjoleusesEffectExoTag);
        LogUtils.LogMessage($"Defenses Enjoleuses dégâts divisés par 2", LogUtils.LogType.Combat);
        return 2;
      }

      return 1;
    }
  }
}
