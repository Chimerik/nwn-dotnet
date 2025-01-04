using Anvil.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void MaitreDeLaChaine(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry)
    {
      if (oCaster is not NwCreature caster)
        return;

      var familiar = caster.GetAssociate(AssociateType.Familiar);

      if (familiar is null)
      {
        caster.LoginPlayer?.SendServerMessage("Votre familier n'est pas invoqué", ColorConstants.Red);
        return;
      }

      SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);

      NwItem weapon = familiar.GetItemInSlot(InventorySlot.CreatureLeftWeapon);

      if (weapon is null)
        return;

      var damageType = spell.Id switch
      {
        CustomSpell.MaitreDeLaChaineRadiant => IPDamageType.Divine,
        _ => CustomItemPropertyDamageType.Necrotic,
      };

      foreach (var ip in weapon.ItemProperties)
      {
        if(ip.Property.PropertyType == ItemPropertyType.DamageBonus)
        {
          switch((IPDamageType)ip.IntParams[1])
          {
            case IPDamageType.Piercing:
            case IPDamageType.Slashing:
            case IPDamageType.Bludgeoning:
            case IPDamageType.Divine:
            case CustomItemPropertyDamageType.Necrotic:

              weapon.AddItemProperty(ItemProperty.DamageBonus(damageType, (IPDamageBonus)ip.IntParams[3]), EffectDuration.Permanent);
              weapon.RemoveItemProperty(ip);

              break;
          }
        }
      }
    }
  }
}
