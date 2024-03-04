using Anvil.API;

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
  }
}
