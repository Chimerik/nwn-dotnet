using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void WildMagicArmeInfusee(NwCreature caster)
    {
      StringUtils.DisplayStringToAllPlayersNearTarget(caster, "Magie Sauvage - Arme Infusée", StringUtils.gold, true);
      caster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpSuperHeroism));

      NwItem mainWeapon = caster.GetItemInSlot(InventorySlot.RightHand);
      NwItem secondaryWeapon = caster.GetItemInSlot(InventorySlot.LeftHand);

      if(mainWeapon is not null && ItemUtils.IsMeleeWeapon(mainWeapon.BaseItem))
        mainWeapon.AddItemProperty(ItemProperty.DamageBonus(IPDamageType.Magical, IPDamageBonus.Plus1d6), EffectDuration.Temporary, NwTimeSpan.FromRounds(10));

      if (secondaryWeapon is not null && ItemUtils.IsMeleeWeapon(secondaryWeapon.BaseItem))
        secondaryWeapon.AddItemProperty(ItemProperty.DamageBonus(IPDamageType.Magical, IPDamageBonus.Plus1d6), EffectDuration.Temporary, NwTimeSpan.FromRounds(10));

    }
  }
}
