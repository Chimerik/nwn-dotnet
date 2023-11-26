using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static int HandleMaitreArmureLourde(CNWSObject target)
    {
      CNWSCreature targetCreature = target.m_nObjectType == (int)ObjectType.Creature ? target.AsNWSCreature() : null;

      if (targetCreature is null || !targetCreature.m_pStats.HasFeat(CustomSkill.MaitreArmureLourde).ToBool())
        return 0;

      CNWSItem armor = targetCreature.m_pInventory.GetItemInSlot((uint)Native.API.InventorySlot.Chest);
      
      if(armor is null || armor.m_nArmorValue < 16)
        return 0;

      LogUtils.LogMessage($"Maître des armures lourdes : Dégâts -3", LogUtils.LogType.Combat);

      return 3;
    }
  }
}
