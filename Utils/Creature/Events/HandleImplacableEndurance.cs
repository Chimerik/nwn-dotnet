using Anvil.API.Events;
using Anvil.API;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static void HandleImplacableEndurance(CreatureEvents.OnDamaged onDamage)
    {
      if (onDamage.Creature.HP < 1)
      {
        //onDamage.Creature.ApplyEffect(EffectDuration.Temporary, Effect.TemporaryHitpoints(onDamage.DamageAmount - onDamage.Creature.HP + 1), TimeSpan.FromSeconds(6));
        onDamage.Creature.HP = 1;

        foreach (var eff in onDamage.Creature.ActiveEffects)
          if (eff.Tag == EffectSystem.EnduranceImplacableEffectTag)
            onDamage.Creature.RemoveEffect(eff);

        if (onDamage.Creature.KnowsFeat(NwFeat.FromFeatId(CustomSkill.FureurOrc))
          && onDamage.Creature.CurrentAction == Anvil.API.Action.AttackObject
          && onDamage.Creature.GetObjectVariable<LocalVariableInt>(ReactionVariable).Value > 0)
        {
          onDamage.Creature.GetObjectVariable<LocalVariableInt>(FureurOrcBonusAttackVariable).Value = 1;
          onDamage.Creature.GetObjectVariable<LocalVariableInt>(ReactionVariable).Value -= 1;
        }

        onDamage.Creature.GetObjectVariable<PersistentVariableInt>(EffectSystem.EnduranceImplacableVariable).Delete();
        onDamage.Creature.OnDamaged -= HandleImplacableEndurance;

        StringUtils.DisplayStringToAllPlayersNearTarget(onDamage.Creature, "Endurance Implacable", StringUtils.gold, true);
      }
    }
  }
}
