using System.Linq;
using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string CroissanceVegetaleAoEEffectTag = "_CROISSANCE_VEGETALE_AOE_EFFECT";
    public const string CroissanceVegetaleEffectTag = "_CROISSANCE_VEGETALE_EFFECT";
    private static ScriptCallbackHandle onEnterCroissanceVegetaleCallback;
    private static ScriptCallbackHandle onExitCroissanceVegetaleCallback;
    public static Effect CroissanceVegetaleAoE(NwGameObject caster)
    {
      Effect eff = Effect.LinkEffects(Effect.VisualEffect(VfxType.ImpAuraNegativeEnergy, fScale:10),
        Effect.AreaOfEffect(PersistentVfxType.PerCustomAoe, onEnterHandle: onEnterCroissanceVegetaleCallback, onExitHandle: onExitCroissanceVegetaleCallback));
      eff.Tag = CroissanceVegetaleAoEEffectTag;
      eff.SubType = EffectSubType.Supernatural;
      eff.Creator = caster;

      return eff;
    }
    public static Effect CroissanceVegetale
    {
      get
      {
        Effect eff = Effect.MovementSpeedDecrease(75);
        eff.Tag = CroissanceVegetaleEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
    private static ScriptHandleResult onEnterCroissanceVegetale(CallInfo callInfo)
    {
      if (!callInfo.TryGetEvent(out AreaOfEffectEvents.OnEnter eventData) || eventData.Entering is not NwCreature entering
        || entering.ActiveEffects.Any(e => e.Tag == CroissanceVegetaleEffectTag))
        return ScriptHandleResult.Handled;

      entering.ApplyEffect(EffectDuration.Permanent, CroissanceVegetale);

      return ScriptHandleResult.Handled;
    }
    private static ScriptHandleResult onExitCroissanceVegetale(CallInfo callInfo)
    {
      if (!callInfo.TryGetEvent(out AreaOfEffectEvents.OnExit eventData) || eventData.Exiting is not NwCreature exiting)
        return ScriptHandleResult.Handled;

      EffectUtils.RemoveTaggedEffect(exiting, eventData.Effect.Creator, CroissanceVegetaleEffectTag);

      return ScriptHandleResult.Handled;
    }
  }
}
