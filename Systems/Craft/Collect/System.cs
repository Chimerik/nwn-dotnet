﻿using System;
using System.Collections.Generic;
using static NWN.Systems.Craft.Collect.Config;
using Anvil.API;
using NLog;
using System.Linq;

namespace NWN.Systems.Craft.Collect
{
  public static class System
  {
    public static readonly Logger Log = LogManager.GetCurrentClassLogger();
    public static readonly string[] badPelts = new string[] { "paraceratherium", "ankheg", "gorille", "giantlizard" };
    public static readonly string[] commonPelts = new string[] { "alligator", "crocodile", "crocblinde", "varan" };
    public static readonly string[] normalPelts = new string[] { "basilisk", "jhakar", "gorgon", "bulette", "dagon" };

    public static readonly CraftResource[] craftResourceArray = new CraftResource[] 
    { 
      new CraftResource(ResourceType.Ore, "Un phénomène mystérieux provoque l'agglomération de Substance à certains minerais bruts, qu'on appelle alors 'matéria'.\n\nCette matéria brute doit être raffinée avant de pouvoir être utilisée par un artisan.", 82, 1, new decimal(0.5)),
      new CraftResource(ResourceType.Ore, "Un phénomène mystérieux provoque l'agglomération de Substance à certains minerais bruts, qu'on appelle alors 'matéria'.\n\nCette matéria brute doit être raffinée avant de pouvoir être utilisée par un artisan.", 82, 2, new decimal(0.5)),
      new CraftResource(ResourceType.Ore, "Un phénomène mystérieux provoque l'agglomération de Substance à certains minerais bruts, qu'on appelle alors 'matéria'.\n\nCette matéria brute doit être raffinée avant de pouvoir être utilisée par un artisan.", 82, 3, new decimal(0.5)),
      new CraftResource(ResourceType.Ore, "Un phénomène mystérieux provoque l'agglomération de Substance à certains minerais bruts, qu'on appelle alors 'matéria'.\n\nCette matéria brute doit être raffinée avant de pouvoir être utilisée par un artisan.", 82, 4, new decimal(0.5)),
      new CraftResource(ResourceType.Ore, "Un phénomène mystérieux provoque l'agglomération de Substance à certains minerais bruts, qu'on appelle alors 'matéria'.\n\nCette matéria brute doit être raffinée avant de pouvoir être utilisée par un artisan.", 82, 5, new decimal(0.5)),
      new CraftResource(ResourceType.Ore, "Un phénomène mystérieux provoque l'agglomération de Substance à certains minerais bruts, qu'on appelle alors 'matéria'.\n\nCette matéria brute doit être raffinée avant de pouvoir être utilisée par un artisan.", 82, 6, new decimal(0.5)),
      new CraftResource(ResourceType.Ore, "Un phénomène mystérieux provoque l'agglomération de Substance à certains minerais bruts, qu'on appelle alors 'matéria'.\n\nCette matéria brute doit être raffinée avant de pouvoir être utilisée par un artisan.", 82, 7, new decimal(0.5)),
      new CraftResource(ResourceType.Ore, "Un phénomène mystérieux provoque l'agglomération de Substance à certains minerais bruts, qu'on appelle alors 'matéria'.\n\nCette matéria brute doit être raffinée avant de pouvoir être utilisée par un artisan.", 82, 8, new decimal(0.5)),
      new CraftResource(ResourceType.Ingot, "Une fois raffinée, la matéria brute peut-être exploitée par un artisan.\n\nCes lingots de métal gorgés de Substance disposeront de certaines propriétés magiques et pourront même peut-être être ré-enchantés.", 106, 1, new decimal(0.2)),
      new CraftResource(ResourceType.Ingot, "Une fois raffinée, la matéria brute peut-être exploitée par un artisan.\n\nCes lingots de métal gorgés de Substance disposeront de certaines propriétés magiques et pourront même peut-être être ré-enchantés.", 106, 2, new decimal(0.2)),
      new CraftResource(ResourceType.Ingot, "Une fois raffinée, la matéria brute peut-être exploitée par un artisan.\n\nCes lingots de métal gorgés de Substance disposeront de certaines propriétés magiques et pourront même peut-être être ré-enchantés.", 106, 3, new decimal(0.2)),
      new CraftResource(ResourceType.Ingot, "Une fois raffinée, la matéria brute peut-être exploitée par un artisan.\n\nCes lingots de métal gorgés de Substance disposeront de certaines propriétés magiques et pourront même peut-être être ré-enchantés.", 106, 4, new decimal(0.2)),
      new CraftResource(ResourceType.Ingot, "Une fois raffinée, la matéria brute peut-être exploitée par un artisan.\n\nCes lingots de métal gorgés de Substance disposeront de certaines propriétés magiques et pourront même peut-être être ré-enchantés.", 106, 5, new decimal(0.2)),
      new CraftResource(ResourceType.Ingot, "Une fois raffinée, la matéria brute peut-être exploitée par un artisan.\n\nCes lingots de métal gorgés de Substance disposeront de certaines propriétés magiques et pourront même peut-être être ré-enchantés.", 106, 6, new decimal(0.2)),
      new CraftResource(ResourceType.Ingot, "Une fois raffinée, la matéria brute peut-être exploitée par un artisan.\n\nCes lingots de métal gorgés de Substance disposeront de certaines propriétés magiques et pourront même peut-être être ré-enchantés.", 106, 7, new decimal(0.2)),
      new CraftResource(ResourceType.Ingot, "Une fois raffinée, la matéria brute peut-être exploitée par un artisan.\n\nCes lingots de métal gorgés de Substance disposeront de certaines propriétés magiques et pourront même peut-être être ré-enchantés.", 106, 8, new decimal(0.2)),
      new CraftResource(ResourceType.Wood, "Un phénomène mystérieux provoque l'agglomération de Substance à certains arbres. Le bois que l'on en retire est alors appelé 'matéria'.\n\nCette matéria brute doit être raffinée avant de pouvoir être utilisée par un artisan.", 114, 1, new decimal(0.4)),
      new CraftResource(ResourceType.Wood, "Un phénomène mystérieux provoque l'agglomération de Substance à certains arbres. Le bois que l'on en retire est alors appelé 'matéria'.\n\nCette matéria brute doit être raffinée avant de pouvoir être utilisée par un artisan.", 114, 2, new decimal(0.4)),
      new CraftResource(ResourceType.Wood, "Un phénomène mystérieux provoque l'agglomération de Substance à certains arbres. Le bois que l'on en retire est alors appelé 'matéria'.\n\nCette matéria brute doit être raffinée avant de pouvoir être utilisée par un artisan.", 114, 3, new decimal(0.4)),
      new CraftResource(ResourceType.Wood, "Un phénomène mystérieux provoque l'agglomération de Substance à certains arbres. Le bois que l'on en retire est alors appelé 'matéria'.\n\nCette matéria brute doit être raffinée avant de pouvoir être utilisée par un artisan.", 114, 4, new decimal(0.4)),
      new CraftResource(ResourceType.Wood, "Un phénomène mystérieux provoque l'agglomération de Substance à certains arbres. Le bois que l'on en retire est alors appelé 'matéria'.\n\nCette matéria brute doit être raffinée avant de pouvoir être utilisée par un artisan.", 114, 5, new decimal(0.4)),
      new CraftResource(ResourceType.Wood, "Un phénomène mystérieux provoque l'agglomération de Substance à certains arbres. Le bois que l'on en retire est alors appelé 'matéria'.\n\nCette matéria brute doit être raffinée avant de pouvoir être utilisée par un artisan.", 114, 6, new decimal(0.4)),
      new CraftResource(ResourceType.Wood, "Un phénomène mystérieux provoque l'agglomération de Substance à certains arbres. Le bois que l'on en retire est alors appelé 'matéria'.\n\nCette matéria brute doit être raffinée avant de pouvoir être utilisée par un artisan.", 114, 7, new decimal(0.4)),
      new CraftResource(ResourceType.Wood, "Un phénomène mystérieux provoque l'agglomération de Substance à certains arbres. Le bois que l'on en retire est alors appelé 'matéria'.\n\nCette matéria brute doit être raffinée avant de pouvoir être utilisée par un artisan.", 114, 8, new decimal(0.4)),
      new CraftResource(ResourceType.Plank, "Une fois raffinée, la matéria brute peut-être exploitée par un artisan.\n\nCes planches de bois gorgés de Substance disposeront de certaines propriétés magiques et pourront même peut-être être ré-enchantés.", 117, 1, new decimal(0.3)),
      new CraftResource(ResourceType.Plank, "Une fois raffinée, la matéria brute peut-être exploitée par un artisan.\n\nCes planches de bois gorgés de Substance disposeront de certaines propriétés magiques et pourront même peut-être être ré-enchantés.", 117, 2, new decimal(0.3)),
      new CraftResource(ResourceType.Plank, "Une fois raffinée, la matéria brute peut-être exploitée par un artisan.\n\nCes planches de bois gorgés de Substance disposeront de certaines propriétés magiques et pourront même peut-être être ré-enchantés.", 117, 3, new decimal(0.3)),
      new CraftResource(ResourceType.Plank, "Une fois raffinée, la matéria brute peut-être exploitée par un artisan.\n\nCes planches de bois gorgés de Substance disposeront de certaines propriétés magiques et pourront même peut-être être ré-enchantés.", 117, 4, new decimal(0.3)),
      new CraftResource(ResourceType.Plank, "Une fois raffinée, la matéria brute peut-être exploitée par un artisan.\n\nCes planches de bois gorgés de Substance disposeront de certaines propriétés magiques et pourront même peut-être être ré-enchantés.", 117, 5, new decimal(0.3)),
      new CraftResource(ResourceType.Plank, "Une fois raffinée, la matéria brute peut-être exploitée par un artisan.\n\nCes planches de bois gorgés de Substance disposeront de certaines propriétés magiques et pourront même peut-être être ré-enchantés.", 117, 6, new decimal(0.3)),
      new CraftResource(ResourceType.Plank, "Une fois raffinée, la matéria brute peut-être exploitée par un artisan.\n\nCes planches de bois gorgés de Substance disposeront de certaines propriétés magiques et pourront même peut-être être ré-enchantés.", 117, 7, new decimal(0.3)),
      new CraftResource(ResourceType.Plank, "Une fois raffinée, la matéria brute peut-être exploitée par un artisan.\n\nCes planches de bois gorgés de Substance disposeront de certaines propriétés magiques et pourront même peut-être être ré-enchantés.", 117, 8, new decimal(0.3)),
      new CraftResource(ResourceType.Pelt, "Un phénomène mystérieux provoque l'agglomération de Substance chez certaines créatures aquatiques. Les peaux que l'on en retire sont alors appelées 'matéria'.\n\nCette matéria brute doit être raffinée avant de pouvoir être utilisée par un artisan.", 6, 1, new decimal(0.3)),
      new CraftResource(ResourceType.Pelt, "Un phénomène mystérieux provoque l'agglomération de Substance chez certaines créatures aquatiques. Les peaux que l'on en retire sont alors appelées 'matéria'.\n\nCette matéria brute doit être raffinée avant de pouvoir être utilisée par un artisan.", 6, 2, new decimal(0.3)),
      new CraftResource(ResourceType.Pelt, "Un phénomène mystérieux provoque l'agglomération de Substance chez certaines créatures aquatiques. Les peaux que l'on en retire sont alors appelées 'matéria'.\n\nCette matéria brute doit être raffinée avant de pouvoir être utilisée par un artisan.", 6, 3, new decimal(0.3)),
      new CraftResource(ResourceType.Pelt, "Un phénomène mystérieux provoque l'agglomération de Substance chez certaines créatures aquatiques. Les peaux que l'on en retire sont alors appelées 'matéria'.\n\nCette matéria brute doit être raffinée avant de pouvoir être utilisée par un artisan.", 6, 4, new decimal(0.3)),
      new CraftResource(ResourceType.Pelt, "Un phénomène mystérieux provoque l'agglomération de Substance chez certaines créatures aquatiques. Les peaux que l'on en retire sont alors appelées 'matéria'.\n\nCette matéria brute doit être raffinée avant de pouvoir être utilisée par un artisan.", 6, 5, new decimal(0.3)),
      new CraftResource(ResourceType.Pelt, "Un phénomène mystérieux provoque l'agglomération de Substance chez certaines créatures aquatiques. Les peaux que l'on en retire sont alors appelées 'matéria'.\n\nCette matéria brute doit être raffinée avant de pouvoir être utilisée par un artisan.", 6, 6, new decimal(0.3)),
      new CraftResource(ResourceType.Pelt, "Un phénomène mystérieux provoque l'agglomération de Substance chez certaines créatures aquatiques. Les peaux que l'on en retire sont alors appelées 'matéria'.\n\nCette matéria brute doit être raffinée avant de pouvoir être utilisée par un artisan.", 6, 7, new decimal(0.3)),
      new CraftResource(ResourceType.Pelt, "Un phénomène mystérieux provoque l'agglomération de Substance chez certaines créatures aquatiques. Les peaux que l'on en retire sont alors appelées 'matéria'.\n\nCette matéria brute doit être raffinée avant de pouvoir être utilisée par un artisan.", 6, 8, new decimal(0.3)),
      new CraftResource(ResourceType.Leather, "Une fois raffinée, la matéria brute peut-être exploitée par un artisan.\n\nCes cuirs tannés et gorgés de Substance disposeront de certaines propriétés magiques et pourront même peut-être être ré-enchantés.", 87, 1, new decimal(0.2)),
      new CraftResource(ResourceType.Leather, "Une fois raffinée, la matéria brute peut-être exploitée par un artisan.\n\nCes cuirs tannés et gorgés de Substance disposeront de certaines propriétés magiques et pourront même peut-être être ré-enchantés.", 87, 2, new decimal(0.2)),
      new CraftResource(ResourceType.Leather, "Une fois raffinée, la matéria brute peut-être exploitée par un artisan.\n\nCes cuirs tannés et gorgés de Substance disposeront de certaines propriétés magiques et pourront même peut-être être ré-enchantés.", 87, 3, new decimal(0.2)),
      new CraftResource(ResourceType.Leather, "Une fois raffinée, la matéria brute peut-être exploitée par un artisan.\n\nCes cuirs tannés et gorgés de Substance disposeront de certaines propriétés magiques et pourront même peut-être être ré-enchantés.", 87, 4, new decimal(0.2)),
      new CraftResource(ResourceType.Leather, "Une fois raffinée, la matéria brute peut-être exploitée par un artisan.\n\nCes cuirs tannés et gorgés de Substance disposeront de certaines propriétés magiques et pourront même peut-être être ré-enchantés.", 87, 5, new decimal(0.2)),
      new CraftResource(ResourceType.Leather, "Une fois raffinée, la matéria brute peut-être exploitée par un artisan.\n\nCes cuirs tannés et gorgés de Substance disposeront de certaines propriétés magiques et pourront même peut-être être ré-enchantés.", 87, 6, new decimal(0.2)),
      new CraftResource(ResourceType.Leather, "Une fois raffinée, la matéria brute peut-être exploitée par un artisan.\n\nCes cuirs tannés et gorgés de Substance disposeront de certaines propriétés magiques et pourront même peut-être être ré-enchantés.", 87, 7, new decimal(0.2)),
      new CraftResource(ResourceType.Leather, "Une fois raffinée, la matéria brute peut-être exploitée par un artisan.\n\nCes cuirs tannés et gorgés de Substance disposeront de certaines propriétés magiques et pourront même peut-être être ré-enchantés.", 87, 8, new decimal(0.2)),
    }; 

