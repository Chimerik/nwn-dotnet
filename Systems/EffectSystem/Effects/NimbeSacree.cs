using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;
using NWN.Core;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string NimbeSacreeAuraEffectTag = "_NIMBE_SACREE_AURA_EFFECT";
    private static ScriptCallbackHandle onIntervalNimbeSacreeCallback;
    
    public static Effect NimbeSacree
    {
      get
      {
        Effect eff = Effect.LinkEffects(Effect.VisualEffect(VfxType.DurLightWhite20),
          Effect.AreaOfEffect(PersistentVfxType.MobHorrificappearance, heartbeatHandle: onIntervalNimbeSacreeCallback));
        eff.Tag = NimbeSacreeAuraEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
    private static ScriptHandleResult OnIntervalNimbeSacree(CallInfo callInfo)
    {
      if (!callInfo.TryGetEvent(out AreaOfEffectEvents.OnHeartbeat eventData) || eventData.Effect.Creator is not NwCreature intimidator)
        return ScriptHandleResult.Handled;


      foreach (var target in eventData.Effect.GetObjectsInEffectArea<NwCreature>())
      {
        target.LoginPlayer?.DisplayFloatingTextStringOnCreature(intimidator, "Nimbre Sacrée".ColorString(StringUtils.gold));

        if (!target.IsReactionTypeHostile(intimidator))
          continue;

        NWScript.AssignCommand(intimidator, () => target.ApplyEffect(EffectDuration.Instant, Effect.Damage(10, DamageType.Divine)));
      }

      return ScriptHandleResult.Handled;
    }
  }
}
