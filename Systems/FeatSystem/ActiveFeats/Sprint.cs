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

      if (caster.Race.Id == CustomRace.HalfOrc 
        || caster.Classes.Any(c => Utils.In(c.Class.ClassType, ClassType.Rogue, (ClassType)CustomClass.RogueArcaneTrickster) && c.Level > 1)
        || caster.Classes.Any(c => c.Class.Id == CustomClass.Monk && c.Level > 1)
        || caster.ActiveEffects.Any(e => e.Tag == EffectSystem.BarbarianRageEffectTag && e.IntParams[6] == CustomSpell.RageSauvageAigle))
      {
        EffectUtils.RemoveTaggedEffect(caster, EffectSystem.SprintEffectTag);

        caster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpHaste));
        caster.ApplyEffect(EffectDuration.Temporary, EffectSystem.sprintEffect, NwTimeSpan.FromRounds(1));

        if (caster.KnowsFeat((Feat)CustomSkill.Mobile))
          caster.ApplyEffect(EffectDuration.Temporary, EffectSystem.sprintMobileEffect, NwTimeSpan.FromRounds(1));

        caster.GetObjectVariable<LocalVariableInt>(CreatureUtils.BonusActionVariable).Value -= 1;

        if (caster.KnowsFeat((Feat)CustomSkill.Chargeur))
          caster.GetObjectVariable<LocalVariableLocation>(EffectSystem.ChargerVariable).Value = caster.Location;

        if (caster.Race.Id == CustomRace.HalfOrc)
          caster.ApplyEffect(EffectDuration.Permanent, Effect.TemporaryHitpoints(NativeUtils.GetCreatureProficiencyBonus(caster)));

        if(caster.ActiveEffects.Any(e => e.Tag == EffectSystem.BarbarianRageEffectTag && e.IntParams[6] == CustomSpell.RageSauvageAigle))
          caster.ApplyEffect(EffectDuration.Temporary, EffectSystem.disengageEffect, NwTimeSpan.FromRounds(1));

        if(caster.KnowsFeat((Feat)CustomSkill.BelluaireEntrainementExceptionnel))
        {
          var companion = caster.GetObjectVariable<LocalVariableObject<NwCreature>>(CreatureUtils.AnimalCompanionVariable).Value;

          if(companion is not null)
          {
            companion.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpHaste));
            companion.ApplyEffect(EffectDuration.Temporary, EffectSystem.sprintEffect, NwTimeSpan.FromRounds(1));
          }
        }

        StringUtils.DisplayStringToAllPlayersNearTarget(caster, $"{caster.Name.ColorString(ColorConstants.Cyan)} sprinte", ColorConstants.Orange, true);
        onUseFeat.PreventFeatUse = true;
      }
    }
  }
}
