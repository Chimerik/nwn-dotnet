using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class ItemSystem
  {
    public static void OnEquipMaitreArmureIntermediaire(OnItemEquip onEquip)
    {
      NwCreature oPC = onEquip.EquippedBy;
      NwItem oItem = onEquip.Item;

      if (oPC is null || oItem is null || oItem.BaseItem.ItemType != BaseItemType.Armor || oItem.BaseACValue < 2 || oItem.BaseACValue > 5)
        return;

      oPC.OnHeartbeat -= OnHBCheckMaitreArmureIntermediaire;
      oPC.OnHeartbeat += OnHBCheckMaitreArmureIntermediaire;

      oPC.OnItemUnequip -= OnUnEquipMaitreArmureIntermediaire;
      oPC.OnItemUnequip += OnUnEquipMaitreArmureIntermediaire;

      if (oPC.GetAbilityModifier(Ability.Dexterity) > 2)
        oPC.ApplyEffect(EffectDuration.Permanent, EffectSystem.maitreArmureIntermediaire);
    }
    public static void OnUnEquipMaitreArmureIntermediaire(OnItemUnequip onUnequip)
    {
      NwCreature oPC = onUnequip.Creature;
      NwItem oItem = onUnequip.Item;

      if (oPC is null || oItem is null || oItem.BaseItem.ItemType != BaseItemType.Armor || oItem.BaseACValue < 2 || oItem.BaseACValue > 5)
        return;

      oPC.OnHeartbeat -= OnHBCheckMaitreArmureIntermediaire;
      oPC.OnItemEquip -= OnEquipMaitreArmureIntermediaire;
      oPC.OnItemEquip += OnEquipMaitreArmureIntermediaire;

      foreach(var eff in oPC.ActiveEffects)
        if(eff.Tag == EffectSystem.MaitreArmureIntermediaireEffectTag)
          oPC.RemoveEffect(eff);
    }
  }
}
