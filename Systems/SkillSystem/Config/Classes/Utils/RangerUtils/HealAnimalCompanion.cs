using Anvil.API;

namespace NWN.Systems
{
  public static partial class RangerUtils
  {
    public static void HealAnimalCompanion(NwCreature creature, bool longRest = false)
    {
      NwCreature companion = creature.GetObjectVariable<LocalVariableObject<NwCreature>>(CreatureUtils.AnimalCompanionVariable);

      if (companion is null)
        return;

      companion.ApplyEffect(EffectDuration.Instant, Effect.Heal(longRest ? companion.HP : companion.HP / 2));
      creature.GetObjectVariable<PersistentVariableInt>(CreatureUtils.AnimalCompanionVariable).Value = companion.HP;

      
    }
  }
}
