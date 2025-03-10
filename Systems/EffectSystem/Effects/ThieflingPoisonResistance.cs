﻿using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string ThieflingPoisonResistanceEffectTag = "_THIEFLING_POISON_RESISTANCE_EFFECT";
    public static Effect ThieflingPoisonResistance
    {
      get
      {
        Effect eff = ResistancePoison;
        eff.Tag = ThieflingPoisonResistanceEffectTag;
        eff.SubType = EffectSubType.Unyielding;
        return eff;
      }
    }
  }
}
