﻿using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string ThieflingResistanceEffectTag = "_THIEFLING_FIRE_RESISTANCE_EFFECT";
    public static Effect ThieflingFireResistance
    {
      get
      {
        Effect eff = Effect.DamageImmunityIncrease(DamageType.Fire, 50);
        eff.Tag = ThieflingResistanceEffectTag;
        eff.SubType = EffectSubType.Unyielding;
        return eff;
      }
    }
  }
}
