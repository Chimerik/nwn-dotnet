using Anvil.API;

namespace NWN
{
  public static partial class CreatureUtils
  {
    public static int GetMetallicArmorAdvantage(NwCreature target)
    {
      NwItem armor = target.GetItemInSlot(InventorySlot.Chest);
      return armor is null || armor.BaseACValue < 5 ? 0 : 1;
    }
  }
}
