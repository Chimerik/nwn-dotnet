﻿using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string JeuDeJambeEffectTag = "_JEU_DE_JAMBE_EFFECT";
    public static Effect jeuDeJambe
    {
      get
      {
        Effect eff = Effect.Icon(CustomEffectIcon.JeuDeJambe);
        eff.Tag = JeuDeJambeEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
  }
}
