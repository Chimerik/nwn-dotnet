using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string EspritEveilleEffectTag = "_ESPRIT_EVEILLE_EFFECT";
    public const string EspritEveilleDisadvantageEffectTag = "_ESPRIT_EVEILLE_DISADVANTAGE_EFFECT";
    public static readonly Native.API.CExoString EspritEveilleEffectExoTag = EspritEveilleDisadvantageEffectTag.ToExoString();
    public static Effect EspritEveille
    {
      get
      {
        Effect eff = Effect.Icon((EffectIcon)221);
        eff.Tag = EspritEveilleEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
    public static Effect EspritEveilleDisadvantage
    {
      get
      {
        Effect eff = Effect.RunAction();
        eff.Tag = EspritEveilleDisadvantageEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
  }
}

