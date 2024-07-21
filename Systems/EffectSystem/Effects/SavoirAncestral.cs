using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string SavoirAncestralEffectTag = "_SAVOIR_ANCESTRAL_EFFECT";
    private static ScriptCallbackHandle onRemoveSavoirAncestralCallback;
    public static Effect SavoirAncestral
    {
      get
      {
        Effect eff = Effect.LinkEffects(Effect.Icon(EffectIcon.SkillIncrease), Effect.RunAction(onRemovedHandle: onRemoveSavoirAncestralCallback));
        eff.Tag = SavoirAncestralEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
    private static ScriptHandleResult OnRemoveSavoirAncestral(CallInfo callInfo)
    {
      EffectRunScriptEvent eventData = new EffectRunScriptEvent();

      if (eventData.EffectTarget is not NwCreature creature)
        return ScriptHandleResult.Handled;

      // SUPPRIMER la maîtrise temporaire. Egalement supprimer la maîtrise à la déco

      return ScriptHandleResult.Handled;
    }
  }
}
