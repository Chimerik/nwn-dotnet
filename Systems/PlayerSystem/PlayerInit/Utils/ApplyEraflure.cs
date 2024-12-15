using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyEraflure()
      {
        if (oid.LoginCreature.KnowsFeat((Feat)CustomSkill.ExpertiseEraflure))
        {
          var weapon = oid.LoginCreature.GetItemInSlot(InventorySlot.RightHand);
          var secondWeapon = oid.LoginCreature.GetItemInSlot(InventorySlot.LeftHand);

          if ((weapon is not null && Utils.In(weapon.BaseItem.ItemType, BaseItemType.LightFlail, BaseItemType.HeavyFlail, BaseItemType.Morningstar, BaseItemType.LightMace, BaseItemType.DireMace, BaseItemType.LightHammer, BaseItemType.Warhammer))
            || (secondWeapon is not null && Utils.In(weapon.BaseItem.ItemType, BaseItemType.LightFlail, BaseItemType.HeavyFlail, BaseItemType.Morningstar, BaseItemType.LightMace, BaseItemType.DireMace, BaseItemType.LightHammer, BaseItemType.Warhammer)))
          {
            oid.LoginCreature.OnCreatureAttack -= CreatureUtils.OnAttackEraflure;
            oid.LoginCreature.OnCreatureAttack += CreatureUtils.OnAttackEraflure;
          }
        }
      }
    }
  }
}
