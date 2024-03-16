using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static int HandleMaitreArmureLourde(CNWSCreature target)
    {
      if (!target.m_pStats.HasFeat(CustomSkill.MaitreArmureLourde).ToBool())
        return 0;

      CNWSItem armor = target.m_pInventory.GetItemInSlot((uint)EquipmentSlot.Chest);
      
      if(armor is null || armor.m_nArmorValue < 16)
        return 0;

      LogUtils.LogMessage($"Maître des armures lourdes : Dégâts -3", LogUtils.LogType.Combat);

      return 3;
    }
  }
}
