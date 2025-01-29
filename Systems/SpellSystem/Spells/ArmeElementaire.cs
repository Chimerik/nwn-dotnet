using System;
using System.Collections.Generic;
using System.Linq;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static List<NwGameObject> ArmeElementaire(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwGameObject oTarget)
    {
      List<NwGameObject> concentrationList = new();

      if (oCaster is not NwCreature caster)
        return concentrationList;

      SpellUtils.SignalEventSpellCast(oCaster, oTarget, spell.SpellType);

      NwItem targetWeapon = null;

      if (oTarget is NwItem target)
        targetWeapon = target;          
      else if (oTarget is NwCreature targetCreature)
        targetWeapon = targetCreature.GetItemInSlot(InventorySlot.RightHand);

      if (targetWeapon is null || !ItemUtils.IsWeapon(targetWeapon.BaseItem) 
        || targetWeapon.ItemProperties.Any(ip => ip.Property.PropertyType == ItemPropertyType.EnhancementBonus || ip.Property.PropertyType == ItemPropertyType.DamageBonus)
        || targetWeapon.Possessor is null)
      {
        caster.LoginPlayer?.SendServerMessage("Cible invalide", ColorConstants.Red);
        return concentrationList;
      }

      var previousUUID = caster.GetObjectVariable<LocalVariableGuid>("_CURRENT_MAGIC_WEAPON").Value;

      if (previousUUID !=  Guid.Empty)
      {
        NwItem previousWeapon = GuidExtensions.ToNwObject<NwItem>(previousUUID);
        
        if (previousWeapon is null)
          ModuleSystem.magicWeaponsToRemoveList.Add(previousUUID);
        else
        {
          previousWeapon.RemoveItemProperties(ItemPropertyType.EnhancementBonus, durationType: EffectDuration.Temporary);
          ModuleSystem.magicWeaponsToRemoveList.Remove(previousWeapon.UUID);
        }
      }

      NWScript.AssignCommand(oCaster, () => targetWeapon.AddItemProperty(ItemProperty.EnhancementBonus(1), EffectDuration.Temporary, SpellUtils.GetSpellDuration(oCaster, spellEntry)));
      
      var damageType = spell.Id switch
      {
        CustomSpell.ArmeElementaireAcide => IPDamageType.Acid,
        CustomSpell.ArmeElementaireFroid => IPDamageType.Cold,
        CustomSpell.ArmeElementaireElec => IPDamageType.Electrical,
        CustomSpell.ArmeElementaireTonnerre => IPDamageType.Sonic,
        _ => IPDamageType.Fire,
      };

      NWScript.AssignCommand(oCaster, () => targetWeapon.AddItemProperty(ItemProperty.EnhancementBonus(1), EffectDuration.Temporary, SpellUtils.GetSpellDuration(oCaster, spellEntry)));
      NWScript.AssignCommand(oCaster, () => targetWeapon.AddItemProperty(ItemProperty.DamageBonus(damageType, IPDamageBonus.Plus1d4), EffectDuration.Temporary, SpellUtils.GetSpellDuration(oCaster, spellEntry)));
      
      targetWeapon.Possessor.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpSuperHeroism));
      concentrationList.Add(targetWeapon);

      return concentrationList;
    }
  }
}
