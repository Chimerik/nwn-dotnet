using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string HebetementEffectTag = "_HEBETEMENT_EFFECT";

    public static void ApplyHebetement(NwGameObject target)
    {
      Effect eff = Effect.LinkEffects(Effect.VisualEffect(VfxType.DurMindAffectingNegative), Effect.Icon(EffectIcon.Dazed), noReactions(target), Effect.RunAction());
      eff.Tag = HebetementEffectTag;
      eff.SubType = EffectSubType.Supernatural;

      target.ApplyEffect(EffectDuration.Temporary, eff, NwTimeSpan.FromRounds(2));
    }
  }
}

