using static NWN.Systems.Craft.Collect.Config;
using Anvil.API;

namespace NWN.Systems.Craft.Collect
{
  public static class System
  {
    //public static readonly string[] badPelts = new string[] { "paraceratherium", "ankheg", "gorille", "giantlizard" };
    //public static readonly string[] commonPelts = new string[] { "alligator", "crocodile", "crocblinde", "varan" };
    //public static readonly string[] normalPelts = new string[] { "basilisk", "jhakar", "gorgon", "bulette", "dagon" };

    public static readonly CraftResource[] craftResourceArray = new CraftResource[] 
    { 
      new (ResourceType.InfluxBrut, "L'influx brut est une substance mystérieuse qui s'accumule parfois en certains endroits aquatiques.\n\nIl s'agit aussi d'une substance éminemment toxique, capable de provoquer des mutations irréversibles à même la chair humaine.", 201, new decimal(0.5)),
      new (ResourceType.InfluxRaffine, "Correctement raffiné l'influx permet de rendre n'importe quelle matière malléable et extrêmement facile à travailler. On dit même qu'avec la bonne préparation, il est capable de prolonger la vie et de donner accès à la magie.", 201, new decimal(0.5)),
      //new (ResourceType.Ingot, "Une fois correctement raffiné et incorporé à un matériau de base, l'influx peut-être exploité par un artisan.\n\nCes lingots de métal gorgés d'influx disposeront de certaines propriétés magiques et pourront même être inscrits.", 106, new decimal(0.2)),
      //new (ResourceType.Plank, "Une fois correctement raffinée et incorporée à un matériau de base, l'influx peut-être exploité par un artisan.\n\nCes planches de bois gorgés d'influx disposeront de certaines propriétés magiques et pourront même être inscrits.", 117, new decimal(0.3)),
      //new (ResourceType.Leather, "Une fois correctement raffiné et incorporé à un matériau de base, l'influx peut-être exploité par un artisan.\n\nCes cuirs tannés et gorgés d'influx disposeront de certaines propriétés magiques et pourront même être inscrits.", 87, new decimal(0.2))
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
      SetCraftItemProperties(craftedItem, grade);

      /*foreach (ItemProperty ip in SetCraftItemProperties(craftedItem, grade))
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
      }*/
    }
  }
}
