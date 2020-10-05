using System;
using NWN.Enums;
using NWN.Enums.Item;
using NWN.Enums.Item.Property;

namespace NWN
{
  class ItemPropertyUtils
  {
    //ItemProperty CreateItemProperty(ItemPropertyType nPropID, int nParam1 = 0, int nParam2 = 0, int nParam3 = 0, int nParam4 = 0)
    //{
    //  switch (nPropID)
    //  {
    //    default: throw new Exception($"Unimplemented property type='{nPropID}'");

    //    case ItemPropertyType.AttackBonus: return NWScript.ItemPropertyAttackBonus(nParam1);
    //    case ItemPropertyType.AbilityBonus: return NWScript.ItemPropertyAbilityBonus(nParam1, nParam2);
    //    case ItemPropertyType.ACBonus: return NWScript.ItemPropertyACBonus(nParam1);
    //    case ItemPropertyType.BaseItemWeightReduction: return NWScript.ItemPropertyWeightReduction(nParam1);
    //    case ItemPropertyType.BonusSpellSlotOfLevelN: return NWScript.ItemPropertyBonusLevelSpell(nParam1, nParam2);
    //    case ItemPropertyType.DamageBonus: return NWScript.ItemPropertyDamageBonus(nParam1, nParam2);
    //    case ItemPropertyType.EnhancementBonus: return NWScript.ItemPropertyEnhancementBonus(nParam1);
    //    case ItemPropertyType.ExtraMeleeDamageType: return NWScript.ItemPropertyExtraMeleeDamageType(nParam1);
    //    case ItemPropertyType.ExtraRangedDamageType: return NWScript.ItemPropertyExtraRangeDamageType(nParam1);
    //    case ItemPropertyType.Haste: return NWScript.ItemPropertyHaste();
    //    case ItemPropertyType.Keen: return NWScript.ItemPropertyKeen();
    //    case ItemPropertyType.MassiveCriticals: return NWScript.ItemPropertyMassiveCritical(nParam1);
    //    case ItemPropertyType.Mighty: return NWScript.ItemPropertyMaxRangeStrengthMod(nParam1);
    //    case ItemPropertyType.MindBlank: return NWScript.ItemPropertyImmunityMisc(IP_CONST_IMMUNITYMISC_MINDSPELLS);
    //    case ItemPropertyType.Regeneration: return NWScript.ItemPropertyRegeneration(nParam1);
    //    case ItemPropertyType.RegenerationVampiric: return NWScript.ItemPropertyVampiricRegeneration(nParam1);
    //    case ItemPropertyType.SavingThrowBonus: return NWScript.ItemPropertyBonusSavingThrow(nParam1, nParam2);
    //    case ItemPropertyType.SavingThrowBonusSpecific: return NWScript.ItemPropertyBonusSavingThrowVsX(nParam1, nParam2);
    //    case ItemPropertyType.SkillBonus: return NWScript.ItemPropertySkillBonus(nParam1, nParam2);
    //    case ItemPropertyType.UnlimitedAmmunition: return NWScript.ItemPropertyUnlimitedAmmo(nParam1);
    //  }
    //}

    float GetAverageDamageFromDamageBonus(DamageBonus damageBonus)
    {
      switch (damageBonus)
      {
        default: return 0.0f;

        case DamageBonus.DAMAGEBONUS_1: return 1.0f;
        case DamageBonus.DAMAGEBONUS_2: return 2.0f;
        case DamageBonus.DAMAGEBONUS_3: return 3.0f;
        case DamageBonus.DAMAGEBONUS_4: return 4.0f;
        case DamageBonus.DAMAGEBONUS_5: return 5.0f;
        case DamageBonus.DAMAGEBONUS_6: return 6.0f;
        case DamageBonus.DAMAGEBONUS_7: return 7.0f;
        case DamageBonus.DAMAGEBONUS_8: return 8.0f;
        case DamageBonus.DAMAGEBONUS_9: return 9.0f;
        case DamageBonus.DAMAGEBONUS_10: return 10.0f;
        case DamageBonus.DAMAGEBONUS_1d4: return 2.5f;
        case DamageBonus.DAMAGEBONUS_1d6: return 3.5f;
        case DamageBonus.DAMAGEBONUS_1d8: return 4.5f;
        case DamageBonus.DAMAGEBONUS_1d10: return 5.5f;
        case DamageBonus.DAMAGEBONUS_1d12: return 6.5f;
        case DamageBonus.DAMAGEBONUS_2d4: return 5.0f;
        case DamageBonus.DAMAGEBONUS_2d6: return 7.0f;
        case DamageBonus.DAMAGEBONUS_2d8: return 9.0f;
        case DamageBonus.DAMAGEBONUS_2d10: return 11.0f;
        case DamageBonus.DAMAGEBONUS_2d12: return 13.0f;
      }
    }

