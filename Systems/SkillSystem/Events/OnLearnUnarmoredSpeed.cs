using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnUnarmoredSpeed(PlayerSystem.Player player, int customSkillId)
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

        if(!player.oid.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.MonkSpeedEffectTag))
          player.oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.GetMonkSpeedEffect(player.oid.LoginCreature.Classes.FirstOrDefault(c => c.Class.ClassType == ClassType.Monk).Level));
      }

      return true;
    }
  }
}
