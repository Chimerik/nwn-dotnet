using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string VoileNecrotiqueEffectTag = "_VOILE_NECROTIQUE_EFFECT";
    private static ScriptCallbackHandle onHeartbeatVoileNecrotiqueCallback;
    public static Effect VoileNecrotique
    {
      get
      {
        Effect eff = Effect.LinkEffects(Effect.VisualEffect(VfxType.ImpAuraNegativeEnergy),
          Effect.AreaOfEffect(PersistentVfxType.PerCustomAoe, heartbeatHandle: onHeartbeatVoileNecrotiqueCallback));
        eff.Tag = VoileNecrotiqueEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
    private static ScriptHandleResult onHeartbeatVoileNecrotique(CallInfo callInfo)
    {
      if (!callInfo.TryGetEvent(out AreaOfEffectEvents.OnHeartbeat eventData) || eventData.Effect.Creator is not NwCreature caster)
        return ScriptHandleResult.Handled;

      int spellDC = SpellUtils.GetCasterSpellDC(caster, Ability.Charisma);

      foreach (NwCreature entering in eventData.Effect.GetObjectsInEffectArea<NwCreature>())
      {
        if (caster.IsReactionTypeHostile(entering) && CreatureUtils.GetSavingThrow(caster, entering, Ability.Charisma, spellDC) == SavingThrowResult.Failure)
          ApplyEffroi(entering, caster, NwTimeSpan.FromRounds(1), spellDC);
      }

      return ScriptHandleResult.Handled;
    }
  }
}