    public static readonly int[] lowArmorBlueprints = new int[] { 0, 1, 2, 3 };
    public static readonly BaseItemType[] lowWeaponBlueprints = new BaseItemType[] { BaseItemType.Whip, BaseItemType.LightMace, BaseItemType.Helmet, BaseItemType.Dagger, BaseItemType.Morningstar, BaseItemType.ShortSpear, BaseItemType.Sickle, BaseItemType.Arrow, BaseItemType.Belt, BaseItemType.Amulet, BaseItemType.Bolt, BaseItemType.Boots, BaseItemType.Bracer, BaseItemType.Bullet, BaseItemType.Cloak, BaseItemType.Club, BaseItemType.Dart, BaseItemType.Gloves, BaseItemType.HeavyCrossbow, BaseItemType.LightCrossbow, BaseItemType.Quarterstaff, BaseItemType.Ring, BaseItemType.Shuriken, BaseItemType.Sling, BaseItemType.SmallShield, BaseItemType.Torch };
    public static readonly int[] mediumArmorBlueprints = new int[] { 4, 5 };
    public static readonly BaseItemType[] mediumWeaponBlueprints = new BaseItemType[] { BaseItemType.Battleaxe, BaseItemType.Greatsword, BaseItemType.Greataxe, BaseItemType.Halberd, BaseItemType.Handaxe, BaseItemType.HeavyFlail, BaseItemType.LargeShield, BaseItemType.LightFlail, BaseItemType.LightHammer, BaseItemType.LightMace, BaseItemType.Longbow, BaseItemType.Longsword, BaseItemType.Rapier, BaseItemType.Scimitar, BaseItemType.Shortbow, BaseItemType.Shortsword, BaseItemType.Shuriken, BaseItemType.ThrowingAxe, BaseItemType.Trident, BaseItemType.Warhammer };
    public static readonly int[] highArmorBlueprints = new int[] { 6, 7, 8 };
    public static readonly BaseItemType[] highWeapônBlueprints = new BaseItemType[] { BaseItemType.TwoBladedSword, BaseItemType.TowerShield, BaseItemType.Scythe, BaseItemType.Kukri, BaseItemType.Katana, BaseItemType.Kama, BaseItemType.DwarvenWaraxe, BaseItemType.DireMace, BaseItemType.Doubleaxe, BaseItemType.Bastardsword };

