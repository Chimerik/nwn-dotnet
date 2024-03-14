using Anvil.API;
using NWN.Native.API;
using NWN.Systems;

namespace NWN
{
  public static partial class EffectUtils
  {
    public static void RemoveTaggedEffect(NwGameObject target, string effectTag)
    {
      foreach (var eff in target.ActiveEffects)
        if (eff.Tag == effectTag)
          target.RemoveEffect(eff);
    }
    public static void RemoveTaggedEffect(NwGameObject target, string effectTag, NwObject creator)
    {
      foreach (var eff in target.ActiveEffects)
        if (eff.Tag == effectTag && creator == eff.Creator)
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
