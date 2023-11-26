using Anvil.API;
namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string MaitreArmureIntermediaireEffectTag = "_MAITRE_ARMURE_INTERMEDIAIRE_EFFECT";
    public static Effect maitreArmureIntermediaire
    {
      get
      {
        Effect eff = Effect.LinkEffects(Effect.ACIncrease(1, ACBonus.Dodge), Effect.Icon((EffectIcon)157));
        eff.Tag = MaitreArmureIntermediaireEffectTag;
        eff.SubType = EffectSubType.Unyielding;
        return eff;
      }
    }
  }
}
