using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string ConstitutionInfernaleEffectTag = "_CONSTITUTION_INFERNALE_EFFECT";
    public static Effect ConstitutionInfernale
    {
      get
      {
        Effect eff = Effect.LinkEffects(Effect.DamageImmunityIncrease(CustomDamageType.Poison, 50),
          Effect.DamageImmunityIncrease(DamageType.Cold, 50));
        eff.Tag = ConstitutionInfernaleEffectTag;
        eff.SubType = EffectSubType.Unyielding;
        return eff;
      }
    }
  }
}
