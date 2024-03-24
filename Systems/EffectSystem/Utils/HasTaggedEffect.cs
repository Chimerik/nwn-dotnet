using System.Linq;
using Anvil.API;

namespace NWN
{
  public static partial class EffectUtils
  {
    public static bool HasTaggedEffect(NwGameObject target, string effectTag)
    {
      if (target.ActiveEffects.Any(e => e.Tag == effectTag))
        return true;

      return false;
    }
  }
}
