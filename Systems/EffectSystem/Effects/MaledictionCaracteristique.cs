using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string MaledictionForceEffectTag = "_MALEDICTION_FORCE_EFFECT";
    public const string MaledictionDexteriteEffectTag = "_MALEDICTION_DEXTERITE_EFFECT";
    public const string MaledictionConstitutionEffectTag = "_MALEDICTION_CONSTITUTION_EFFECT";
    public const string MaledictionIntelligenceEffectTag  = "_MALEDICTION_INTELLIGENCE_EFFECT";
    public const string MaledictionSagesseEffectTag = "_MALEDICTION_SAGESSE_EFFECT";
    public static Effect GetMaledictionCaracteristique(int spellId)
    {
      Effect eff = Effect.Icon(EffectIcon.Curse);
      eff.Tag = spellId switch
      {
        CustomSpell.MaledictionForce => MaledictionForceEffectTag,
        CustomSpell.MaledictionDexterite => MaledictionDexteriteEffectTag,
        CustomSpell.MaledictionConstitution => MaledictionConstitutionEffectTag,
        CustomSpell.MaledictionIntelligence => MaledictionIntelligenceEffectTag,
        CustomSpell.MaledictionSagesse => MaledictionSagesseEffectTag,
        _ => MaledictionForceEffectTag,
      };
      eff.SubType = EffectSubType.Supernatural;
      return eff;
    }
  }
}
