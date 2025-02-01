using System.Linq;
using Anvil.API.Events;

namespace NWN.Systems
{
  public static partial class BarbarianUtils
  {
    public static void OnDamagedWildMagic(CreatureEvents.OnDamaged onDamaged)
    {
      if (onDamaged.DamageAmount > 0)
      {
        var reaction = onDamaged.Creature.ActiveEffects.FirstOrDefault(e => e.Tag == EffectSystem.ReactionEffectTag);

        if (reaction is not null)
        {
          DispelWildMagicEffects(onDamaged.Creature);
          FeatSystem.HandleWildMagicRage(onDamaged.Creature);
          onDamaged.Creature.RemoveEffect(reaction);
        }
      }
    }
  }
}
