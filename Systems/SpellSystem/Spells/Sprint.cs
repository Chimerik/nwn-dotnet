using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void Sprint(SpellEvents.OnSpellCast onSpellCast, PlayerSystem.Player player)
    {
      if (onSpellCast.Caster is not NwCreature caster)
        return;

      SpellUtils.SignalEventSpellCast(onSpellCast.TargetObject, caster, onSpellCast.Spell.SpellType);

      caster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpHaste));
      caster.ApplyEffect(EffectDuration.Temporary, EffectSystem.sprintEffect, NwTimeSpan.FromRounds(1));

      if (player.learnableSkills.ContainsKey(CustomSkill.Chargeur))
      {
        caster.GetObjectVariable<LocalVariableLocation>("_CHARGER_INITIAL_LOCATION").Value = caster.Location;
        caster.OnCreatureAttack -= CreatureUtils.OnAttackCharge;
        caster.OnCreatureAttack += CreatureUtils.OnAttackCharge;
      }

      if (player.oid.LoginCreature.KnowsFeat(NwFeat.FromFeatId(CustomSkill.Mobile)))
        caster.ApplyEffect(EffectDuration.Temporary, EffectSystem.sprintMobileEffect, NwTimeSpan.FromRounds(1));
    }
  }
}
