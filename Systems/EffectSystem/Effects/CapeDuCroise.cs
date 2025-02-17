using System.Linq;
using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;
using NWN.Core;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string CapeDuCroiseAuraEffectTag = "_CAPE_DU_CROISE_AURA_EFFECT";
    public const string CapeDuCroiseEffectTag = "_CAPE_DU_CROISE_EFFECT";
    private static ScriptCallbackHandle onEnterCapeDuCroiseCallback;
    private static ScriptCallbackHandle onExitCapeDuCroiseCallback;
    public static Effect CapeDuCroiseAura(NwCreature caster, NwSpell spell)
    {
      Effect eff = Effect.LinkEffects(Effect.VisualEffect(VfxType.DurAuraSilence, fScale: 2),
        Effect.LinkEffects(Effect.AreaOfEffect(PersistentVfxType.PerCustomAoe, onEnterCapeDuCroiseCallback, onExitHandle: onExitCapeDuCroiseCallback)));
      eff.Tag = CapeDuCroiseAuraEffectTag;
      eff.SubType = EffectSubType.Supernatural;
      eff.Creator = caster;
      eff.Spell = spell;
      return eff;
    }
    public static Effect CapeDuCroise
    {
      get
      {
        Effect eff = Effect.DamageIncrease((int)DamageBonus.Plus1d4, DamageType.Divine);
        eff.Tag = CapeDuCroiseEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
    private static ScriptHandleResult onEnterCapeDuCroise(CallInfo callInfo)
    {
      if (!callInfo.TryGetEvent(out AreaOfEffectEvents.OnEnter eventData) || eventData.Entering is not NwCreature entering 
        || eventData.Effect.Creator is not NwCreature protector || !entering.IsReactionTypeHostile(protector))
        return ScriptHandleResult.Handled;

      if (!entering.ActiveEffects.Any(e => e.Tag == CapeDuCroiseEffectTag))
      {
        NWScript.AssignCommand(protector, () => entering.ApplyEffect(EffectDuration.Permanent, CapeDuCroise));
        entering.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpHolyAid));
      }

      return ScriptHandleResult.Handled;
    }
    private static ScriptHandleResult onExitCapeDuCroise(CallInfo callInfo)
    {
      if (!callInfo.TryGetEvent(out AreaOfEffectEvents.OnExit eventData) || eventData.Exiting is not NwCreature exiting
        || eventData.Effect.Creator is not NwCreature protector)
        return ScriptHandleResult.Handled;

      EffectUtils.RemoveTaggedEffect(exiting, protector, CapeDuCroiseEffectTag);
      return ScriptHandleResult.Handled;
    }
  }
}
