using Anvil.API;
using NWN.Native.API;
using InventorySlot = NWN.Native.API.InventorySlot;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static int GetFightingStyleDefenseBonus(CNWSCreature creature)
    {
      if (creature.m_pStats.HasFeat(CustomSkill.FighterCombatStyleDefense).ToBool())
      {
        CNWSItem armor = NWNXLib.AppManager().m_pServerExoApp.GetItemByGameObjectID(creature.m_pInventory.m_pEquipSlot[(int)InventorySlot.Chest]);
        CNWSItem shield = NWNXLib.AppManager().m_pServerExoApp.GetItemByGameObjectID(creature.m_pInventory.m_pEquipSlot[(int)InventorySlot.LeftHand]);

        if ((armor is not null && armor.m_nArmorValue > 0) || (shield is not null && shield.m_nArmorValue > 0))
        {
          LogUtils.LogMessage("Style de combat défensif : +1 CA", LogUtils.LogType.Combat);
          return 1;
        }
      }

      return 0;
    }
  }
}
