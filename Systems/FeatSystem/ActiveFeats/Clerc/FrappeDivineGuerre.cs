using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void FrappeDivineGuerre(NwCreature caster)
    {
      var clerc = caster.GetClassInfo((ClassType)CustomClass.Clerc);

      if (clerc is null || clerc.Level < 1)
        return;

      NwItem weapon = caster.GetItemInSlot(InventorySlot.RightHand);

      DamageType damageType = weapon is null || !ItemUtils.IsWeapon(weapon.BaseItem) ? DamageType.Bludgeoning : weapon.BaseItem.WeaponType.FirstOrDefault();
      DamageBonus damage = clerc.Level > 13 ? DamageBonus.Plus2d8 : DamageBonus.Plus1d8;
      caster.ApplyEffect(EffectDuration.Permanent, EffectSystem.GetFrappeDivineGuerreEffect(damage, damageType));

      StringUtils.DisplayStringToAllPlayersNearTarget(caster, $"{caster.Name.ColorString(ColorConstants.Cyan)} - Frappe Divine", StringUtils.gold, true, true);
      caster.DecrementRemainingFeatUses((Feat)CustomSkill.ClercDuperieFrappeDivine);
    }
  }
}
