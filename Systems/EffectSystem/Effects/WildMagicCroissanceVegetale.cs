using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;

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
        Effect eff = Effect.LinkEffects(Effect.Icon(CustomEffectIcon.WildMagicCroissanceVegetale), Effect.VisualEffect(VfxType.DurAuraGreenDark), Effect.VisualEffect(VfxType.DurEntangle),
          Effect.AreaOfEffect(PersistentVfxType.PerCustomAoe, onEnterWildMagicCroissanceVegetaleCallback, onExitHandle:onExitWildMagicCroissanceVegetaleCallback));
        eff.Tag = WildMagicCroissanceVegetaleAuraEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
    private static ScriptHandleResult onEnterWildMagicCroissanceVegetale(CallInfo callInfo)
    {
      if (callInfo.TryGetEvent(out AreaOfEffectEvents.OnEnter eventData) && eventData.Effect.Creator is NwCreature caster 
        && eventData.Entering is NwCreature entering && caster != entering && caster.IsReactionTypeHostile(entering))
          ApplyTerrainDifficileEffect(entering, caster, NwSpell.FromSpellId(CustomSpell.CroissanceVegetale));

      return ScriptHandleResult.Handled;
    }
    private static ScriptHandleResult onExitWildMagicCroissanceVegetale(CallInfo callInfo)
    {
      if (callInfo.TryGetEvent(out AreaOfEffectEvents.OnExit eventData) && eventData.Exiting is NwCreature exiting)
        EffectUtils.RemoveTaggedEffect(exiting, eventData.Effect.Creator, CustomSpell.CroissanceVegetale, TerrainDifficileEffectTag);

      return ScriptHandleResult.Handled;
    }
  }
}
