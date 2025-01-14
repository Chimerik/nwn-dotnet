﻿using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string BroyeurEffectTag = "_BROYEUR_EFFECT";
    public static Effect BroyeurEffect
    {
      get
      {
        Effect eff = Effect.Icon((EffectIcon)135);
        eff.Tag = BroyeurEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
  }
}