    DamageBonus GetNextAverageDamageBonus(DamageBonus damageBonus)
    {
      switch (damageBonus)
      {
        default: return DamageBonus.DAMAGEBONUS_1;
        case DamageBonus.DAMAGEBONUS_1: return DamageBonus.DAMAGEBONUS_2;
        case DamageBonus.DAMAGEBONUS_2: return DamageBonus.DAMAGEBONUS_1d4;
        case DamageBonus.DAMAGEBONUS_3: return DamageBonus.DAMAGEBONUS_1d6;
        case DamageBonus.DAMAGEBONUS_4: return DamageBonus.DAMAGEBONUS_1d8;
        case DamageBonus.DAMAGEBONUS_5: return DamageBonus.DAMAGEBONUS_1d10;
        case DamageBonus.DAMAGEBONUS_6: return DamageBonus.DAMAGEBONUS_1d12;
        case DamageBonus.DAMAGEBONUS_7: return DamageBonus.DAMAGEBONUS_8;
        case DamageBonus.DAMAGEBONUS_8: return DamageBonus.DAMAGEBONUS_2d8;
        case DamageBonus.DAMAGEBONUS_9: return DamageBonus.DAMAGEBONUS_10;
        case DamageBonus.DAMAGEBONUS_10: return DamageBonus.DAMAGEBONUS_2d10;
        case DamageBonus.DAMAGEBONUS_1d4: return DamageBonus.DAMAGEBONUS_3;
        case DamageBonus.DAMAGEBONUS_1d6: return DamageBonus.DAMAGEBONUS_4;
        case DamageBonus.DAMAGEBONUS_1d8: return DamageBonus.DAMAGEBONUS_2d4;
        case DamageBonus.DAMAGEBONUS_1d10: return DamageBonus.DAMAGEBONUS_6;
        case DamageBonus.DAMAGEBONUS_1d12: return DamageBonus.DAMAGEBONUS_2d6;
        case DamageBonus.DAMAGEBONUS_2d4: return DamageBonus.DAMAGEBONUS_6;
        case DamageBonus.DAMAGEBONUS_2d6: return DamageBonus.DAMAGEBONUS_8;
        case DamageBonus.DAMAGEBONUS_2d8: return DamageBonus.DAMAGEBONUS_10;
        case DamageBonus.DAMAGEBONUS_2d10: return DamageBonus.DAMAGEBONUS_2d12;
        case DamageBonus.DAMAGEBONUS_2d12: return DamageBonus.DAMAGEBONUS_2d12;
      }
    }

    string DamageBonusToString(DamageBonus damageBonus)
    {
      switch (damageBonus)
      {
        default: return "";
        case DamageBonus.DAMAGEBONUS_1: return "1";
        case DamageBonus.DAMAGEBONUS_2: return "2";
        case DamageBonus.DAMAGEBONUS_3: return "3";
        case DamageBonus.DAMAGEBONUS_4: return "4";
        case DamageBonus.DAMAGEBONUS_5: return "5";
        case DamageBonus.DAMAGEBONUS_6: return "6";
        case DamageBonus.DAMAGEBONUS_7: return "7";
        case DamageBonus.DAMAGEBONUS_8: return "8";
        case DamageBonus.DAMAGEBONUS_9: return "9";
        case DamageBonus.DAMAGEBONUS_10: return "10";
        case DamageBonus.DAMAGEBONUS_1d4: return "1d4";
        case DamageBonus.DAMAGEBONUS_1d6: return "1d6";
        case DamageBonus.DAMAGEBONUS_1d8: return "1d8";
        case DamageBonus.DAMAGEBONUS_1d10: return "1d10";
        case DamageBonus.DAMAGEBONUS_1d12: return "1d12";
        case DamageBonus.DAMAGEBONUS_2d4: return "2d4";
        case DamageBonus.DAMAGEBONUS_2d6: return "2d6";
        case DamageBonus.DAMAGEBONUS_2d8: return "2d8";
        case DamageBonus.DAMAGEBONUS_2d10: return "2d10";
        case DamageBonus.DAMAGEBONUS_2d12: return "2d12";
      }
    }

