using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string RayonnementInterieurEffectTag = "_RAYONNEMENT_INTERIEUR_EFFECT";
    private static ScriptCallbackHandle onHeartbeatRayonnementInterieurCallback;
    public static Effect RayonnementInterieur
    {
      get
      {
        Effect eff = Effect.LinkEffects(Effect.VisualEffect(VfxType.ImpAuraHoly),
          Effect.AreaOfEffect(PersistentVfxType.PerCustomAoe, heartbeatHandle: onHeartbeatRayonnementInterieurCallback));
        eff.Tag = RayonnementInterieurEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
    private static ScriptHandleResult onHeartbeatRayonnementInterieur(CallInfo callInfo)
    {
      if (!callInfo.TryGetEvent(out AreaOfEffectEvents.OnHeartbeat eventData) || eventData.Effect.Creator is not NwCreature caster)
        return ScriptHandleResult.Handled;

      int proficiencyLevel = NativeUtils.GetCreatureProficiencyBonus(caster);

      foreach (NwCreature entering in eventData.Effect.GetObjectsInEffectArea<NwCreature>())
      {
        if (caster.IsReactionTypeHostile(entering))
          entering.ApplyEffect(EffectDuration.Instant, Effect.Damage(proficiencyLevel, DamageType.Divine));
      }

      return ScriptHandleResult.Handled;
    }
  }
}
