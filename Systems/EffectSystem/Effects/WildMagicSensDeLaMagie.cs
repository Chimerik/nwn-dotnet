using System.Linq;
using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;
using NWN.Core;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string SensDeLaMagieAuraEffectTag = "_WILD_MAGIC_SENS_DE_LA_MAGIE_AURA_EFFECT";
    public const string SensDeLaMagieEffectTag = "_WILD_MAGIC_SENS_DE_LA_MAGIE_EFFECT";
    private static ScriptCallbackHandle onEnterWildMagicAwarenessCallback;
    private static ScriptCallbackHandle onExitWildMagicAwarenessCallback;

    public static Effect wildMagicAwareness
    {
      get
      {
        Effect eff = Effect.Icon(CustomEffectIcon.WildMagicSensDeLaMagie);
        eff.Tag = SensDeLaMagieEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }

    public static Effect wildMagicAwarenessAura
    {
      get
      {
        Effect eff = Effect.AreaOfEffect(PersistentVfxType.MobCircgood, onEnterHandle: onEnterWildMagicAwarenessCallback, onExitHandle: onExitWildMagicAwarenessCallback);
        eff.Tag = SensDeLaMagieAuraEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
    private static ScriptHandleResult onEnterWildMagicAwarenessAura(CallInfo callInfo)
    {
      if (!callInfo.TryGetEvent(out AreaOfEffectEvents.OnEnter eventData) || eventData.Entering is not NwCreature entering 
        || eventData.Effect.Creator is not NwCreature protector || entering.IsReactionTypeHostile(protector)
        || entering.ActiveEffects.Any(e => e.Tag == SensDeLaMagieEffectTag))
        return ScriptHandleResult.Handled;

      NWScript.AssignCommand(protector, () => entering.ApplyEffect(EffectDuration.Permanent, wildMagicAwareness));
      return ScriptHandleResult.Handled;
    }
    private static ScriptHandleResult onExitWildMagicAwarenessAura(CallInfo callInfo)
    {
      if (!callInfo.TryGetEvent(out AreaOfEffectEvents.OnExit eventData) || eventData.Exiting is not NwCreature exiting
        || eventData.Effect.Creator is not NwCreature protector)
        return ScriptHandleResult.Handled;

      EffectUtils.RemoveTaggedEffect(exiting, protector, SensDeLaMagieEffectTag);

      return ScriptHandleResult.Handled;
    }
  }
}
