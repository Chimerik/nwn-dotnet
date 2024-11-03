using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string AasimarResistanceEffectTag = "_AASIMAR_RESISTANCE_EFFECT";
    public static Effect AasimarResistance
    {
      get
      {
        Effect divine = Effect.DamageImmunityIncrease(DamageType.Divine, 50);
        divine.ShowIcon = false;

        Effect necrotic = Effect.DamageImmunityIncrease(CustomDamageType.Necrotic, 50);
        necrotic.ShowIcon = false; 

        Effect link = Effect.LinkEffects(divine, necrotic, Effect.Icon((EffectIcon)207), Effect.Icon((EffectIcon)215));

        link.Tag = AasimarResistanceEffectTag;
        link.SubType = EffectSubType.Unyielding;

        return link;
      }
    }
  }
}
