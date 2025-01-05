using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string AideEffectTag = "_AIDE_EFFECT";
    private static ScriptCallbackHandle onRemoveAideCallback;
    public static Effect Aide
    {
      get
      {
        Effect eff = Effect.LinkEffects(Effect.Icon(CustomEffectIcon.Aide), Effect.RunAction(onRemovedHandle: onRemoveAideCallback));
        eff.Tag = AideEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
    private static ScriptHandleResult OnRemoveAide(CallInfo callInfo)
    {
      EffectRunScriptEvent eventData = new EffectRunScriptEvent();

      if (eventData.EffectTarget is NwCreature creature)
      {
        if (creature.IsLoginPlayerCharacter)
        {
          creature.LevelInfo[0].HitDie -= 5;
        }
        else
        {
          creature.MaxHP -= 5;
        }
      }

      return ScriptHandleResult.Handled;
    }
  }
}
