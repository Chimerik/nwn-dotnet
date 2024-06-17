using System;
using Anvil.API;

namespace NWN.Systems
{
  public static partial class SpellUtils
  {
    public static int HandleResistanceBypass(NwCreature target, bool isElementalist, int damage, DamageType damageType)
    {
      if (!isElementalist || damage < 1)
        return damage;

      foreach (var eff in target.ActiveEffects)
        if (eff.EffectType == EffectType.DamageImmunityIncrease && eff.IntParams[0] == (int)damageType)
          return damage *= 2;

        foreach (InventorySlot slot in Enum.GetValues<InventorySlot>())
          foreach (var ip in target.GetItemInSlot(slot)?.ItemProperties)
            if (ip.Property.PropertyType == ItemPropertyType.ImmunityDamageType
              && ItemUtils.GetDamageTypeFromItemProperty((IPDamageType)ip.SubType.RowIndex) == damageType)
              return damage *= 2;
        
        return damage;
    }
  }
}
