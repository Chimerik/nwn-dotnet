using System.Collections.Generic;
using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string FireAffinityEffectTag = "_FIRE_AFFINITY_EFFECT";
    public static Effect FireAffinity
    {
      get
      {
        Effect resist = Effect.DamageImmunityIncrease(DamageType.Fire, 50);
        resist.ShowIcon = false;

        Effect eff = Effect.LinkEffects(resist, Effect.Icon(CustomEffectIcon.FireResistance));
        eff.Tag = FireAffinityEffectTag;
        eff.SubType = EffectSubType.Unyielding;
        return eff;
      }
    }
  }
}
