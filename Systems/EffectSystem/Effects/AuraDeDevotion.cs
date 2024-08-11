using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;
using NWN.Core;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string AuraDeDevotionEffectTag = "_AURA_DE_DEVOTION_EFFECT";
    public const string DevotionEffectTag = "_DEVOTION_EFFECT";
    private static ScriptCallbackHandle onEnterAuraDeDevotionCallback;
    private static ScriptCallbackHandle onExitAuraDeDevotionCallback;
    public static Effect GetAuraDeDevotion(NwCreature caster, int paladinLevel)
    {
      Effect eff = Effect.LinkEffects(Effect.VisualEffect(VfxType.DurAuraSilence, fScale: paladinLevel < 18 ? 0.9f : 1.8f),
        Effect.AreaOfEffect(PersistentVfxType.PerCustomAoe, onEnterHandle: onEnterAuraDeDevotionCallback, onExitHandle: onExitAuraDeDevotionCallback));
      eff.Tag = AuraDeDevotionEffectTag;
      eff.SubType = EffectSubType.Unyielding;
      eff.Creator = caster;
      return eff;
    }
    private static ScriptHandleResult onEnterDevotionAura(CallInfo callInfo)
    {
      if (!callInfo.TryGetEvent(out AreaOfEffectEvents.OnEnter eventData) || eventData.Entering is not NwCreature entering 
        || eventData.Effect.Creator is not NwCreature protector || protector.HP < 1
        || entering.IsReactionTypeHostile(protector))
        return ScriptHandleResult.Handled;

      NWScript.AssignCommand(protector, () => entering.ApplyEffect(EffectDuration.Permanent, GetCharmImmunityEffect(DevotionEffectTag)));
      return ScriptHandleResult.Handled;
    }
    private static ScriptHandleResult onExitDevotionAura(CallInfo callInfo)
    {
      if (!callInfo.TryGetEvent(out AreaOfEffectEvents.OnExit eventData) || eventData.Exiting is not NwCreature exiting
        || eventData.Effect.Creator is not NwCreature protector || exiting.IsReactionTypeHostile(protector))
        return ScriptHandleResult.Handled;

      EffectUtils.RemoveTaggedEffect(exiting, protector, DevotionEffectTag);
      return ScriptHandleResult.Handled;
    }
  }
}
