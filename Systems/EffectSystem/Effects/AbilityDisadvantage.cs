using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string StrengthDisadvantageEffectTag = "_DISADVANTAGE_STRENGTH_EFFECT";
    public static Effect strengthDisadvantage
    {
      get
      {
        Effect eff = Effect.RunAction();
        eff.Tag = StrengthDisadvantageEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }

    public const string DexterityDisadvantageEffectTag = "_DISADVANTAGE_DEXTERITY_EFFECT";
    public static Effect dexterityDisadvantage
    {
      get
      {
        Effect eff = Effect.RunAction();
        eff.Tag = DexterityDisadvantageEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }

    public const string ConstitutionDisadvantageEffectTag = "_DISADVANTAGE_CONSTITUTION_EFFECT";
    public static Effect constitutionDisadvantage
    {
      get
      {
        Effect eff = Effect.RunAction();
        eff.Tag = ConstitutionDisadvantageEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }

    public const string IntelligenceDisadvantageEffectTag = "_DISADVANTAGE_INTELLIGENCE_EFFECT";
    public static Effect intelligenceDisadvantage
    {
      get
      {
        Effect eff = Effect.RunAction();
        eff.Tag = IntelligenceDisadvantageEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }

    public const string WisdomDisadvantageEffectTag = "_DISADVANTAGE_WISDOM_EFFECT";
    public static Effect wisdomDisadvantage
    {
      get
      {
        Effect eff = Effect.RunAction();
        eff.Tag = WisdomDisadvantageEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }

    public const string CharismaDisadvantageEffectTag = "_DISADVANTAGE_CHARISMA_EFFECT";
    public static Effect charismaDisadvantage
    {
      get
      {
        Effect eff = Effect.RunAction();
        eff.Tag = CharismaDisadvantageEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
  }
}
