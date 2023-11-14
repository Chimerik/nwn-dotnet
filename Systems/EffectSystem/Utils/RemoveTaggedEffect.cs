using Anvil.API;

namespace NWN
{
  public static partial class EffectUtils
  {
    public static void RemoveTaggedEffect(NwCreature creature, string effectTag)
    {
      foreach (var eff in creature.ActiveEffects)
        if (eff.Tag == effectTag)
          creature.RemoveEffect(eff);
    }
  }
}
