using System;
using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    private static ScriptCallbackHandle onIntervalWildMagicCroissanceVegetaleCallback;
    public const string WildMagicCroissanceVegetaleEffectTag = "_EFFECT_WILD_MAGIC_CROISSANCE_VEGETALE";
    public static Effect wildMagicCroissanceVegetale
    {
      get
      {
        Effect eff = Effect.LinkEffects(Effect.VisualEffect(VfxType.DurAuraGreenDark), Effect.RunAction(onIntervalHandle: onIntervalWildMagicCroissanceVegetaleCallback, interval: NwTimeSpan.FromRounds(1)));
        eff.Tag = WildMagicCroissanceVegetaleEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
    private static ScriptHandleResult OnIntervalWildMagicCroissanceVegetale(CallInfo callInfo)
    {
      EffectRunScriptEvent eventData = new EffectRunScriptEvent();

      if (eventData.EffectTarget is not NwCreature source)
        return ScriptHandleResult.Handled;

      source?.Location.ApplyEffect(EffectDuration.Temporary, Effect.AreaOfEffect(PersistentVfxType.PerEntangle), NwTimeSpan.FromRounds(1));

      return ScriptHandleResult.Handled;
    }
  }
}
