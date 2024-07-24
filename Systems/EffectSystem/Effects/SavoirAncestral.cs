using Anvil.API;
namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string SavoirAncestralDexteriteEffectTag = "_SAVOIR_ANCESTRAL_DEXTERITE_EFFECT";
    public const string SavoirAncestralConstitutionEffectTag = "_SAVOIR_ANCESTRAL_CONSTITUTION_EFFECT";
    public const string SavoirAncestralIntelligenceEffectTag = "_SAVOIR_ANCESTRAL_INTELLIGENCE_EFFECT";
    public const string SavoirAncestralSagesseEffectTag = "_SAVOIR_ANCESTRAL_SAGESSE_EFFECT";
    public const string SavoirAncestralCharismeEffectTag = "_SAVOIR_ANCESTRAL_CHARISME_EFFECT";
    public static Effect SavoirAncestral(Ability ability)
    {
      Effect eff = Effect.Icon(EffectIcon.SkillIncrease);

      eff.Tag = ability switch
      {
        Ability.Dexterity => SavoirAncestralDexteriteEffectTag,
        Ability.Constitution => SavoirAncestralConstitutionEffectTag,
        Ability.Intelligence => SavoirAncestralIntelligenceEffectTag,
        Ability.Charisma => SavoirAncestralCharismeEffectTag,
        _ => SavoirAncestralSagesseEffectTag
      };

      eff.SubType = EffectSubType.Supernatural;
      return eff;
    }
  }
}
