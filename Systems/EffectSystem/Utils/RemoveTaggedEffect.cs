﻿using System.Collections.Generic;
using System.Linq;
using Anvil.API;
using NWN.Native.API;
using NWN.Systems;

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
    public static void RemoveTaggedEffect(CNWSObject target, CExoString effectTag)
    {
      List<CGameEffect> effToRemove = new();

      foreach (var eff in target.m_appliedEffects)
        if (eff.m_sCustomTag.CompareNoCase(effectTag).ToBool())
          effToRemove.Add(eff);

      foreach (var eff in effToRemove)
        target.RemoveEffect(eff);
    }
  }
}
