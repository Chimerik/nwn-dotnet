using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string SentinelleImmortelleEffectTag = "-SENTINELLE_IMMORTELLE_EFFECT";
    public const string SentinelleImmortelleVariable = "_SENTINELLE_IMMORTELLE";
    private static ScriptCallbackHandle onRemoveSentinelleImmortelleCallback;

    public static Effect SentinelleImmortelle
    {
      get
      {
        Effect eff = Effect.LinkEffects(Effect.Icon(CustomEffectIcon.SentinelleImmortelle), Effect.RunAction(onRemovedHandle: onRemoveSentinelleImmortelleCallback));
        eff.Tag = SentinelleImmortelleEffectTag;
        eff.SubType = EffectSubType.Unyielding;
        return eff;
      }
    }

    private static ScriptHandleResult OnRemoveSentinelleImmortelle(CallInfo callInfo)
    {
      EffectRunScriptEvent eventData = new EffectRunScriptEvent();

      if (eventData.EffectTarget is NwCreature creature)
      {
        creature.Immortal = false;
      }

      return ScriptHandleResult.Handled;
    }
  }
}
