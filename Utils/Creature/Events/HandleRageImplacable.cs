using Anvil.API.Events;
using Anvil.API;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static void OnDamagedRageImplacable(CreatureEvents.OnDamaged onDamage)
    {
      if (onDamage.Creature.HP - onDamage.DamageAmount < 1)
      {
        SpellConfig.SavingThrowFeedback feedback = new();
        int saveDC = onDamage.Creature.GetObjectVariable<PersistentVariableInt>("_RAGE_IMPLACABLE_DD").Value;
        int advantage = GetCreatureAbilityAdvantage(onDamage.Creature, Ability.Constitution);
        int totalSave = SpellUtils.GetSavingThrowRoll(onDamage.Creature, Ability.Constitution, saveDC, advantage, feedback);

        onDamage.Creature.GetObjectVariable<PersistentVariableInt>("_RAGE_IMPLACABLE_DD").Value += 5;

        if(saveDC >= totalSave) 
        {
          onDamage.Creature.HP = 1;
          StringUtils.DisplayStringToAllPlayersNearTarget(onDamage.Creature, "Rage Implacable", StringUtils.gold, true);
        }
      }
    }
  }
}
