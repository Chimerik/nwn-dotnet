using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public static Effect strengthAdvantage;
    public const string StrengthAdvantageEffectTag = "_ADVANTAGE_STRENGTH_EFFECT";
    public static Effect dexterityAdvantage;
    public const string DexterityAdvantageEffectTag = "_ADVANTAGE_DEXTERITY_EFFECT";
    public static Effect constitutionAdvantage;
    public const string ConstitutionAdvantageEffectTag = "_ADVANTAGE_CONSTITUTION_EFFECT";
    public static Effect intelligenceAdvantage;
    public const string IntelligenceAdvantageEffectTag = "_ADVANTAGE_INTELLIGENCE_EFFECT";
    public static Effect wisdomAdvantage;
    public const string WisdomAdvantageEffectTag = "_ADVANTAGE_WISDOM_EFFECT";
    public static Effect charismaAdvantage;
    public const string CharismaAdvantageEffectTag = "_ADVANTAGE_CHARISMA_EFFECT";

    public static void InitAbilityAdvantageEffect()
    {
      strengthAdvantage = Effect.RunAction();
      strengthAdvantage.Tag = StrengthAdvantageEffectTag;
      strengthAdvantage.SubType = EffectSubType.Supernatural;

      dexterityAdvantage = Effect.RunAction();
      dexterityAdvantage.Tag = DexterityAdvantageEffectTag;
      dexterityAdvantage.SubType = EffectSubType.Supernatural;

      constitutionAdvantage = Effect.RunAction();
      constitutionAdvantage.Tag = ConstitutionAdvantageEffectTag;
      constitutionAdvantage.SubType = EffectSubType.Supernatural;

      intelligenceAdvantage = Effect.RunAction();
      intelligenceAdvantage.Tag = IntelligenceAdvantageEffectTag;
      intelligenceAdvantage.SubType = EffectSubType.Supernatural;

      wisdomAdvantage = Effect.RunAction();
      wisdomAdvantage.Tag = WisdomAdvantageEffectTag;
      wisdomAdvantage.SubType = EffectSubType.Supernatural;

      charismaAdvantage = Effect.RunAction();
      charismaAdvantage.Tag = CharismaAdvantageEffectTag;
      charismaAdvantage.SubType = EffectSubType.Supernatural;
    }
  }
}
