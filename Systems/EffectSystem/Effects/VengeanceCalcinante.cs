using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;
using NWN.Core;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string VengeanceCalcinanteAuraEffectTag = "_VENGEANCE_CALCINANTE_AURA_EFFECT";
    public const string VengeanceCalcinanteEffectTag = "_VENGEANCE_CALCINANTE_EFFECT";
    private static ScriptCallbackHandle onEnterVengeanceCalcinanteCallback;
    private static ScriptCallbackHandle onExitVengeanceCalcinanteCallback;
    public static Effect VengeanceCalcinanteAura(NwCreature caster)
    {
      Effect eff = Effect.LinkEffects(Effect.Icon(CustomEffectIcon.VengeanceCalcinante),
        Effect.AreaOfEffect(PersistentVfxType.PerCustomAoe, onEnterHandle: onEnterVengeanceCalcinanteCallback, onExitHandle: onExitVengeanceCalcinanteCallback));
      eff.Tag = VengeanceCalcinanteAuraEffectTag;
      eff.SubType = EffectSubType.Supernatural;
      eff.Creator = caster;
      return eff;
    }
    public static Effect VengeanceCalcinante
    {
      get
      {
        Effect eff = Effect.Icon((EffectIcon)218);
        eff.Tag = VengeanceCalcinanteEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
    private static ScriptHandleResult onEnterVengeanceCalcinante(CallInfo callInfo)
    {
      if (!callInfo.TryGetEvent(out AreaOfEffectEvents.OnEnter eventData) || eventData.Entering is not NwCreature entering 
        || eventData.Effect.Creator is not NwCreature protector || protector.IsReactionTypeHostile(entering))
        return ScriptHandleResult.Handled;

      NWScript.AssignCommand(protector, () => entering.ApplyEffect(EffectDuration.Permanent, VengeanceCalcinante));

      entering.OnDamaged -= OccultisteUtils.OnDamagedVengeanceCalcinante;
      entering.OnDamaged += OccultisteUtils.OnDamagedVengeanceCalcinante;

      return ScriptHandleResult.Handled;
    }
    private static ScriptHandleResult onExitVengeanceCalcinante(CallInfo callInfo)
    {
      if (!callInfo.TryGetEvent(out AreaOfEffectEvents.OnExit eventData) || eventData.Exiting is not NwCreature exiting
        || eventData.Effect.Creator is not NwCreature protector)
        return ScriptHandleResult.Handled;

      EffectUtils.RemoveTaggedEffect(exiting, protector, VengeanceCalcinanteEffectTag);
      exiting.OnDamaged -= OccultisteUtils.OnDamagedVengeanceCalcinante;

      return ScriptHandleResult.Handled;
    }
  }
}
