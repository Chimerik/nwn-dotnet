using System.Linq;
using Anvil.API;
using NWN.Native.API;

namespace NWN
{
  public static partial class EffectUtils
  {
    public static void RemoveTaggedEffect(NwGameObject target, params string[] effectTag)
    {
      foreach (var eff in target.ActiveEffects)
        if (effectTag.Contains(eff.Tag))
          target.RemoveEffect(eff);
    }
    public static void RemoveTaggedEffect(NwGameObject target, NwObject creator, params string[] effectTag)
    {
      foreach (var eff in target.ActiveEffects)
        if (creator == eff.Creator && effectTag.Contains(eff.Tag))
          target.RemoveEffect(eff);
    }
    public static void RemoveTaggedEffect(CNWSCreature target, CExoString effectTag)
    {
      foreach (var eff in target.m_appliedEffects)
        if (eff.m_sCustomTag.CompareNoCase(effectTag).ToBool())
          target.RemoveEffect(eff);
    }
  }
}
