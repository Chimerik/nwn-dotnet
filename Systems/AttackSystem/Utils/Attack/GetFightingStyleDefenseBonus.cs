using Anvil.API;
using NWN.Core.NWNX;
using NWN.Native.API;
using InventorySlot = NWN.Native.API.InventorySlot;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static int GetFightingStyleDefenseBonus(CNWSCreature creature)
    {
      uint armor = creature.m_pInventory.m_pEquipSlot[(int)InventorySlot.Chest];

      if (armor != 0x7F000000 && ItemPlugin.GetBaseArmorClass(armor) > 0 && creature.m_pStats.HasFeat(CustomSkill.FighterCombatStyleDefense).ToBool())
      {
        LogUtils.LogMessage("Style de combat défensif : +1 CA", LogUtils.LogType.Combat);
        return 1;
      }

      return 0;
    }
  }
}
