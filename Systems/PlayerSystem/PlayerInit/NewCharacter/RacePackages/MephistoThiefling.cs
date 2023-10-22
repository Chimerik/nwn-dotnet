using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyMephistoPackage()
      {
        oid.LoginCreature.GetItemInSlot(InventorySlot.CreatureSkin).AddItemProperty(ItemProperty.DamageImmunity(IPDamageType.Fire, IPDamageImmunityType.Immunity50Pct), EffectDuration.Permanent);
      
        // TODO : Penser à gérer les sorts 
        // Level 1 : Mage Hand
        // Level 3 : Mains brûlantes
        // Level 5 : Flame Blade
      }
    }
  }
}
