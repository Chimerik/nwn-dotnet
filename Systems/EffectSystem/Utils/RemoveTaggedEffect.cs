using System;
using System.Collections.Generic;
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

    public static async void RemoveTaggedEffect(NwGameObject target, double delay, params string[] effectTag)
    {
      await NwTask.Delay(TimeSpan.FromSeconds(delay));

      foreach (var eff in target.ActiveEffects)
        if (effectTag.Contains(eff.Tag))
          target.RemoveEffect(eff);
    }

    public static void RemoveTaggedParamEffect(NwGameObject target, int param, params string[] effectTag)
    {
      foreach (var eff in target.ActiveEffects)
        if (effectTag.Contains(eff.Tag) && eff.IntParams[5] == param)
          target.RemoveEffect(eff);
    }
    public static void RemoveTaggedSpellEffect(NwGameObject target, string effectTag, params NwSpell[] param)
    {
      foreach (var eff in target.ActiveEffects)
        if (eff.Tag == effectTag && param.Contains(eff.Spell))
          target.RemoveEffect(eff);
    }

    public static void RemoveTaggedEffect(NwGameObject target, NwObject creator, params string[] effectTag)
    {
      foreach (var eff in target.ActiveEffects)
        if (creator == eff.Creator && effectTag.Contains(eff.Tag))
          target.RemoveEffect(eff);
    }

    public static void RemoveTaggedEffect(NwGameObject target, NwObject creator, int spellId, params string[] effectTag)
    {
      foreach (var eff in target.ActiveEffects)
        if (creator == eff.Creator && effectTag.Contains(eff.Tag) && eff.Spell?.Id == spellId)
          target.RemoveEffect(eff);
    }
    /*public static void RemoveTaggedEffect(CNWSObject target, params CExoString[] effectTag)
    {
      List<CGameEffect> effToRemove = new();

      foreach (var eff in target.m_appliedEffects)
        foreach(var tag in effectTag)
          if (eff.m_sCustomTag.CompareNoCase(tag).ToBool())
            effToRemove.Add(eff);

      foreach (var eff in effToRemove)
        target.RemoveEffect(eff);
    }*/
    public static void RemoveTaggedNativeEffect(CNWSObject target, params string[] effectTag)
    {
      List<CGameEffect> effToRemove = new();

      foreach (var eff in target.m_appliedEffects)
        foreach (var tag in effectTag)
          if (eff.m_sCustomTag.ToString() == tag)
            effToRemove.Add(eff);

      foreach (var eff in effToRemove)
        target.RemoveEffect(eff);
    }
  }
}
