using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyAsmodeusPackage()
      {
        oid.LoginCreature.GetItemInSlot(InventorySlot.CreatureSkin).AddItemProperty(ItemProperty.DamageImmunity(IPDamageType.Fire, IPDamageImmunityType.Immunity50Pct), EffectDuration.Permanent);
      
        // TODO : Penser à gérer les sorts 
        // Level 1 : Produce Flame
        // Level 3 : Hellish Rebuke
        // Level 5 : Ténèbres
      }
    }
  }
}
