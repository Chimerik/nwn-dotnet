using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string AuraPutrideEffectTag = "_AURA_PUTRIDE_EFFECT";
    private static ScriptCallbackHandle onHeartbeatAuraPutrideCallback;

    public static Effect AuraPutride(NwCreature caster, int saveDC)
    {
      Effect eff = Effect.AreaOfEffect(PersistentVfxType.MobTyrantFog, heartbeatHandle: onHeartbeatAuraPutrideCallback);

      eff.Tag = AuraPutrideEffectTag;
      eff.SubType = EffectSubType.Supernatural;
      eff.Creator = caster;
      eff.CasterLevel = saveDC;

      return eff;
    }

    private static ScriptHandleResult onHeartbeatAuraPutride(CallInfo callInfo)
    {
      if (callInfo.TryGetEvent(out AreaOfEffectEvents.OnHeartbeat eventData) && eventData.Effect.Creator is NwCreature caster)
      {
        foreach(NwCreature target in eventData.Effect.GetObjectsInEffectArea<NwCreature>())
        {
          if (target == caster || target == caster.Master)
            continue;

          if (CreatureUtils.GetSavingThrow(caster, target, Ability.Constitution, eventData.Effect.CasterLevel) == SavingThrowResult.Failure)
            ApplyPoison(target, caster, NwTimeSpan.FromRounds(1), Ability.Constitution, noSave: true);
        }
      }

      return ScriptHandleResult.Handled;
    }
  }
}
