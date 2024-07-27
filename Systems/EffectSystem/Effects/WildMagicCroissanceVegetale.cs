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
    private static ScriptHandleResult onEnterWildMagicCroissanceVegetale(CallInfo callInfo)
    {
      if (callInfo.TryGetEvent(out AreaOfEffectEvents.OnEnter eventData) && eventData.Effect.Creator != eventData.Entering
        && eventData.Entering is NwCreature entering)
          ApplyTerrainDifficileEffect(entering);

      return ScriptHandleResult.Handled;
    }
    private static ScriptHandleResult onExitWildMagicCroissanceVegetale(CallInfo callInfo)
    {
      if (callInfo.TryGetEvent(out AreaOfEffectEvents.OnExit eventData) && eventData.Exiting is NwCreature exiting)
        EffectUtils.RemoveTaggedEffect(exiting, TerrainDifficileEffectTag);

      return ScriptHandleResult.Handled;
    }
  }
}
