using System;
using System.Linq;
using NWN.Native.API;

namespace NWN
{
  public static partial class EffectUtils
  {
    public static bool In(this string me, params string[] set)
    {
      return set.Contains(me);
    }
    public static bool In(this EffectTrueType me, params EffectTrueType[] set)
    {
      return set.Contains(me);
    }
  }
}
