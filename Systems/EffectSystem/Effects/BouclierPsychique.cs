using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string BouclierPsychiqueEffectTag = "_BOUCLIER_PSYCHIQUE_EFFECT";
    public static Effect BouclierPsychique
    {
      get
      {
        Effect eff = Effect.DamageImmunityIncrease(CustomDamageType.Psychic, 50);
        eff.ShowIcon = false;

        eff = Effect.LinkEffects(eff, Effect.Icon(DamageType2da.damageResistanceEffectIcon[CustomDamageType.Psychic]));
        eff.Tag = BouclierPsychiqueEffectTag;
        eff.SubType = EffectSubType.Unyielding;

        return eff;
      }
    }
  }
}

