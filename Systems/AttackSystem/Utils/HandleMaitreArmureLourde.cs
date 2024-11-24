using Anvil.API;
using NWN.Native.API;

namespace NWN.Systems
{
  public static partial class NativeUtils
  {
    public static int HandleMaitreArmureLourde(CNWSCreature target)
    {
      int damageReduction = 0;

      if (target.m_pStats.HasFeat(CustomSkill.MaitreArmureLourde).ToBool())
      {
        CNWSItem armor = target.m_pInventory.GetItemInSlot((uint)EquipmentSlot.Chest);

        if (armor is not null || armor.m_nArmorValue > 5)
        {
          damageReduction = GetCreatureProficiencyBonus(target);
          LogUtils.LogMessage($"Maître des armures lourdes : Dégâts -{damageReduction}", LogUtils.LogType.Combat);
        }
      }
      return damageReduction;
    }
  }
}
