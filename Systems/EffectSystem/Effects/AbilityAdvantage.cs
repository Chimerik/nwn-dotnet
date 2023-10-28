using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string StrengthAdvantageEffectTag = "_ADVANTAGE_STRENGTH_EFFECT";
    public static Effect strengthAdvantage 
    { 
      get
      {
        Effect eff = Effect.RunAction();
        eff.Tag = StrengthAdvantageEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }

    public const string DexterityAdvantageEffectTag = "_ADVANTAGE_DEXTERITY_EFFECT";
    public static Effect dexterityAdvantage
    {
      get
      {
        Effect eff = Effect.RunAction();
        eff.Tag = DexterityAdvantageEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }

    public const string ConstitutionAdvantageEffectTag = "_ADVANTAGE_CONSTITUTION_EFFECT";
    public static Effect constitutionAdvantage
    {
      get
      {
        Effect eff = Effect.RunAction();
        eff.Tag = ConstitutionAdvantageEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }

    public const string IntelligenceAdvantageEffectTag = "_ADVANTAGE_INTELLIGENCE_EFFECT";
    public static Effect intelligenceAdvantage
    {
      get
      {
        Effect eff = Effect.RunAction();
        eff.Tag = IntelligenceAdvantageEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }

    public const string WisdomAdvantageEffectTag = "_ADVANTAGE_WISDOM_EFFECT";
    public static Effect wisdomAdvantage
    {
      get
      {
        Effect eff = Effect.RunAction();
        eff.Tag = WisdomAdvantageEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }

    public const string CharismaAdvantageEffectTag = "_ADVANTAGE_CHARISMA_EFFECT";
    public static Effect charismaAdvantage
    {
      get
      {
        Effect eff = Effect.RunAction();
        eff.Tag = CharismaAdvantageEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
  }
}
