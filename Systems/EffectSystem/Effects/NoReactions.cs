using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public static Effect noReactions
    {
      get
      {
        Effect eff = Effect.LinkEffects(Effect.RunAction(), Effect.Icon(NwGameTables.EffectIconTable.GetRow(147)));
        eff.Tag = noReactionsEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
    public const string noReactionsEffectTag = "_NO_REACTIONS_EFFECT";
  }
}