    //int DamageBonusStringToInt(string sDamageBonus)
    //{
    //  if (sDamageBonus == "1") return IP_CONST_DAMAGEBONUS_1;
    //  if (sDamageBonus == "2") return IP_CONST_DAMAGEBONUS_2;
    //  if (sDamageBonus == "3") return IP_CONST_DAMAGEBONUS_3;
    //  if (sDamageBonus == "4") return IP_CONST_DAMAGEBONUS_4;
    //  if (sDamageBonus == "5") return IP_CONST_DAMAGEBONUS_5;
    //  if (sDamageBonus == "6") return IP_CONST_DAMAGEBONUS_6;
    //  if (sDamageBonus == "7") return IP_CONST_DAMAGEBONUS_7;
    //  if (sDamageBonus == "8") return IP_CONST_DAMAGEBONUS_8;
    //  if (sDamageBonus == "9") return IP_CONST_DAMAGEBONUS_9;
    //  if (sDamageBonus == "10") return IP_CONST_DAMAGEBONUS_10;
    //  if (sDamageBonus == "1d4") return IP_CONST_DAMAGEBONUS_1d4;
    //  if (sDamageBonus == "1d6") return IP_CONST_DAMAGEBONUS_1d6;
    //  if (sDamageBonus == "1d8") return IP_CONST_DAMAGEBONUS_1d8;
    //  if (sDamageBonus == "1d10") return IP_CONST_DAMAGEBONUS_1d10;
    //  if (sDamageBonus == "1d12") return IP_CONST_DAMAGEBONUS_1d12;
    //  if (sDamageBonus == "2d4") return IP_CONST_DAMAGEBONUS_2d4;
    //  if (sDamageBonus == "2d6") return IP_CONST_DAMAGEBONUS_2d6;
    //  if (sDamageBonus == "2d8") return IP_CONST_DAMAGEBONUS_2d8;
    //  if (sDamageBonus == "2d10") return IP_CONST_DAMAGEBONUS_2d10;
    //  if (sDamageBonus == "2d12") return IP_CONST_DAMAGEBONUS_2d12;

    //  return 0;
    //}

    public static void ReplaceItemProperty(uint oItem, ItemProperty ip, float fDuration = 0.0f, bool bIgnoreSubType = false)
    {
      int nType = NWScript.GetItemPropertyType(ip);
      int nSubType = NWScript.GetItemPropertySubType(ip);
      // if duration is 0.0f, make the item property permanent
      DurationType nDuration = fDuration == 0.0f ? DurationType.Permanent : DurationType.Temporary;

      // remove any matching properties
      if (bIgnoreSubType)
      {
        nSubType = -1;
      }
      RemoveMatchingItemProperties(oItem, nType, nDuration, nSubType);

      if (nDuration == DurationType.Permanent)
      {
        NWScript.AddItemProperty(nDuration, ip, oItem);
      }
      else
      {
        NWScript.AddItemProperty(nDuration, ip, oItem, fDuration);
      }
    }

    public static void RemoveMatchingItemProperties(uint oItem, int nItemPropertyType, DurationType nItemPropertyDuration = DurationType.Temporary, int nItemPropertySubType = -1)
    {
      var ip = NWScript.GetFirstItemProperty(oItem);

      while (NWScript.GetIsItemPropertyValid(ip) == 1)
      {
        if (
          NWScript.GetItemPropertyType(ip) == nItemPropertyType &&
          NWScript.GetItemPropertyDurationType(ip) == (int)nItemPropertyDuration &&
          (NWScript.GetItemPropertySubType(ip) == nItemPropertySubType || nItemPropertySubType == -1)
        )
        {
          NWScript.RemoveItemProperty(oItem, ip);
        }
        ip = NWScript.GetNextItemProperty(oItem);
      }
    }
  }
}
