using NWN.Core;

namespace NWN
{
  public static class ItemPropertyUtils
  {
    public enum DamageBonus
    {
      None = 0,
      D1 = NWScript.IP_CONST_DAMAGEBONUS_1,
      D2 = NWScript.IP_CONST_DAMAGEBONUS_2,
      D3 = NWScript.IP_CONST_DAMAGEBONUS_3,
      D4 = NWScript.IP_CONST_DAMAGEBONUS_4,
      D5 = NWScript.IP_CONST_DAMAGEBONUS_5,
      D6 = NWScript.IP_CONST_DAMAGEBONUS_6,
      D7 = NWScript.IP_CONST_DAMAGEBONUS_7,
      D8 = NWScript.IP_CONST_DAMAGEBONUS_8,
      D9 = NWScript.IP_CONST_DAMAGEBONUS_9,
      D10 = NWScript.IP_CONST_DAMAGEBONUS_10,
      D1d4 = NWScript.IP_CONST_DAMAGEBONUS_1d4,
      D1d6 = NWScript.IP_CONST_DAMAGEBONUS_1d6,
      D1d8 = NWScript.IP_CONST_DAMAGEBONUS_1d8,
      D1d10 = NWScript.IP_CONST_DAMAGEBONUS_1d10,
      D1d12 = NWScript.IP_CONST_DAMAGEBONUS_1d12,
      D2d4 = NWScript.IP_CONST_DAMAGEBONUS_2d4,
      D2d6 = NWScript.IP_CONST_DAMAGEBONUS_2d6,
      D2d8 = NWScript.IP_CONST_DAMAGEBONUS_2d8,
      D2d10 = NWScript.IP_CONST_DAMAGEBONUS_2d10,
      D2d12 = NWScript.IP_CONST_DAMAGEBONUS_2d12,
    }

    public static float GetAverageDamageFromDamageBonus(DamageBonus damageBonus)
    {
      switch (damageBonus)
      {
        default: return 0.0f;

        case DamageBonus.None: return 0.0f;
        case DamageBonus.D1: return 1.0f;
        case DamageBonus.D2: return 2.0f;
        case DamageBonus.D3: return 3.0f;
        case DamageBonus.D4: return 4.0f;
        case DamageBonus.D5: return 5.0f;
        case DamageBonus.D6: return 6.0f;
        case DamageBonus.D7: return 7.0f;
        case DamageBonus.D8: return 8.0f;
        case DamageBonus.D9: return 9.0f;
        case DamageBonus.D10: return 10.0f;
        case DamageBonus.D1d4: return 2.5f;
        case DamageBonus.D1d6: return 3.5f;
        case DamageBonus.D1d8: return 4.5f;
        case DamageBonus.D1d10: return 5.5f;
        case DamageBonus.D1d12: return 6.5f;
        case DamageBonus.D2d4: return 5.0f;
        case DamageBonus.D2d6: return 7.0f;
        case DamageBonus.D2d8: return 9.0f;
        case DamageBonus.D2d10: return 11.0f;
        case DamageBonus.D2d12: return 13.0f;
      }
    }

    public static DamageBonus GetNextAverageDamageBonus(DamageBonus damageBonus)
    {
      switch (damageBonus)
      {
        default: return DamageBonus.D1;

        case DamageBonus.None: return DamageBonus.D1;
        case DamageBonus.D1: return DamageBonus.D2;
        case DamageBonus.D2: return DamageBonus.D1d4;
        case DamageBonus.D3: return DamageBonus.D1d6;
        case DamageBonus.D4: return DamageBonus.D1d8;
        case DamageBonus.D5: return DamageBonus.D1d10;
        case DamageBonus.D6: return DamageBonus.D1d12;
        case DamageBonus.D7: return DamageBonus.D8;
        case DamageBonus.D8: return DamageBonus.D2d8;
        case DamageBonus.D9: return DamageBonus.D10;
        case DamageBonus.D10: return DamageBonus.D2d10;
        case DamageBonus.D1d4: return DamageBonus.D3;
        case DamageBonus.D1d6: return DamageBonus.D4;
        case DamageBonus.D1d8: return DamageBonus.D2d4;
        case DamageBonus.D1d10: return DamageBonus.D6;
        case DamageBonus.D1d12: return DamageBonus.D2d6;
        case DamageBonus.D2d4: return DamageBonus.D6;
        case DamageBonus.D2d6: return DamageBonus.D8;
        case DamageBonus.D2d8: return DamageBonus.D10;
        case DamageBonus.D2d10: return DamageBonus.D2d12;
        case DamageBonus.D2d12: return DamageBonus.D2d12;
      }
    }