    public static void AddCraftedItemProperties(NwItem craftedItem, int grade)
    {
      craftedItem.GetObjectVariable<LocalVariableInt>("_ITEM_GRADE").Value = grade;

      foreach (ItemProperty ip in GetCraftItemProperties(craftedItem, grade))
      {
        ItemProperty existingIP = craftedItem.ItemProperties.FirstOrDefault(i => i.DurationType == EffectDuration.Permanent && i.Property.RowIndex == ip.Property.RowIndex && i.SubType?.RowIndex == ip.SubType?.RowIndex && i.Param1TableValue?.RowIndex == ip.Param1TableValue?.RowIndex);

        if (existingIP != null)
        {
          craftedItem.RemoveItemProperty(existingIP);

          if (ip.Property.PropertyType == ItemPropertyType.DamageBonus
            || ip.Property.PropertyType == ItemPropertyType.DamageBonusVsAlignmentGroup
            || ip.Property.PropertyType == ItemPropertyType.DamageBonusVsRacialGroup
            || ip.Property.PropertyType == ItemPropertyType.DamageBonusVsSpecificAlignment)
          {
            int newRank = ItemPropertyDamageCost2da.GetRankFromCostValue(ip.IntParams[3]);
            int existingRank = ItemPropertyDamageCost2da.GetRankFromCostValue(existingIP.IntParams[3]);

            if (existingRank > newRank)
              newRank = existingRank + 1;
            else
              newRank += 1;

            ip.IntParams[3] = ItemPropertyDamageCost2da.GetDamageCostValueFromRank(newRank);
          }
          else if (ip.Property.PropertyType == ItemPropertyType.AcBonus
            || ip.Property.PropertyType == ItemPropertyType.AcBonusVsAlignmentGroup
            || ip.Property.PropertyType == ItemPropertyType.AcBonusVsDamageType
            || ip.Property.PropertyType == ItemPropertyType.AcBonusVsRacialGroup
            || ip.Property.PropertyType == ItemPropertyType.AcBonusVsSpecificAlignment
            || ip.Property.PropertyType == ItemPropertyType.AttackBonus
            || ip.Property.PropertyType == ItemPropertyType.AttackBonusVsAlignmentGroup
            || ip.Property.PropertyType == ItemPropertyType.AttackBonusVsRacialGroup
            || ip.Property.PropertyType == ItemPropertyType.AttackBonusVsSpecificAlignment)
          {
            ip.IntParams[3] += existingIP.IntParams[3];
          }
          else
          {
            if (existingIP.IntParams[3] > ip.IntParams[3])
              ip.IntParams[3] = existingIP.IntParams[3] + 1;
            else
              ip.IntParams[3] += 1;
          }
        }

        craftedItem.AddItemProperty(ip, EffectDuration.Permanent);
      }
    }
  }
}
