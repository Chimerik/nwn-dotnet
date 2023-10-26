using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public static Effect strengthDisadvantage;
    public const string StrengthDisadvantageEffectTag = "_DISADVANTAGE_STRENGTH_EFFECT";
    public static Effect dexterityDisadvantage;
    public const string DexterityDisadvantageEffectTag = "_DISADVANTAGE_DEXTERITY_EFFECT";
    public static Effect constitutionDisadvantage;
    public const string ConstitutionDisadvantageEffectTag = "_DISADVANTAGE_CONSTITUTION_EFFECT";
    public static Effect intelligenceDisadvantage;
    public const string IntelligenceDisadvantageEffectTag = "_DISADVANTAGE_INTELLIGENCE_EFFECT";
    public static Effect wisdomDisadvantage;
    public const string WisdomDisadvantageEffectTag = "_DISADVANTAGE_WISDOM_EFFECT";
    public static Effect charismaDisadvantage;
    public const string CharismaDisadvantageEffectTag = "_DISADVANTAGE_CHARISMA_EFFECT";

    public static void InitAbilityDisadvantageEffect()
    {
      strengthDisadvantage = Effect.RunAction();
      strengthDisadvantage.Tag = StrengthDisadvantageEffectTag;
      strengthDisadvantage.SubType = EffectSubType.Supernatural;

      dexterityDisadvantage = Effect.RunAction();
      dexterityDisadvantage.Tag = DexterityDisadvantageEffectTag;
      dexterityDisadvantage.SubType = EffectSubType.Supernatural;

      constitutionDisadvantage = Effect.RunAction();
      constitutionDisadvantage.Tag = ConstitutionDisadvantageEffectTag;
      constitutionDisadvantage.SubType = EffectSubType.Supernatural;

      intelligenceDisadvantage = Effect.RunAction();
      intelligenceDisadvantage.Tag = IntelligenceDisadvantageEffectTag;
      intelligenceDisadvantage.SubType = EffectSubType.Supernatural;

      wisdomDisadvantage = Effect.RunAction();
      wisdomDisadvantage.Tag = WisdomDisadvantageEffectTag;
      wisdomDisadvantage.SubType = EffectSubType.Supernatural;

      charismaDisadvantage = Effect.RunAction();
      charismaDisadvantage.Tag = CharismaDisadvantageEffectTag;
      charismaDisadvantage.SubType = EffectSubType.Supernatural;
    }
  }
}
