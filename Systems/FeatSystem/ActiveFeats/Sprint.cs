using System;
using System.Linq;
using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void Sprint(NwCreature caster, OnUseFeat onUseFeat)
    {
      if (caster.GetObjectVariable<LocalVariableInt>(CreatureUtils.BonusActionVariable).Value < 1)
        return;

      if (caster.Classes.Any(c => c.Class.ClassType == ClassType.Rogue && c.Level > 1)
        || !caster.Classes.Any(c => c.Class.ClassType == ClassType.Monk && c.Level > 1)
        || (caster.KnowsFeat(NwFeat.FromFeatId(CustomSkill.TotemEspritAigle))
           && caster.ActiveEffects.Any(e => e.Tag == EffectSystem.BarbarianRageEffectTag)))
      {

        caster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpHaste));
        caster.ApplyEffect(EffectDuration.Temporary, EffectSystem.sprintEffect, TimeSpan.FromSeconds(9));

        if (caster.KnowsFeat(NwFeat.FromFeatId(CustomSkill.Mobile)))
          caster.ApplyEffect(EffectDuration.Temporary, EffectSystem.sprintMobileEffect, TimeSpan.FromSeconds(9));

        if (caster.KnowsFeat(NwFeat.FromFeatId(CustomSkill.TotemAspectEtalon)))
        {
          foreach (var eff in caster.ActiveEffects)
            if (eff.EffectType == EffectType.TemporaryHitpoints)
              caster.RemoveEffect(eff);

          caster.ApplyEffect(EffectDuration.Permanent, Effect.TemporaryHitpoints(caster.GetClassInfo(NwClass.FromClassType(ClassType.Barbarian)).Level * 2));
        }

        caster.GetObjectVariable<LocalVariableInt>(CreatureUtils.BonusActionVariable).Value -= 1;

        if (caster.KnowsFeat(NwFeat.FromFeatId(CustomSkill.Chargeur)))
          caster.GetObjectVariable<LocalVariableLocation>(EffectSystem.ChargerVariable).Value = caster.Location;

        StringUtils.DisplayStringToAllPlayersNearTarget(caster, $"{caster.Name.ColorString(ColorConstants.Cyan)} sprinte", ColorConstants.Orange, true);
        onUseFeat.PreventFeatUse = true;
      }
    }
  }
}
