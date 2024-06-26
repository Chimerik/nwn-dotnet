﻿using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string SprintEffectTag = "_EFFECT_SPRINT";
    public const string SprintMobileEffectTag = "_EFFECT_SPRINT_MOBILE";
    public static Effect sprintEffect
    {
      get
      {
        Effect eff = Effect.LinkEffects(Effect.MovementSpeedIncrease(50), Effect.Icon(NwGameTables.EffectIconTable.GetRow(142)));
        eff.Tag = SprintEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
    public static Effect sprintMobileEffect
    {
      get
      {
        Effect eff = Effect.LinkEffects(Effect.Immunity(ImmunityType.Entangle), Effect.Immunity(ImmunityType.Slow));
        eff.Tag = SprintMobileEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
  }
}
