using System.Linq;
using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public static partial class SpellEvent
  {
    public static void OnDamagedProtectionContreLaMort(CreatureEvents.OnDamaged onDamage)
    {
      if (onDamage.Creature.HP < 1)
      {
        if (onDamage.Creature.ActiveEffects.Any(e => e.EffectType == EffectType.Polymorph))
          return;

        onDamage.Creature.OnDamaged -= OnDamagedProtectionContreLaMort;

        if (!onDamage.Creature.ActiveEffects.Any(e => e.Tag == EffectSystem.ProtectionContreLaMortEffectTag))
          return;

        onDamage.Creature.HP = 1;

        EffectUtils.RemoveTaggedEffect(onDamage.Creature, EffectSystem.ProtectionContreLaMortEffectTag);
        StringUtils.DisplayStringToAllPlayersNearTarget(onDamage.Creature, $"{onDamage.Creature.Name.ColorString(ColorConstants.Cyan)} - Protection contre la Mort", StringUtils.gold, true, true);
      }
    }
  }
}
