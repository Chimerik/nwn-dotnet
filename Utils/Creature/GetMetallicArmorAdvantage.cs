using Anvil.API;

namespace NWN
{
  public static partial class CreatureUtils
  {
    public static bool GetMetallicArmorAdvantage(NwCreature target, NwSpell spell)
    {
      if (spell.SpellType == Spell.ElectricJolt)
      {
        NwItem armor = target.GetItemInSlot(InventorySlot.Chest);
        if (armor is not null && armor.BaseACValue > 4)
        {
          LogUtils.LogMessage("Avantage - Poigne électrique sur armure métallique", LogUtils.LogType.Combat);
          return true;
        }
      }

      return false;
    }
  }
}
