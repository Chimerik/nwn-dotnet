using System.Linq;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static bool In(this EffectTrueType me, params EffectTrueType[] set)
    {
      return set.Contains(me);
    }
  }
}