    public static string DamageBonusToString(DamageBonus damageBonus)
    {
      switch (damageBonus)
      {
        default: return "";

        case DamageBonus.None: return "";
        case DamageBonus.D1: return "1";
        case DamageBonus.D2: return "2";
        case DamageBonus.D3: return "3";
        case DamageBonus.D4: return "4";
        case DamageBonus.D5: return "5";
        case DamageBonus.D6: return "6";
        case DamageBonus.D7: return "7";
        case DamageBonus.D8: return "8";
        case DamageBonus.D9: return "9";
        case DamageBonus.D10: return "10";
        case DamageBonus.D1d4: return "1d4";
        case DamageBonus.D1d6: return "1d6";
        case DamageBonus.D1d8: return "1d8";
        case DamageBonus.D1d10: return "1d10";
        case DamageBonus.D1d12: return "1d12";
        case DamageBonus.D2d4: return "2d4";
        case DamageBonus.D2d6: return "2d6";
        case DamageBonus.D2d8: return "2d8";
        case DamageBonus.D2d10: return "2d10";
        case DamageBonus.D2d12: return "2d12";
      }
    }

    public static string AbilityBonusToString(int abilityBonusType)
    {
      switch (abilityBonusType)
      {
        default: return "";

        case NWScript.ABILITY_CHARISMA: return "charisme";
        case NWScript.ABILITY_CONSTITUTION: return "constitution";
        case NWScript.ABILITY_DEXTERITY: return "dexterite";
        case NWScript.ABILITY_INTELLIGENCE: return "intelligence";
        case NWScript.ABILITY_STRENGTH: return "force";
        case NWScript.ABILITY_WISDOM: return "sagesse";
      }
    }

    public static string DamageBonusTypeToString(int damageBonusType)
    {
      switch (damageBonusType)
      {
        default: return "";

        case NWScript.IP_CONST_DAMAGETYPE_ACID: return "acide";
        case NWScript.IP_CONST_DAMAGETYPE_BLUDGEONING: return "contondant";
        case NWScript.IP_CONST_DAMAGETYPE_COLD: return "froid";
        case NWScript.IP_CONST_DAMAGETYPE_DIVINE: return "divin";
        case NWScript.IP_CONST_DAMAGETYPE_ELECTRICAL: return "electrique";
        case NWScript.IP_CONST_DAMAGETYPE_FIRE: return "feu";
        case NWScript.IP_CONST_DAMAGETYPE_MAGICAL: return "magique";
        case NWScript.IP_CONST_DAMAGETYPE_NEGATIVE: return "energie negative";
        case NWScript.IP_CONST_DAMAGETYPE_PIERCING: return "percant";
        case NWScript.IP_CONST_DAMAGETYPE_POSITIVE: return "energie positive";
        case NWScript.IP_CONST_DAMAGETYPE_SLASHING: return "tranchant";
        case NWScript.IP_CONST_DAMAGETYPE_SONIC: return "sonique";
      }
    }

    public static string SavingThrowBonusToString(int savingThrowBonusType)
    {
      switch (savingThrowBonusType)
      {
        default: return "";

        case NWScript.IP_CONST_SAVEBASETYPE_FORTITUDE: return "vigueur";
        case NWScript.IP_CONST_SAVEBASETYPE_WILL: return "volonte";
        case NWScript.IP_CONST_SAVEBASETYPE_REFLEX: return "reflexe";
      }
    }

    public static void ReplaceItemProperty(uint oItem, ItemProperty ip, float fDuration = 0.0f, bool bIgnoreSubType = false)
    {
      int nType = NWScript.GetItemPropertyType(ip);
      int nSubType = NWScript.GetItemPropertySubType(ip);
      // if duration is 0.0f, make the item property permanent
      int nDuration = fDuration == 0.0f ? NWScript.DURATION_TYPE_PERMANENT : NWScript.DURATION_TYPE_TEMPORARY;

      // remove any matching properties
      if (bIgnoreSubType)
      {
        nSubType = -1;
      }
      RemoveMatchingItemProperties(oItem, nType, nDuration, nSubType);

      if (nDuration == NWScript.DURATION_TYPE_PERMANENT)
      {
        NWScript.AddItemProperty(nDuration, ip, oItem);
      }
      else
      {
        NWScript.AddItemProperty(nDuration, ip, oItem, fDuration);
      }
    }

    public static void RemoveMatchingItemProperties(uint oItem, int nItemPropertyType, int nItemPropertyDuration = NWScript.DURATION_TYPE_PERMANENT, int nItemPropertySubType = -1)
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
