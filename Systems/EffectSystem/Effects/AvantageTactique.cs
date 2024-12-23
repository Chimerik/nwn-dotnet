﻿using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string AvantageTactiqueEffectTag = "_AVANTAGE_TACTIQUE_EFFECT";
    public static Effect AvantageTactique
    {
      get
      {
        Effect eff = Effect.MovementSpeedIncrease(25);
        eff.Tag = AvantageTactiqueEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
  }
}

