using Anvil.API;
using static NWN.Systems.SkillSystem;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyZarielPackage()
      {
        oid.LoginCreature.GetItemInSlot(InventorySlot.CreatureSkin).AddItemProperty(ItemProperty.DamageImmunity(IPDamageType.Fire, IPDamageImmunityType.Immunity50Pct), EffectDuration.Permanent);
      
        // TODO : Penser à gérer les sorts 
        // Level 1 : Thaumaturgy
        // Level 3 : Searing Smite
        // Level 5 : Branding Smite
      }
    }
  }
}
