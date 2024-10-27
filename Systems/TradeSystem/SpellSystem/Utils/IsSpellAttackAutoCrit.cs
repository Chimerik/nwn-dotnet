using Anvil.API;

namespace NWN.Systems
{
  public static partial class SpellUtils
  {
    public static bool IsSpellAttackAutoCrit(NwCreature target)
    {
      NwItem targetArmor = target.GetItemInSlot(InventorySlot.Chest);

      if (target.IsLoginPlayerCharacter && targetArmor is not null && targetArmor.GetObjectVariable<LocalVariableInt>("_MAX_DURABILITY").HasValue
       && targetArmor.GetObjectVariable<LocalVariableInt>("_DURABILITY") < 1)
      {
        target.ControllingPlayer.SendServerMessage($"CRITIQUE AUTOMATIQUE - Armure en ruine ".ColorString(new Color(255, 215, 0)));
        LogUtils.LogMessage($"Armure ruinée : critique automatique", LogUtils.LogType.Combat);
        return true;
      }

      return false;
    }
  }
}
