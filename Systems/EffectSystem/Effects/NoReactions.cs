using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public static Effect noReactions
    {
      get
      {
        Effect eff = Effect.Icon(CustomEffectIcon.ElectricJolt);
        eff.Tag = NoReactionsEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
    public const string NoReactionsEffectTag = "_NO_REACTIONS_EFFECT";
  }
}
