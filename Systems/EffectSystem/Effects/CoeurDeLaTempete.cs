using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string CoeurDeLaTempeteEffectTag = "_COEUR_DE_LA_TEMPETE_EFFECT";
    public static Effect CoeurDeLaTempete
    {
      get
      {
        Effect eff = Effect.LinkEffects(Effect.DamageImmunityIncrease(DamageType.Electrical, 50), 
          Effect.DamageImmunityIncrease(DamageType.Sonic, 50));
        eff.Tag = CoeurDeLaTempeteEffectTag;
        eff.SubType = EffectSubType.Unyielding;
        return eff;
      }
    }
  }
}
