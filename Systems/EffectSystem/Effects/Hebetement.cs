using Anvil.API;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string HebetementEffectTag = "_HEBETEMENT_EFFECT";
    public static readonly Native.API.CExoString HebetementEffectExoTag = HebetementEffectTag.ToExoString();
    public static Effect Hebetement(NwCreature target)
    {
      target.GetObjectVariable<LocalVariableInt>(CreatureUtils.ReactionVariable).Value = 0;

      Effect eff = Effect.LinkEffects(Effect.Icon(EffectIcon.Dazed), Effect.VisualEffect(VfxType.DurMindAffectingNegative), noReactions);
      eff.Tag = HebetementEffectTag;
      eff.SubType = EffectSubType.Supernatural;
      return eff;
    }
  }
}

