using System.Collections.Generic;
using System.Linq;
using Anvil.API;

namespace NWN
{
  public static class ItemPropertyUtils
  {
    public static float GetAverageDamageFromDamageBonus(IPDamageBonus damageBonus)
    {
      switch (damageBonus)
      {
        default: return 0.0f;

        case IPDamageBonus.Plus1: return 1.0f;
        case IPDamageBonus.Plus2: return 2.0f;
        case IPDamageBonus.Plus3: return 3.0f;
        case IPDamageBonus.Plus4: return 4.0f;
        case IPDamageBonus.Plus5: return 5.0f;
        case IPDamageBonus.Plus6: return 6.0f;
        case IPDamageBonus.Plus7: return 7.0f;
        case IPDamageBonus.Plus8: return 8.0f;
        case IPDamageBonus.Plus9: return 9.0f;
        case IPDamageBonus.Plus10: return 10.0f;
        case IPDamageBonus.Plus1d4: return 2.5f;
        case IPDamageBonus.Plus1d6: return 3.5f;
        case IPDamageBonus.Plus1d8: return 4.5f;
        case IPDamageBonus.Plus1d10: return 5.5f;
        case IPDamageBonus.Plus1d12: return 6.5f;
        case IPDamageBonus.Plus2d4: return 5.0f;
        case IPDamageBonus.Plus2d6: return 7.0f;
        case IPDamageBonus.Plus2d8: return 9.0f;
        case IPDamageBonus.Plus2d10: return 11.0f;
        case IPDamageBonus.Plus2d12: return 13.0f;
      }
    }

    public static IPDamageBonus GetNextAverageDamageBonus(IPDamageBonus damageBonus)
    {
      switch (damageBonus)
      {
        default: return IPDamageBonus.Plus1;

        case IPDamageBonus.Plus1: return IPDamageBonus.Plus2;
        case IPDamageBonus.Plus2: return IPDamageBonus.Plus1d4;
        case IPDamageBonus.Plus3: return IPDamageBonus.Plus1d6;
        case IPDamageBonus.Plus4: return IPDamageBonus.Plus1d8;
        case IPDamageBonus.Plus5: return IPDamageBonus.Plus1d10;
        case IPDamageBonus.Plus6: return IPDamageBonus.Plus1d12;
        case IPDamageBonus.Plus7: return IPDamageBonus.Plus8;
        case IPDamageBonus.Plus8: return IPDamageBonus.Plus2d8;
        case IPDamageBonus.Plus9: return IPDamageBonus.Plus10;
        case IPDamageBonus.Plus10: return IPDamageBonus.Plus2d10;
        case IPDamageBonus.Plus1d4: return IPDamageBonus.Plus3;
        case IPDamageBonus.Plus1d6: return IPDamageBonus.Plus4;
        case IPDamageBonus.Plus1d8: return IPDamageBonus.Plus2d4;
        case IPDamageBonus.Plus1d10: return IPDamageBonus.Plus6;
        case IPDamageBonus.Plus1d12: return IPDamageBonus.Plus2d6;
        case IPDamageBonus.Plus2d4: return IPDamageBonus.Plus6;
        case IPDamageBonus.Plus2d6: return IPDamageBonus.Plus8;
        case IPDamageBonus.Plus2d8: return IPDamageBonus.Plus10;
        case IPDamageBonus.Plus2d10: return IPDamageBonus.Plus2d12;
        case IPDamageBonus.Plus2d12: return IPDamageBonus.Plus2d12;
      }
    }

    public static string DamageBonusToString(IPDamageBonus damageBonus)
    {
      switch (damageBonus)
      {
        default: return "";

        case IPDamageBonus.Plus1: return "1";
        case IPDamageBonus.Plus2: return "2";
        case IPDamageBonus.Plus3: return "3";
        case IPDamageBonus.Plus4: return "4";
        case IPDamageBonus.Plus5: return "5";
        case IPDamageBonus.Plus6: return "6";
        case IPDamageBonus.Plus7: return "7";
        case IPDamageBonus.Plus8: return "8";
        case IPDamageBonus.Plus9: return "9";
        case IPDamageBonus.Plus10: return "10";
        case IPDamageBonus.Plus1d4: return "1d4";
        case IPDamageBonus.Plus1d6: return "1d6";
        case IPDamageBonus.Plus1d8: return "1d8";
        case IPDamageBonus.Plus1d10: return "1d10";
        case IPDamageBonus.Plus1d12: return "1d12";
        case IPDamageBonus.Plus2d4: return "2d4";
        case IPDamageBonus.Plus2d6: return "2d6";
        case IPDamageBonus.Plus2d8: return "2d8";
        case IPDamageBonus.Plus2d10: return "2d10";
        case IPDamageBonus.Plus2d12: return "2d12";
      }
    }

    public static string AbilityBonusToString(IPAbility abilityBonusType)
    {
      switch (abilityBonusType)
      {
        default: return "";

        case IPAbility.Charisma: return "charisme";
        case IPAbility.Constitution: return "constitution";
        case IPAbility.Dexterity: return "dexterite";
        case IPAbility.Intelligence: return "intelligence";
        case IPAbility.Strength: return "force";
        case IPAbility.Wisdom: return "sagesse";
      }
    }

    public static string DamageBonusTypeToString(IPDamageType damageBonusType)
    {
      switch (damageBonusType)
      {
        default: return "";

        case IPDamageType.Acid: return "acide";
        case IPDamageType.Bludgeoning: return "contondant";
        case IPDamageType.Cold: return "froid";
        case IPDamageType.Divine: return "divin";
        case IPDamageType.Electrical: return "electrique";
        case IPDamageType.Fire: return "feu";
        case IPDamageType.Magical: return "magique";
        case IPDamageType.Negative: return "energie negative";
        case IPDamageType.Piercing: return "percant";
        case IPDamageType.Positive: return "energie positive";
        case IPDamageType.Slashing: return "tranchant";
        case IPDamageType.Sonic: return "sonique";
      }
    }

    public static string SavingThrowBonusToString(IPSaveBaseType savingThrowBonusType)
    {
      switch (savingThrowBonusType)
      {
        default: return "";

        case IPSaveBaseType.Fortitude: return "vigueur";
        case IPSaveBaseType.Will: return "volonté";
        case IPSaveBaseType.Reflex: return "réflexe";
      }
    }

    public static void ReplaceItemProperty(NwItem oItem, ItemProperty ip)
    {
      List<ItemProperty> sortedIP = oItem.ItemProperties.Where(i => i.PropertyType == ip.PropertyType && i.SubType == ip.SubType && i.DurationType == ip.DurationType).ToList();
      ItemProperty maxIP = sortedIP.OrderByDescending(i => i.CostTableValue).FirstOrDefault();

      if (maxIP != null)
        maxIP.CostTableValue = ip.CostTableValue;
    }
  }
}
