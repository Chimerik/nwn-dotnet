using Anvil.API;

namespace NWN.Systems{
  public static partial class RangerUtils
  {
    public static void HealAnimalCompanion(NwCreature creature, bool longRest = false)
    {
      NwCreature companion = creature.GetObjectVariable<LocalVariableObject<NwCreature>>(CreatureUtils.AnimalCompanionVariable);

      if (companion is not null)
      {
        companion.ApplyEffect(EffectDuration.Instant, Effect.Heal(longRest ? companion.MaxHP : companion.MaxHP / 2));
        creature.GetObjectVariable<PersistentVariableInt>(CreatureUtils.AnimalCompanionVariable).Value = companion.HP;
      }
      else if(creature.GetObjectVariable<PersistentVariableInt>(CreatureUtils.AnimalCompanionVariable).HasValue)
      {
        if (longRest)
          creature.GetObjectVariable<PersistentVariableInt>(CreatureUtils.AnimalCompanionVariable).Delete();
        else
          creature.GetObjectVariable<PersistentVariableInt>(CreatureUtils.AnimalCompanionVariable).Value = 1000;
      }
    }
  }
}
