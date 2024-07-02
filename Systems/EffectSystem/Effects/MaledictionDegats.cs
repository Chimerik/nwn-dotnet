using System.Linq;
using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;
using NWN.Core;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string MaledictionDegatsEffectTag = "_MALEDICTION_DEGATS_EFFECT";
    private static ScriptCallbackHandle onRemoveMaledictionDegatsCallback;
    public static Effect GetMaledictionDegats(NwCreature target)
    {
      target.OnDamaged -= OnDamagedMaledictionDegats;
      target.OnDamaged += OnDamagedMaledictionDegats;

      Effect eff = Effect.LinkEffects(Effect.Icon(EffectIcon.Curse),
        Effect.RunAction(onRemovedHandle: onRemoveMaledictionDegatsCallback));
      eff.Tag = MaledictionAttaqueEffectTag;
      eff.SubType = EffectSubType.Supernatural;
      return eff;
    }
    private static ScriptHandleResult OnRemoveMaledictionDegats(CallInfo callInfo)
    {
      EffectRunScriptEvent eventData = new EffectRunScriptEvent();

      if (eventData.EffectTarget is not NwCreature creature)
        return ScriptHandleResult.Handled;

      creature.OnDamaged -= OnDamagedMaledictionDegats;

      return ScriptHandleResult.Handled;
    }
    public static void OnDamagedMaledictionDegats(CreatureEvents.OnDamaged onDamaged)
    {
      var creature = onDamaged.Creature;
      var oDamager = NWScript.GetLastDamager(onDamaged.Creature).ToNwObject<NwObject>();

      if (oDamager is not NwCreature damager)
        return;

      if(creature.ActiveEffects.Any(e => e.Tag == MaledictionDegatsEffectTag && e.Creator == damager))
        NWScript.AssignCommand(damager, () => creature.ApplyEffect(EffectDuration.Instant, Effect.Damage(NwRandom.Roll(Utils.random, 8), CustomDamageType.Necrotic)));
    }
  }
}
