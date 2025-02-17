using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string NoReactionsEffectTag = "_NO_REACTIONS_EFFECT";

    public static Effect noReactions(NwGameObject target)
    {
      EffectUtils.RemoveTaggedEffect(target, ReactionEffectTag);

      Effect eff = Effect.Icon(CustomEffectIcon.NoReaction);
      eff.Tag = NoReactionsEffectTag;
      eff.SubType = EffectSubType.Supernatural;
      return eff;
    }
  }
}
