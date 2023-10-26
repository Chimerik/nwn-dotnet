using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public static Effect noReactions;
    public const string noReactionsEffectTag = "_NO_REACTIONS_EFFECT";

    public static void InitNoReactionsEffect()
    {
      noReactions = Effect.LinkEffects(Effect.RunAction(), Effect.Icon(NwGameTables.EffectIconTable.GetRow(147)));
      noReactions.Tag = noReactionsEffectTag;
      noReactions.SubType = EffectSubType.Supernatural;
    }
  }
}
