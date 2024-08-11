using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;
using NWN.Core;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string AuraDeGardeEffectTag = "_AURA_DE_GARDE_EFFECT";
    public const string GardeEffectTag = "_GARDE_EFFECT";
    private static ScriptCallbackHandle onEnterAuraDeGardeCallback;
    private static ScriptCallbackHandle onExitAuraDeGardeCallback;
    public static Effect AuraDeGarde(NwCreature caster, int paladinLevel)
    {
      Effect eff = Effect.LinkEffects(Effect.VisualEffect(VfxType.ImpAuraNegativeEnergy, fScale: paladinLevel < 18 ? 0.9f : 1.8f),
        Effect.AreaOfEffect(PersistentVfxType.PerCustomAoe, onEnterHandle: onEnterAuraDeGardeCallback, onExitHandle: onExitAuraDeGardeCallback));
      eff.Tag = AuraDeGardeEffectTag;
      eff.SubType = EffectSubType.Unyielding;
      eff.Creator = caster;
      return eff;
    }
    public static Effect Garde
    {
      get
      {
        Effect eff = Effect.Icon(EffectIcon.SpellResistanceIncrease);
        eff.Tag = GardeEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
    private static ScriptHandleResult onEnterGardeAura(CallInfo callInfo)
    {
      if (!callInfo.TryGetEvent(out AreaOfEffectEvents.OnEnter eventData) || eventData.Entering is not NwCreature entering 
        || eventData.Effect.Creator is not NwCreature protector || protector.HP < 1
        || entering.IsReactionTypeHostile(protector))
        return ScriptHandleResult.Handled;

      NWScript.AssignCommand(protector, () => entering.ApplyEffect(EffectDuration.Permanent, Garde));
      return ScriptHandleResult.Handled;
    }
    private static ScriptHandleResult onExitGardeAura(CallInfo callInfo)
    {
      if (!callInfo.TryGetEvent(out AreaOfEffectEvents.OnExit eventData) || eventData.Exiting is not NwCreature exiting
        || eventData.Effect.Creator is not NwCreature protector || exiting.IsReactionTypeHostile(protector))
        return ScriptHandleResult.Handled;

      EffectUtils.RemoveTaggedEffect(exiting, protector, GardeEffectTag);
      return ScriptHandleResult.Handled;
    }
  }
}
