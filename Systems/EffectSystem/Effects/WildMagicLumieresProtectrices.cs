using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;
using NWN.Core;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string LumieresProtectricesAuraEffectTag = "_WILD_MAGIC_LUMIERES_PROTECTRICES_AURA_EFFECT";
    public const string LumieresProtectricesEffectTag = "_WILD_MAGIC_LUMIERES_PROTECTRICES";
    private static ScriptCallbackHandle onEnterWildMagicLumieresProtectricesCallback;
    private static ScriptCallbackHandle onExitWildMagicLumieresProtectricesCallback;

    public static Effect wildMagicLumieresProtectrices
    {
      get
      {
        Effect eff = Effect.ACIncrease(1);
        eff.Tag = LumieresProtectricesEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
    public static Effect wildMagicLumieresProtectricesAura
    {
      get
      {
        Effect eff = Effect.LinkEffects(Effect.Icon(CustomEffectIcon.BarbarianWildMagicLumieresProtectrices),
          Effect.AreaOfEffect(PersistentVfxType.MobCircgood, onEnterHandle: onEnterWildMagicLumieresProtectricesCallback, onExitHandle: onExitWildMagicLumieresProtectricesCallback));
        eff.Tag = LumieresProtectricesAuraEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
    private static ScriptHandleResult onEnterWildMagicLumieresProtectricesAura(CallInfo callInfo)
    {
      if (!callInfo.TryGetEvent(out AreaOfEffectEvents.OnEnter eventData) || eventData.Entering is not NwCreature entering 
        || eventData.Effect.Creator is not NwCreature protector || entering.IsReactionTypeHostile(protector))
        return ScriptHandleResult.Handled;

      entering.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpHolyAid));
      NWScript.AssignCommand(protector, () => entering.ApplyEffect(EffectDuration.Permanent, wildMagicLumieresProtectrices));
      return ScriptHandleResult.Handled;
    }
    private static ScriptHandleResult onExitWildMagicLumieresProtectricesAura(CallInfo callInfo)
    {
      if (!callInfo.TryGetEvent(out AreaOfEffectEvents.OnExit eventData) || eventData.Exiting is not NwCreature exiting
        || eventData.Effect.Creator is not NwCreature protector)
        return ScriptHandleResult.Handled;

      foreach (var eff in exiting.ActiveEffects)
        if (eff.Creator == protector && eff.Tag == LumieresProtectricesEffectTag)
          exiting.RemoveEffect(eff);

      return ScriptHandleResult.Handled;
    }
  }
}
