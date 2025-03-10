﻿using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public static Effect SentinelleImmortelle
    {
      get
      {
        Effect eff = Effect.Icon(CustomEffectIcon.SentinelleImmortelle);
        eff.Tag = SentinelleImmortelleEffectTag;
        eff.SubType = EffectSubType.Unyielding;
        return eff;
      }
    }
    public const string SentinelleImmortelleEffectTag = "-SENTINELLE_IMMORTELLE_EFFECT";
    public const string SentinelleImmortelleVariable = "_SENTINELLE_IMMORTELLE";
  }
}
