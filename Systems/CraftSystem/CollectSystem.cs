using System;
using System.Collections.Generic;
using NWN.Core;
using NWN.Core.NWNX;
using static NWN.Systems.Blueprint;
using static NWN.Systems.PlayerSystem;
using static NWN.Systems.Items.Utils;

namespace NWN.Systems
{
  public static partial class CollectSystem
  {
    public static Dictionary<string, Func<uint, int>> Register = new Dictionary<string, Func<uint, int>>
    {
            { "event_collect_cycle_cancel_before", HandleBeforeCollectCycleCancel },
            { "on_collect_cycle_complete", HandleAfterCollectCycleComplete },
    };

    public static Dictionary<int, Feat> craftBaseItemFeatDictionnary = new Dictionary<int, Feat>()
    {
      {-13, Feat.Research },
      {-12, Feat.Metallurgy },
      {-11, Feat.BlueprintCopy },
      {-9, Feat.CraftClothing },
      {-8, Feat.CraftFullPlate },
      {-7, Feat.CraftHalfPlate },
      {-6, Feat.CraftSplintMail },
      {-5, Feat.CraftBreastPlate },
      {-4, Feat.CraftScaleMail },
      {-3, Feat.CraftStuddedLeather },
      {-2, Feat.CraftLeatherArmor },
      {-1, Feat.CraftPaddedArmor },
      {0, Feat.CraftShortsword },
      {1, Feat.CraftLongsword },
      {2, Feat.CraftBattleAxe },
      {3, Feat.CraftBastardSword },
      {4, Feat.CraftLightFlail },
      {5, Feat.CraftWarHammer },
      {6, Feat.CraftHeavyCrossbow },
      {7, Feat.CraftLightCrossbow },
      {8, Feat.CraftLongBow },
      {9, Feat.CraftLightMace },
      {10, Feat.CraftHalberd },
      {11, Feat.CraftShortBow },
      {12, Feat.CraftTwoBladedSword },
      {13, Feat.CraftGreatSword },
      {14, Feat.CraftSmallShield },
      {15, Feat.CraftTorch },
      {17, Feat.CraftHelmet },
      {18, Feat.CraftGreatAxe },
      {19, Feat.CraftAmulet },
      {20, Feat.CraftArrow },
      {21, Feat.CraftBelt },
      {22, Feat.CraftDagger },
      {25, Feat.CraftBolt },
      {26, Feat.CraftBoots },
      {27, Feat.CraftBullets },
      {28, Feat.CraftClub },
      {31, Feat.CraftDarts },
      {32, Feat.CraftDireMace },
      {33, Feat.CraftDoubleAxe },
      {35, Feat.CraftHeavyFlail },
      {36, Feat.CraftGloves },
      {37, Feat.CraftLightHammer },
      {38, Feat.CraftHandAxe },
      {40, Feat.CraftKama },
      {41, Feat.CraftKatana },
      {42, Feat.CraftKukri },
      {44, Feat.CraftMagicRod },
      {45, Feat.CraftStaff },
      {46, Feat.CraftMagicWand },
      {47, Feat.CraftMorningStar },
      {49, Feat.CraftPotion },
      {50, Feat.CraftQuarterstaff },
      {51, Feat.CraftRapier },
      {52, Feat.CraftRing },
      {53, Feat.CraftScimitar },
      {55, Feat.CraftScythe },
      {56, Feat.CraftLargeShield },
      {57, Feat.CraftTowerShield },
      {58, Feat.CraftShortSpear },
      {59, Feat.CraftShuriken },
      {60, Feat.CraftSickle },
      {61, Feat.CraftSling },
      {63, Feat.CraftThrowingAxe },
      {75, Feat.CraftSpellScroll },
      {78, Feat.CraftBracer },
      {80, Feat.CraftCloak },
      {92, Feat.CraftLance },
      {95, Feat.CraftTrident },
      {108, Feat.CraftDwarvenWarAxe },
      {111, Feat.CraftWhip },
      {114, Feat.CraftForgeHammer },
      {115, Feat.CraftOreExtractor },
    };
    public static int[] forgeBasicBlueprints = new int[] { -4, 114, 115, NWScript.BASE_ITEM_LIGHTMACE, NWScript.BASE_ITEM_HELMET, NWScript.BASE_ITEM_DAGGER, NWScript.BASE_ITEM_MORNINGSTAR, NWScript.BASE_ITEM_SHORTSPEAR, NWScript.BASE_ITEM_SICKLE, 92};
    public static int[] lowBlueprints = new int[] { -1, -2, -3, -9, 114, 115, NWScript.BASE_ITEM_LIGHTMACE, NWScript.BASE_ITEM_HELMET, NWScript.BASE_ITEM_DAGGER, NWScript.BASE_ITEM_MORNINGSTAR, NWScript.BASE_ITEM_SHORTSPEAR, NWScript.BASE_ITEM_SICKLE, 92, NWScript.BASE_ITEM_ARROW, NWScript.BASE_ITEM_BELT, NWScript.BASE_ITEM_AMULET, NWScript.BASE_ITEM_BOLT, NWScript.BASE_ITEM_BOOTS, NWScript.BASE_ITEM_BRACER, NWScript.BASE_ITEM_BULLET, NWScript.BASE_ITEM_CLOAK, NWScript.BASE_ITEM_CLUB, NWScript.BASE_ITEM_DART, NWScript.BASE_ITEM_GLOVES, NWScript.BASE_ITEM_HEAVYCROSSBOW, NWScript.BASE_ITEM_LIGHTCROSSBOW, NWScript.BASE_ITEM_QUARTERSTAFF, NWScript.BASE_ITEM_RING, NWScript.BASE_ITEM_SHURIKEN, NWScript.BASE_ITEM_SLING, NWScript.BASE_ITEM_SMALLSHIELD, NWScript.BASE_ITEM_TORCH };
    public static int[] mediumBlueprints = new int[] { -4, -5, NWScript.BASE_ITEM_BATTLEAXE, NWScript.BASE_ITEM_GREATAXE, NWScript.BASE_ITEM_GREATSWORD, NWScript.BASE_ITEM_HALBERD, NWScript.BASE_ITEM_HANDAXE, NWScript.BASE_ITEM_HEAVYFLAIL, NWScript.BASE_ITEM_LARGESHIELD, NWScript.BASE_ITEM_LIGHTFLAIL, NWScript.BASE_ITEM_LIGHTHAMMER, NWScript.BASE_ITEM_LONGBOW, NWScript.BASE_ITEM_LONGSWORD, NWScript.BASE_ITEM_RAPIER, NWScript.BASE_ITEM_SCIMITAR, NWScript.BASE_ITEM_SHORTBOW, NWScript.BASE_ITEM_SHORTSWORD, NWScript.BASE_ITEM_SHURIKEN, NWScript.BASE_ITEM_THROWINGAXE, NWScript.BASE_ITEM_TRIDENT, NWScript.BASE_ITEM_WARHAMMER };
    public static int[] highBlueprints = new int[] { -6, -7, -8, NWScript.BASE_ITEM_WHIP, NWScript.BASE_ITEM_TWOBLADEDSWORD, NWScript.BASE_ITEM_TOWERSHIELD, NWScript.BASE_ITEM_SCYTHE, NWScript.BASE_ITEM_KUKRI, NWScript.BASE_ITEM_KATANA, NWScript.BASE_ITEM_KAMA, NWScript.BASE_ITEM_DWARVENWARAXE, NWScript.BASE_ITEM_DOUBLEAXE, NWScript.BASE_ITEM_DIREMACE, NWScript.BASE_ITEM_BASTARDSWORD };

