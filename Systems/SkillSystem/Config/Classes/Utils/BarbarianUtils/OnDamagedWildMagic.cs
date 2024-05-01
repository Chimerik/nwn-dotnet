using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public static partial class BarbarianUtils
  {
    public static void OnDamagedWildMagic(CreatureEvents.OnDamaged onDamaged)
    {
      if (onDamaged.DamageAmount > 0 && onDamaged.Creature.GetObjectVariable<LocalVariableInt>(CreatureUtils.ReactionVariable).Value > 0)
      {
        DispelWildMagicEffects(onDamaged.Creature);
        FeatSystem.HandleWildMagicRage(onDamaged.Creature);
        onDamaged.Creature.GetObjectVariable<LocalVariableInt>(CreatureUtils.ReactionVariable).Value -= 1;
      }
    }
  }
}
