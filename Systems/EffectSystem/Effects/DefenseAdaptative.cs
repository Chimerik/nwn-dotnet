using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string DefenseAdaptativeEffectTag = "_DEFENSE_ADAPTATIVE_EFFECT";
    public static readonly Native.API.CExoString DefenseAdaptativeEffectExoTag = DefenseAdaptativeEffectTag.ToExoString();
    public static Effect DefenseAdaptative
    {
      get
      {
        Effect eff = Effect.Icon(EffectIcon.ACIncrease);
        eff.Tag = DefenseAdaptativeEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
  }
}
