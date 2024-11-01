using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string CharmeImmuniteEffectTag = "_CHARME_IMMUNE_EFFECT";
    public static Effect CharmeImmunite
    {
      get
      {
        Effect eff = Effect.Immunity(ImmunityType.Charm);
        eff.Tag = CharmeImmuniteEffectTag;
        eff.SubType = EffectSubType.Unyielding;
        return eff;
      }
    }
  }
}

