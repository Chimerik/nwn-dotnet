using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnBarbarianFastMovement(PlayerSystem.Player player, int customSkillId)
    {
      NwItem armor = player.oid.LoginCreature.GetItemInSlot(InventorySlot.Chest);
      NwItem shield = player.oid.LoginCreature.GetItemInSlot(InventorySlot.LeftHand);

      if (armor is null || armor.BaseACValue < 1)
      {
        if (shield is not null)
        {
          switch (shield.BaseItem.ItemType)
          {
            case BaseItemType.SmallShield:
            case BaseItemType.LargeShield:
            case BaseItemType.TowerShield:
              return true;
          }
        }

        if (player.oid.LoginCreature.Classes.Any(c => c.Class.Id == CustomClass.Barbarian && c.Level > 4)
            && !player.oid.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.BarbarianSpeedEffectTag))
          player.oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.BarbarianSpeed);
      }

      return true;
    }
  }
}
