using System;
using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static async void Sprint(NwCreature caster, PlayerSystem.Player player)
    {
      await NwTask.Delay(TimeSpan.FromSeconds(3));
      caster.ApplyEffect(EffectDuration.Temporary, EffectSystem.sprintEffect, NwTimeSpan.FromRounds(1));

      if (player.learnableSkills.ContainsKey(CustomSkill.Chargeur))
      {
        caster.GetObjectVariable<LocalVariableLocation>("_CHARGER_INITIAL_LOCATION").Value = caster.Location;
        caster.OnCreatureAttack -= CreatureUtils.OnAttackCharge;
        caster.OnCreatureAttack += CreatureUtils.OnAttackCharge;
      }

      if(player.oid.LoginCreature.KnowsFeat(NwFeat.FromFeatId(CustomSkill.Mobile)))
        caster.ApplyEffect(EffectDuration.Temporary, EffectSystem.sprintMobileEffect, NwTimeSpan.FromRounds(1));
    }
  }
}
