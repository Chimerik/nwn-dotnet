using System;
using Anvil.API;

namespace NWN.Systems
{
  public static partial class SpellUtils
  {
    public static int HandleResistanceBypass(NwCreature target, bool isElementalist, bool isEvocateurSurcharge, int damage, DamageType damageType)
    {
      if ((!isElementalist && !isEvocateurSurcharge) || damage < 1 || target is null)
        return damage;

      foreach (var eff in target.ActiveEffects)
        if (eff.EffectType == EffectType.DamageImmunityIncrease && eff.IntParams[0] == (int)damageType)
          return damage *= 2;

        foreach (InventorySlot slot in Enum.GetValues<InventorySlot>())
          if(target.GetItemInSlot(slot) is not null)
            foreach (var ip in target.GetItemInSlot(slot)?.ItemProperties)
              if (ip.Property.PropertyType == ItemPropertyType.ImmunityDamageType
                && ItemUtils.GetDamageTypeFromItemProperty((IPDamageType)ip.SubType.RowIndex) == damageType)
                return damage *= 2;
        
        return damage;
    }
  }
}
