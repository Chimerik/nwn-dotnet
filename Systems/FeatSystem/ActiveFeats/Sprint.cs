﻿using System.Linq;
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

      if (caster.KnowsFeat((Feat)CustomSkill.RangerDeplacementFluide)
        || caster.Classes.Any(c => Utils.In(c.Class.ClassType, ClassType.Rogue, (ClassType)CustomClass.RogueArcaneTrickster) && c.Level > 1)
        || caster.Classes.Any(c => c.Class.Id == CustomClass.Monk && c.Level > 1)
        || (caster.KnowsFeat((Feat)CustomSkill.TotemEspritAigle)
           && caster.ActiveEffects.Any(e => e.Tag == EffectSystem.BarbarianRageEffectTag)))
      {

        EffectUtils.RemoveTaggedEffect(caster, EffectSystem.SprintEffectTag);

        caster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpHaste));
        caster.ApplyEffect(EffectDuration.Temporary, EffectSystem.sprintEffect, NwTimeSpan.FromRounds(1));

        if (caster.KnowsFeat((Feat)CustomSkill.Mobile))
          caster.ApplyEffect(EffectDuration.Temporary, EffectSystem.sprintMobileEffect, NwTimeSpan.FromRounds(1));

        if (caster.KnowsFeat((Feat)CustomSkill.TotemAspectEtalon))
        {
          foreach (var eff in caster.ActiveEffects)
            if (eff.EffectType == EffectType.TemporaryHitpoints)
              caster.RemoveEffect(eff);

          caster.ApplyEffect(EffectDuration.Permanent, Effect.TemporaryHitpoints(caster.GetClassInfo(ClassType.Barbarian).Level * 2));
        }

        caster.GetObjectVariable<LocalVariableInt>(CreatureUtils.BonusActionVariable).Value -= 1;

        if (caster.KnowsFeat((Feat)CustomSkill.Chargeur))
          caster.GetObjectVariable<LocalVariableLocation>(EffectSystem.ChargerVariable).Value = caster.Location;

        StringUtils.DisplayStringToAllPlayersNearTarget(caster, $"{caster.Name.ColorString(ColorConstants.Cyan)} sprinte", ColorConstants.Orange, true);
        onUseFeat.PreventFeatUse = true;
      }
    }
  }
}
