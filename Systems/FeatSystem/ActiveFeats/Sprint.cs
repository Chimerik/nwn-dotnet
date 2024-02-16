using System.Linq;
using Anvil.API;
using static NWN.Systems.PlayerSystem;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void Sprint(NwCreature caster)
    {
      if (!caster.KnowsFeat(NwFeat.FromFeatId(CustomSkill.TotemEspritAigle)) 
        || !caster.ActiveEffects.Any(e => e.Tag == EffectSystem.BarbarianRageEffectTag)
        || caster.GetObjectVariable<LocalVariableInt>(CreatureUtils.BonusActionVariable).Value < 1)
        return;

      caster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpHaste));
      caster.ApplyEffect(EffectDuration.Temporary, EffectSystem.sprintEffect, NwTimeSpan.FromRounds(1));

      if (caster.KnowsFeat(NwFeat.FromFeatId(CustomSkill.Chargeur)))
        caster.GetObjectVariable<LocalVariableLocation>("_CHARGER_INITIAL_LOCATION").Value = caster.Location;

      if (caster.KnowsFeat(NwFeat.FromFeatId(CustomSkill.Mobile)))
        caster.ApplyEffect(EffectDuration.Temporary, EffectSystem.sprintMobileEffect, NwTimeSpan.FromRounds(1));

      if (caster.KnowsFeat(NwFeat.FromFeatId(CustomSkill.TotemAspectEtalon)))
      {
        foreach (var eff in caster.ActiveEffects)
          if (eff.EffectType == EffectType.TemporaryHitpoints)
            caster.RemoveEffect(eff);

        caster.ApplyEffect(EffectDuration.Permanent, Effect.TemporaryHitpoints(caster.GetClassInfo(NwClass.FromClassType(ClassType.Barbarian)).Level * 2));
      }

      caster.GetObjectVariable<LocalVariableInt>(CreatureUtils.BonusActionVariable).Value -= 1;

      StringUtils.DisplayStringToAllPlayersNearTarget(caster, $"{caster.Name.ColorString(ColorConstants.Cyan)} sprinte", ColorConstants.Orange, true);
    }
  }
}
