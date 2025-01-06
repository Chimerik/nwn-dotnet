using System.Collections.Generic;
using System.Linq;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void ArmeMagique(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwGameObject oTarget)
    {
      if (oCaster is not NwCreature caster)
        return;

      SpellUtils.SignalEventSpellCast(oCaster, oTarget, spell.SpellType);

      NwItem targetWeapon = null;

      if (oTarget is NwItem target)
        targetWeapon = target;          
      else if (oTarget is NwCreature targetCreature)
        targetWeapon = targetCreature.GetItemInSlot(InventorySlot.RightHand);

      if (targetWeapon is null || !ItemUtils.IsWeapon(targetWeapon.BaseItem) 
        || targetWeapon.ItemProperties.Any(ip => ip.Property.PropertyType == ItemPropertyType.EnhancementBonus)
        || targetWeapon.Possessor is null)
      {
        caster.LoginPlayer?.SendServerMessage("Cible invalide", ColorConstants.Red);
        return;
      }

      NwItem previousWeapon = caster.GetObjectVariable<LocalVariableObject<NwItem>>("_CURRENT_MAGIC_WEAPON").Value;

      if(previousWeapon is not null)
        previousWeapon.RemoveItemProperties(ItemPropertyType.EnhancementBonus);
      else
        ModuleSystem.magicWeaponsToRemove.Add(previousWeapon.UUID);
      
      caster.GetObjectVariable<LocalVariableObject<NwItem>>("_CURRENT_MAGIC_WEAPON").Value = targetWeapon;

      NWScript.AssignCommand(oCaster, () => targetWeapon.AddItemProperty(ItemProperty.EnhancementBonus(1), EffectDuration.Temporary, SpellUtils.GetSpellDuration(oCaster, spellEntry)));
      targetWeapon.Possessor.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpSuperHeroism));  
    }
  }
}
