using System.Linq;
using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void Disengage(NwCreature caster, OnUseFeat onUseFeat)
    {
      if (caster.GetObjectVariable<LocalVariableInt>(CreatureUtils.BonusActionVariable).Value < 1
        || (!caster.Classes.Any(c => Utils.In(c.Class.ClassType, ClassType.Rogue, (ClassType)CustomClass.RogueArcaneTrickster) && c.Level > 1)
        && (!caster.Classes.Any(c => c.Class.Id == CustomClass.Monk && c.Level > 1))))
        return;

      caster.ApplyEffect(EffectDuration.Temporary, EffectSystem.disengageEffect, NwTimeSpan.FromRounds(1));
      caster.GetObjectVariable<LocalVariableInt>(CreatureUtils.BonusActionVariable).Value -= 1;

      if (caster.KnowsFeat((Feat)CustomSkill.BelluaireEntrainementExceptionnel))
      {
        var companion = caster.GetObjectVariable<LocalVariableObject<NwCreature>>(CreatureUtils.AnimalCompanionVariable).Value;

        if (companion is not null)
          companion.ApplyEffect(EffectDuration.Temporary, EffectSystem.disengageEffect, NwTimeSpan.FromRounds(1));
      }

      StringUtils.DisplayStringToAllPlayersNearTarget(caster, $"{caster.Name.ColorString(ColorConstants.Cyan)} se désengage", ColorConstants.Orange, true);
      onUseFeat.PreventFeatUse = true;
    }
  }
}
