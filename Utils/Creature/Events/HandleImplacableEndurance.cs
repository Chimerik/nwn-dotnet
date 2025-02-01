using Anvil.API.Events;
using Anvil.API;
using System.Linq;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static void HandleImplacableEndurance(CreatureEvents.OnDamaged onDamage)
    {
      if (onDamage.Creature.HP < 1)
      {
        if (onDamage.Creature.ActiveEffects.Any(e => e.EffectType == EffectType.Polymorph))
          return;

        onDamage.Creature.HP = 1;

        EffectUtils.RemoveTaggedEffect(onDamage.Creature, EffectSystem.EnduranceImplacableEffectTag);

        if (onDamage.Creature.KnowsFeat((Feat)CustomSkill.FureurOrc)
          && onDamage.Creature.CurrentAction == Action.AttackObject)
        {
          var reaction = onDamage.Creature.ActiveEffects.FirstOrDefault(e => e.Tag == EffectSystem.ReactionEffectTag);

          if(reaction is not null)
          {
            onDamage.Creature.GetObjectVariable<LocalVariableInt>(FureurOrcBonusAttackVariable).Value = 1;
            onDamage.Creature.RemoveEffect(reaction);
          }
        }

        onDamage.Creature.GetObjectVariable<PersistentVariableInt>(EffectSystem.EnduranceImplacableVariable).Delete();
        onDamage.Creature.OnDamaged -= HandleImplacableEndurance;

        StringUtils.DisplayStringToAllPlayersNearTarget(onDamage.Creature, $"{onDamage.Creature.Name.ColorString(ColorConstants.Cyan)} - Endurance Implacable", StringUtils.gold, true, true);
      }
    }
  }
}
