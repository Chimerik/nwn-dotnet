using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string HebetementEffectTag = "_HEBETEMENT_EFFECT";

    public static Effect Hebetement(NwCreature target)
    {
      EffectUtils.RemoveTaggedEffect(target, ReactionEffectTag);

      Effect eff = Effect.LinkEffects(Effect.VisualEffect(VfxType.DurMindAffectingNegative), Effect.Icon(EffectIcon.Dazed), noReactions, Effect.RunAction());
      eff.Tag = HebetementEffectTag;
      eff.SubType = EffectSubType.Supernatural;
      return eff;
    }
  }
}

