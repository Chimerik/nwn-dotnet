using System.Collections.Generic;
using System.Linq;
using Anvil.API;
using NWN.Core.NWNX;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static List<NwGameObject> InvocationBestiale(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, Location targetLocation)
    {
      List<NwGameObject> concentrationList = new();

      if (oCaster is not NwCreature caster)
        return concentrationList;

      SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);

      NwCreature summon = CreatureUtils.SummonAssociate(caster, AssociateType.Summoned, "esprit_bestial");

      NwItem weapon = summon.GetItemInSlot(InventorySlot.CreatureLeftWeapon);
      weapon.GetObjectVariable<LocalVariableInt>(ItemConfig.IsFinesseWeaponVariable).Value = 1;
      weapon.AddItemProperty(ItemProperty.DamageBonus(IPDamageType.Piercing, IPDamageBonus.Plus1d8), EffectDuration.Permanent);
      weapon.AddItemProperty(ItemProperty.DamageBonus(IPDamageType.Piercing, IPDamageBonus.Plus2), EffectDuration.Permanent);

      return concentrationList;
    }
  }
}
