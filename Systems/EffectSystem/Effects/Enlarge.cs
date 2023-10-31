using System;
using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string EnlargeEffectTag = "_EFFECT_ENLARGE";
    public static Effect enlargeEffect
    {
      get
      {
        Effect eff = Effect.LinkEffects(Effect.DamageIncrease(2, DamageType.BaseWeapon), Effect.VisualEffect(VfxType.DurCessatePositive));
        eff.Tag = EnlargeEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
  }
}
