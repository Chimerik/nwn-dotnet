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

      if (onDamaged.DamageAmount > 0 && creature.GetObjectVariable<LocalVariableInt>(CreatureUtils.ReactionVariable).Value > 0)
      {
        var weapon = creature.GetItemInSlot(InventorySlot.RightHand);

        if (weapon is null || !ItemUtils.IsMeleeWeapon(NwBaseItem.FromItemId((int)weapon.BaseItem.Id))
          || creature.DistanceSquared(damager) > 12)
          return;

        creature.GetObjectVariable<LocalVariableObject<NwCreature>>(CreatureUtils.BersekerRepresaillesVariable).Value = damager;
        creature.GetObjectVariable<LocalVariableInt>(CreatureUtils.ReactionVariable).Value -= 1;
      }
    }
  }
}
