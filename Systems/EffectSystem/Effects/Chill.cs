using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string ChillEffectTag = "_CHILL_EFFECT";
    public static Effect Chill
    {
      get
      {
        Effect eff = Effect.LinkEffects(Effect.Icon((EffectIcon)188), Effect.DamageImmunityDecrease(DamageType.Cold, 50), Effect.DamageImmunityIncrease(DamageType.Fire, 50));
        eff.Tag = ChillEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
  }
}