    public static Dictionary<int, Blueprint> blueprintDictionnary = new Dictionary<int, Blueprint>();
    private static int HandleBeforeCollectCycleCancel(uint oidSelf)
    {
      Player player;
      if (Players.TryGetValue(oidSelf, out player))
      {
        player.CancelCollectCycle();
      }

      return 0;
    }
    private static int HandleAfterCollectCycleComplete(uint oidSelf)
    {
      Player player;
      if (Players.TryGetValue(oidSelf, out player))
      {
        player.CompleteCollectCycle();
      }

      return 0;
    }    
    public static void StartCollectCycle(Player player, uint rock, Action cancelCallback, Action completeCallback)
    {
      player.OnCollectCycleCancel = cancelCallback;
      player.OnCollectCycleComplete = completeCallback;

      var resourceExtractor = NWScript.GetItemInSlot(NWScript.INVENTORY_SLOT_RIGHTHAND, player.oid);
      float cycleDuration = 180.0f;

      if (NWScript.GetIsObjectValid(resourceExtractor) == 1) // TODO : Idée pour plus tard, le strip miner le plus avancé pourra équipper un cristal de spécialisation pour extraire deux fois plus de minerai en un cycle sur son minerai de spécialité
      {
        cycleDuration = cycleDuration - (cycleDuration * NWScript.GetLocalInt(resourceExtractor, "_ITEM_LEVEL") * 2 / 100);
      }

      Effect eRay = NWScript.EffectBeam(NWScript.VFX_BEAM_DISINTEGRATE, resourceExtractor, 1);
      eRay = NWScript.TagEffect(eRay, $"_{NWScript.GetPCPublicCDKey(player.oid)}_MINING_BEAM");
      NWScript.ApplyEffectToObject(NWScript.DURATION_TYPE_TEMPORARY, eRay, rock, cycleDuration);
      
      PlayerPlugin.StartGuiTimingBar(player.oid, cycleDuration, "on_collect_cycle_complete");

      EventsPlugin.AddObjectToDispatchList("NWNX_ON_TIMING_BAR_CANCEL_BEFORE", "event_collect_cycle_cancel_before", player.oid);
      EventsPlugin.AddObjectToDispatchList("NWNX_ON_CLIENT_DISCONNECT_BEFORE", "event_collect_cycle_cancel_before", player.oid);
      EventsPlugin.AddObjectToDispatchList("NWNX_ON_ITEM_EQUIP_BEFORE", "event_collect_cycle_cancel_before", player.oid);
      EventsPlugin.AddObjectToDispatchList("NWNX_ON_ITEM_UNEQUIP_BEFORE", "event_collect_cycle_cancel_before", player.oid);
      EventsPlugin.AddObjectToDispatchList("NWNX_ON_START_COMBAT_ROUND_AFTER", "event_collect_cycle_cancel_before", player.oid);
      EventsPlugin.AddObjectToDispatchList("NWNX_ON_INPUT_CAST_SPELL_BEFORE", "event_collect_cycle_cancel_before", player.oid);
    }
    public static void RemoveCollectCycleCallbacks(Player player)
    {
      player.OnCollectCycleCancel = () => { };
      player.OnCollectCycleComplete = () => { };
      EventsPlugin.RemoveObjectFromDispatchList("NWNX_ON_TIMING_BAR_CANCEL_BEFORE", "event_collect_cycle_cancel_before", player.oid);
      EventsPlugin.RemoveObjectFromDispatchList("NWNX_ON_CLIENT_DISCONNECT_BEFORE", "event_collect_cycle_cancel_before", player.oid);
      EventsPlugin.RemoveObjectFromDispatchList("NWNX_ON_ITEM_EQUIP_BEFORE", "event_collect_cycle_cancel_before", player.oid);
      EventsPlugin.RemoveObjectFromDispatchList("NWNX_ON_ITEM_UNEQUIP_BEFORE", "event_collect_cycle_cancel_before", player.oid);
      EventsPlugin.RemoveObjectFromDispatchList("NWNX_ON_START_COMBAT_ROUND_AFTER", "event_collect_cycle_cancel_before", player.oid);
      EventsPlugin.RemoveObjectFromDispatchList("NWNX_ON_INPUT_CAST_SPELL_BEFORE", "event_collect_cycle_cancel_before", player.oid);
    }
    public static void AddCraftedItemProperties(uint craftedItem, Blueprint blueprint, string material)
    {
      NWScript.SetName(craftedItem, NWScript.GetName(craftedItem) + " en " + material);
      NWScript.SetLocalString(craftedItem, "_ITEM_MATERIAL", material);

      foreach (ItemProperty ip in GetCraftItemProperties(GetMineralTypeFromName(material), GetItemCategory(NWScript.GetBaseItemType(craftedItem))))
      {
        NWScript.AddItemProperty(NWScript.DURATION_TYPE_PERMANENT, ip, craftedItem);
      }

      if(GetItemCategory(NWScript.GetBaseItemType(craftedItem)) == ItemCategory.CraftTool)
        switch(material)
        {
          case "Tritanium":
            NWScript.SetLocalInt(craftedItem, "_DURABILITY", 10);
            break;
          case "Pyerite":
            NWScript.SetLocalInt(craftedItem, "_DURABILITY", 25);
            NWScript.SetLocalInt(craftedItem, "_ITEM_LEVEL", 1);
            break;
        }
    }
    public static bool IsItemCraftMaterial(string itemTag)
    {
      if (GetOreTypeFromName(itemTag) != OreType.Invalid || GetMineralTypeFromName(itemTag) != MineralType.Invalid)
        return true;

      return false;
    }
    public static string GetCraftMaterialItemTemplate(string itemTag)
    {
      if (GetOreTypeFromName(itemTag) != OreType.Invalid)
        return "ore";
      else if (GetMineralTypeFromName(itemTag) != MineralType.Invalid)
        return "mineral";

      Utils.LogMessageToDMs($"Could not find item template for tag : {itemTag}");
      return "";
    }
  }
}
