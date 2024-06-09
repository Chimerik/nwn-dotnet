using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string PoisonWandererEffectTag = "_POISON_WANDERER_EFFECT";
    public static Effect PoisonWanderer
    {
      get
      {
        Effect eff = Effect.DamageImmunityIncrease(CustomDamageType.Poison, 50);
        eff.ShowIcon = false;
        Effect link = Effect.LinkEffects(eff, Effect.Icon((EffectIcon)174));
        link.Tag = PoisonWandererEffectTag;
        link.SubType = EffectSubType.Unyielding;
        return link;
      }
    }
  }
}
