﻿using Anvil.API;
using EffectSubType = Anvil.API.EffectSubType;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string ChatimentOcculteEffectTag = "_CHATIMENT_OCCULTE_EFFECT";
    public static Effect ChatimentOcculte
    {
      get
      {
        Effect eff = Effect.LinkEffects(Effect.Icon((EffectIcon)192), Effect.RunAction());
        eff.Tag = ChatimentOcculteEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
  }
}
