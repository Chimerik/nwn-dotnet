using Anvil.API;
using NWN.Core;

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
      var ip = ItemProperty.DamageBonus(IPDamageType.Magical, IPDamageBonus.Plus1d6);
      ip.Tag = "_WILDMAGIC_ARME_INFUSEE_ITEM_PROPERTY";

      if (mainWeapon is not null && ItemUtils.IsMeleeWeapon(mainWeapon.BaseItem))
      {
        caster.GetObjectVariable<LocalVariableObject<NwItem>>("_WILDMAGIC_ARME_INFUSEE_1").Value = mainWeapon;
        NWScript.AssignCommand(caster, () => mainWeapon.AddItemProperty(ip, EffectDuration.Temporary, NwTimeSpan.FromRounds(10)));
      }
      if (secondaryWeapon is not null && ItemUtils.IsMeleeWeapon(secondaryWeapon.BaseItem))
      {
        caster.GetObjectVariable<LocalVariableObject<NwItem>>("_WILDMAGIC_ARME_INFUSEE_2").Value = secondaryWeapon;
        NWScript.AssignCommand(caster, () => secondaryWeapon.AddItemProperty(ip, EffectDuration.Temporary, NwTimeSpan.FromRounds(10)));
      }
    }
  }
}
