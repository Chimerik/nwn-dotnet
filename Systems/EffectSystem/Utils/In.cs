using System;
using System.Linq;

namespace NWN
{
  public static partial class EffectUtils
  {
    public static bool In(this string me, params string[] set)
    {
      return set.Contains(me);
    }
  }
}
