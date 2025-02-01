using System.Linq;
using Anvil.API;
using Anvil.API.Events;
using NWN.Core;

namespace NWN.Systems
{
  public static partial class BarbarianUtils
  {
    public static void OnDamagedBerserkerRepresailles(CreatureEvents.OnDamaged onDamaged)
    {
      var oDamager = NWScript.GetLastDamager(onDamaged.Creature).ToNwObject<NwObject>();

      if (oDamager is not NwCreature damager)
        return;

      var creature = onDamaged.Creature;

      if (onDamaged.DamageAmount > 0)
      {
        var reaction = creature.ActiveEffects.FirstOrDefault(e => e.Tag == EffectSystem.ReactionEffectTag);

        if (reaction is not null)
        {
          var weapon = creature.GetItemInSlot(InventorySlot.RightHand);

          if (weapon is null || !ItemUtils.IsMeleeWeapon(NwBaseItem.FromItemId((int)weapon.BaseItem.Id))
            || creature.DistanceSquared(damager) > 12)
            return;

          creature.GetObjectVariable<LocalVariableObject<NwCreature>>(CreatureUtils.BersekerRepresaillesVariable).Value = damager;
          creature.RemoveEffect(reaction);
        }
      }
    }
  }
}
