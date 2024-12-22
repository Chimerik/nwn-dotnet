using System.Linq;
using System.Security.Cryptography;
using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnUnarmoredDefence(PlayerSystem.Player player, int customSkillId)
    {
      if (player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.BarbarianUnarmoredDefence))
        player.oid.LoginCreature.AddFeat((Feat)CustomSkill.BarbarianUnarmoredDefence);

      player.oid.LoginCreature.OnItemEquip -= ItemSystem.OnEquipUnarmoredDefence;
      player.oid.LoginCreature.OnItemUnequip -= ItemSystem.OnUnEquipUnarmoredDefence;
      player.oid.LoginCreature.OnItemEquip += ItemSystem.OnEquipUnarmoredDefence;
      player.oid.LoginCreature.OnItemUnequip += ItemSystem.OnUnEquipUnarmoredDefence;

      if (!ItemUtils.IsArmor(player.oid.LoginCreature.GetItemInSlot(InventorySlot.Chest)))
      {
        player.oid.LoginCreature.OnHeartbeat -= CreatureUtils.OnHeartBeatCheckUnarmoredDefence;
        player.oid.LoginCreature.OnHeartbeat += CreatureUtils.OnHeartBeatCheckUnarmoredDefence;
        EffectSystem.ApplyUnarmoredDefenseEffect(player.oid.LoginCreature);
      }

      return true;
    }
  }
}
