using System;
using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;
using DamageType = Anvil.API.DamageType;
using EffectSubType = Anvil.API.EffectSubType;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string ManifestationAmeEffectTag = "_MANIFESTATION_AME_EFFECT";
    private static ScriptCallbackHandle onIntervalManifestationCallback;
    public static Effect GetMonkManifestationAmeEffect(int wisdomModifier)
    {
      Effect eff = Effect.LinkEffects(Effect.DamageIncrease(6, DamageType.Divine), Effect.DamageIncrease(wisdomModifier, DamageType.Divine), 
        Effect.RunAction(onIntervalHandle: onIntervalManifestationCallback, interval: TimeSpan.FromSeconds(2)));
      eff.Tag = ManifestationAmeEffectTag;
      eff.SubType = EffectSubType.Unyielding;
      return eff;
    }
    private static ScriptHandleResult OnIntervalManifestation(CallInfo callInfo)
    {
      EffectRunScriptEvent eventData = new EffectRunScriptEvent();

      if (eventData.EffectTarget is not NwCreature target)
        return ScriptHandleResult.Handled;

      if (target.GetItemInSlot(InventorySlot.RightHand) is not null)
        EffectUtils.RemoveTaggedEffect(target, ManifestationAmeEffectTag, ManifestationCorpsEffectTag, ManifestationEspritEffectTag);

        return ScriptHandleResult.Handled;
    }
  }
}
