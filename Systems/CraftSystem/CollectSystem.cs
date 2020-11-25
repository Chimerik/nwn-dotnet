using System;
using System.Collections.Generic;
using NWN.Core;
using NWN.Core.NWNX;
using static NWN.Systems.Blueprint;
using static NWN.Systems.PlayerSystem;

namespace NWN.Systems
{
  public static partial class CollectSystem
  {
    public static Dictionary<string, Func<uint, int>> Register = new Dictionary<string, Func<uint, int>>
    {
            { "event_mining_cycle_cancel_before", HandleBeforeMiningCycleCancel },
            { "on_mining_cycle_complete", HandleAfterMiningCycleComplete },
    };

    public static Dictionary<int, Feat> craftBaseItemFeatDictionnary = new Dictionary<int, Feat>()
    {
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
    };
    public static int[] forgeBasicBlueprints = new int[] { -4, -5, -6, NWScript.BASE_ITEM_SHORTSWORD, NWScript.BASE_ITEM_LONGSWORD, NWScript.BASE_ITEM_BATTLEAXE, NWScript.BASE_ITEM_LIGHTFLAIL, NWScript.BASE_ITEM_WARHAMMER, NWScript.BASE_ITEM_LIGHTMACE, NWScript.BASE_ITEM_HELMET, NWScript.BASE_ITEM_DAGGER, NWScript.BASE_ITEM_DIREMACE, NWScript.BASE_ITEM_HEAVYFLAIL, NWScript.BASE_ITEM_LIGHTHAMMER, NWScript.BASE_ITEM_HANDAXE, NWScript.BASE_ITEM_MORNINGSTAR, NWScript.BASE_ITEM_RAPIER, NWScript.BASE_ITEM_SCIMITAR, NWScript.BASE_ITEM_SHORTSPEAR, NWScript.BASE_ITEM_SICKLE, NWScript.BASE_ITEM_THROWINGAXE, 92, NWScript.BASE_ITEM_TRIDENT };

    public static Dictionary<int, Blueprint> blueprintDictionnary = new Dictionary<int, Blueprint>();
    private static int HandleBeforeMiningCycleCancel(uint oidSelf)
    {
      Player player;
      if (Players.TryGetValue(oidSelf, out player))
      {
        player.DoActionOnMiningCycleCancelled();
      }

      return 0;
    }
    private static int HandleAfterMiningCycleComplete(uint oidSelf)
    {
      NWScript.SendMessageToPC(oidSelf, "Mining cycle completed !");

      Player player;
      if (Players.TryGetValue(oidSelf, out player))
      {
        player.DoActionOnMiningCycleCompleted();
      }

      return 0;
    }    
    public static void StartMiningCycle(Player player, uint rock, Action cancelCallback, Action completeCallback)
    {
      player.OnMiningCycleCancelled = cancelCallback;
      player.OnMiningCycleCompleted = completeCallback;

      var miningStriper = NWScript.GetItemInSlot(NWScript.INVENTORY_SLOT_RIGHTHAND, player.oid);
      float cycleDuration = 180.0f;

      if (NWScript.GetIsObjectValid(miningStriper) == 1) // TODO : Idée pour plus tard, le strip miner le plus avancé pourra équipper un cristal de spécialisation pour extraire deux fois plus de minerai en un cycle sur son minerai de spécialité
      {
        cycleDuration = cycleDuration - (cycleDuration * NWScript.GetLocalInt(miningStriper, "_ITEM_LEVEL") * 2 / 100);
      }

      Effect eRay = NWScript.EffectBeam(NWScript.VFX_BEAM_DISINTEGRATE, miningStriper, 1);
      eRay = NWScript.TagEffect(eRay, $"_{NWScript.GetPCPublicCDKey(player.oid)}_MINING_BEAM");
      NWScript.ApplyEffectToObject(NWScript.DURATION_TYPE_TEMPORARY, eRay, rock, cycleDuration);
      
      PlayerPlugin.StartGuiTimingBar(player.oid, cycleDuration, "on_mining_cycle_complete");

      EventsPlugin.AddObjectToDispatchList("NWNX_ON_TIMING_BAR_CANCEL_BEFORE", "event_mining_cycle_cancel_before", player.oid);
      EventsPlugin.AddObjectToDispatchList("NWNX_ON_CLIENT_DISCONNECT_BEFORE", "event_mining_cycle_cancel_before", player.oid);
      EventsPlugin.AddObjectToDispatchList("NWNX_ON_ITEM_EQUIP_BEFORE", "event_mining_cycle_cancel_before", player.oid);
      EventsPlugin.AddObjectToDispatchList("NWNX_ON_ITEM_UNEQUIP_BEFORE", "event_mining_cycle_cancel_before", player.oid);
      EventsPlugin.AddObjectToDispatchList("NWNX_ON_START_COMBAT_ROUND_AFTER", "event_mining_cycle_cancel_before", player.oid);
      EventsPlugin.AddObjectToDispatchList("NWNX_ON_INPUT_CAST_SPELL_BEFORE", "event_mining_cycle_cancel_before", player.oid);
    }
    public static void RemoveMiningCycleCallbacks(Player player)
    {
      player.OnMiningCycleCancelled = () => { };
      player.OnMiningCycleCompleted = () => { };
      EventsPlugin.RemoveObjectFromDispatchList("NWNX_ON_TIMING_BAR_CANCEL_BEFORE", "event_mining_cycle_cancel_before", player.oid);
      EventsPlugin.RemoveObjectFromDispatchList("NWNX_ON_CLIENT_DISCONNECT_BEFORE", "event_mining_cycle_cancel_before", player.oid);
      EventsPlugin.RemoveObjectFromDispatchList("NWNX_ON_ITEM_EQUIP_BEFORE", "event_mining_cycle_cancel_before", player.oid);
      EventsPlugin.RemoveObjectFromDispatchList("NWNX_ON_ITEM_UNEQUIP_BEFORE", "event_mining_cycle_cancel_before", player.oid);
      EventsPlugin.RemoveObjectFromDispatchList("NWNX_ON_START_COMBAT_ROUND_AFTER", "event_mining_cycle_cancel_before", player.oid);
      EventsPlugin.RemoveObjectFromDispatchList("NWNX_ON_INPUT_CAST_SPELL_BEFORE", "event_mining_cycle_cancel_before", player.oid);
    }
    public static void AddCraftedItemProperties(uint craftedItem, Blueprint blueprint, string material)
    {
      NWScript.SetName(craftedItem, NWScript.GetName(craftedItem) + " en " + material);
      NWScript.SetLocalString(craftedItem, "_ITEM_MATERIAL", material);

      foreach (ItemProperty ip in GetCraftItemProperties(GetMineralTypeFromName(material), ItemSystem.GetItemCategory(craftedItem)))
      {
        NWScript.AddItemProperty(NWScript.DURATION_TYPE_PERMANENT, ip, craftedItem);
      }
    }
    public static Boolean IsItemCraftMaterial(string itemTag)
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
