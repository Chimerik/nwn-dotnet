using System.Linq;
using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;
using NWN.Core;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    private static ScriptCallbackHandle onEnterWildMagicCroissanceVegetaleCallback;
    private static ScriptCallbackHandle onExitWildMagicCroissanceVegetaleCallback;
    public const string WildMagicCroissanceVegetaleAuraEffectTag = "_EFFECT_WILD_MAGIC_CROISSANCE_VEGETALE_AURA";
    public const string WildMagicCroissanceVegetaleEffectTag = "_EFFECT_WILD_MAGIC_CROISSANCE_VEGETALE";
    public static Effect WildMagicCroissanceVegetaleAura
    {
      get
      {
        Effect eff = Effect.LinkEffects(Effect.VisualEffect(VfxType.DurAuraGreenDark), Effect.AreaOfEffect(PersistentVfxType.PerEntangle, onEnterWildMagicCroissanceVegetaleCallback, onExitHandle:onExitWildMagicCroissanceVegetaleCallback));
        eff.Tag = WildMagicCroissanceVegetaleAuraEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
    public static Effect WildMagicCroissanceVegetale
    {
      get
      {
        Effect eff = Effect.MovementSpeedDecrease(50);
        eff.Tag = WildMagicCroissanceVegetaleEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
    private static ScriptHandleResult onEnterWildMagicCroissanceVegetale(CallInfo callInfo)
    {
      if (!callInfo.TryGetEvent(out AreaOfEffectEvents.OnEnter eventData) || eventData.Effect.Creator == eventData.Entering
        || eventData.Entering is not NwCreature entering || entering.ActiveEffects.Any(e => e.Tag == WildMagicCroissanceVegetaleEffectTag))
        return ScriptHandleResult.Handled;

      NWScript.AssignCommand(eventData.Effect.Creator, () => entering.ApplyEffect(EffectDuration.Permanent, WildMagicCroissanceVegetale));

      return ScriptHandleResult.Handled;
    }
    private static ScriptHandleResult onExitWildMagicCroissanceVegetale(CallInfo callInfo)
    {
      if (!callInfo.TryGetEvent(out AreaOfEffectEvents.OnExit eventData) || eventData.Exiting is not NwCreature exiting)
        return ScriptHandleResult.Handled;

      ModuleSystem.Log.Info($"exiting : {exiting.Name}");
      foreach(var eff in exiting.ActiveEffects)
        ModuleSystem.Log.Info($"eff : {eff.Tag}");

      EffectUtils.RemoveTaggedEffect(exiting, eventData.Effect.Creator, WildMagicCroissanceVegetaleEffectTag);

      return ScriptHandleResult.Handled;
    }
  }
}
