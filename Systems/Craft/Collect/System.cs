using System;
using System.Collections.Generic;
using NWN.Core;
using NWN.Core.NWNX;
using static NWN.Systems.Craft.Collect.Config;
using static NWN.Systems.Craft.Blueprint;
using NWN.Services;
using NWN.API;
using NLog;
using System.Linq;

namespace NWN.Systems.Craft.Collect
{
  public static class System
  {
    public static readonly Logger Log = LogManager.GetCurrentClassLogger();
    public static string[] badPelts = new string[] { "paraceratherium", "ankheg", "gorille", "giantlizard" };
    public static string[] commonPelts = new string[] { "alligator", "crocodile", "crocblinde", "varan" };
    public static string[] normalPelts = new string[] { "basilisk", "jhakar", "gorgon", "bulette", "dagon" };

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
    public static int[] forgeBasicBlueprints = new int[] { -4, 114, 115, NWScript.BASE_ITEM_LIGHTMACE, NWScript.BASE_ITEM_HELMET, NWScript.BASE_ITEM_DAGGER, NWScript.BASE_ITEM_MORNINGSTAR, NWScript.BASE_ITEM_SHORTSPEAR, NWScript.BASE_ITEM_SICKLE };
    public static int[] woodBasicBlueprints = new int[] { 114, 115, NWScript.BASE_ITEM_SMALLSHIELD, NWScript.BASE_ITEM_CLUB, NWScript.BASE_ITEM_DART, NWScript.BASE_ITEM_BULLET, NWScript.BASE_ITEM_HEAVYCROSSBOW, NWScript.BASE_ITEM_LIGHTCROSSBOW, NWScript.BASE_ITEM_QUARTERSTAFF, NWScript.BASE_ITEM_SLING, NWScript.BASE_ITEM_ARROW, NWScript.BASE_ITEM_BOLT };
    public static int[] leatherBasicBlueprints = new int[] { -1, -2, -3, -9, 114, 115, NWScript.BASE_ITEM_BELT, NWScript.BASE_ITEM_BOOTS, NWScript.BASE_ITEM_BRACER, NWScript.BASE_ITEM_CLOAK, NWScript.BASE_ITEM_GLOVES, NWScript.BASE_ITEM_WHIP };
    public static int[] lowBlueprints = new int[] { -1, -2, -3, -9, 114, 115, NWScript.BASE_ITEM_LIGHTMACE, NWScript.BASE_ITEM_HELMET, NWScript.BASE_ITEM_DAGGER, NWScript.BASE_ITEM_MORNINGSTAR, NWScript.BASE_ITEM_SHORTSPEAR, NWScript.BASE_ITEM_SICKLE, NWScript.BASE_ITEM_ARROW, NWScript.BASE_ITEM_BELT, NWScript.BASE_ITEM_AMULET, NWScript.BASE_ITEM_BOLT, NWScript.BASE_ITEM_BOOTS, NWScript.BASE_ITEM_BRACER, NWScript.BASE_ITEM_BULLET, NWScript.BASE_ITEM_CLOAK, NWScript.BASE_ITEM_CLUB, NWScript.BASE_ITEM_DART, NWScript.BASE_ITEM_GLOVES, NWScript.BASE_ITEM_HEAVYCROSSBOW, NWScript.BASE_ITEM_LIGHTCROSSBOW, NWScript.BASE_ITEM_QUARTERSTAFF, NWScript.BASE_ITEM_RING, NWScript.BASE_ITEM_SHURIKEN, NWScript.BASE_ITEM_SLING, NWScript.BASE_ITEM_SMALLSHIELD, NWScript.BASE_ITEM_TORCH };
    public static int[] mediumBlueprints = new int[] { -4, -5, NWScript.BASE_ITEM_BATTLEAXE, NWScript.BASE_ITEM_GREATAXE, NWScript.BASE_ITEM_GREATSWORD, NWScript.BASE_ITEM_HALBERD, NWScript.BASE_ITEM_HANDAXE, NWScript.BASE_ITEM_HEAVYFLAIL, NWScript.BASE_ITEM_LARGESHIELD, NWScript.BASE_ITEM_LIGHTFLAIL, NWScript.BASE_ITEM_LIGHTHAMMER, NWScript.BASE_ITEM_LONGBOW, NWScript.BASE_ITEM_LONGSWORD, NWScript.BASE_ITEM_RAPIER, NWScript.BASE_ITEM_SCIMITAR, NWScript.BASE_ITEM_SHORTBOW, NWScript.BASE_ITEM_SHORTSWORD, NWScript.BASE_ITEM_SHURIKEN, NWScript.BASE_ITEM_THROWINGAXE, NWScript.BASE_ITEM_TRIDENT, NWScript.BASE_ITEM_WARHAMMER };
    public static int[] highBlueprints = new int[] { -6, -7, -8, NWScript.BASE_ITEM_WHIP, NWScript.BASE_ITEM_TWOBLADEDSWORD, NWScript.BASE_ITEM_TOWERSHIELD, NWScript.BASE_ITEM_SCYTHE, NWScript.BASE_ITEM_KUKRI, NWScript.BASE_ITEM_KATANA, NWScript.BASE_ITEM_KAMA, NWScript.BASE_ITEM_DWARVENWARAXE, NWScript.BASE_ITEM_DOUBLEAXE, NWScript.BASE_ITEM_DIREMACE, NWScript.BASE_ITEM_BASTARDSWORD };

    public static Dictionary<int, Blueprint> blueprintDictionnary = new Dictionary<int, Blueprint>();

    public static void StartCollectCycle(PlayerSystem.Player player, uint oPlaceable, Action completeCallback)
    {
      player.OnCollectCycleCancel = () => {
        NWN.Utils.RemoveTaggedEffect(oPlaceable, $"_{NWScript.GetPCPublicCDKey(player.oid)}_MINING_BEAM");
        RemoveCollectCycleCallbacks(player);
        PlayerPlugin.StopGuiTimingBar(player.oid);
      };
      player.OnCollectCycleComplete = () => {
        completeCallback();
        RemoveCollectCycleCallbacks(player);
      };

      var resourceExtractor = NWScript.GetItemInSlot(NWScript.INVENTORY_SLOT_RIGHTHAND, player.oid);
      float cycleDuration = 180.0f;
      if (NWN.Systems.Config.env == NWN.Systems.Config.Env.Chim)
        cycleDuration = 10.0f;

      if (NWScript.GetIsObjectValid(resourceExtractor) == 1) // TODO : Idée pour plus tard, le strip miner le plus avancé pourra équipper un cristal de spécialisation pour extraire deux fois plus de minerai en un cycle sur son minerai de spécialité
      {
        cycleDuration = cycleDuration - (cycleDuration * NWScript.GetLocalInt(resourceExtractor, "_ITEM_LEVEL") * 2 / 100);
      }

      Core.Effect eRay = NWScript.EffectBeam(NWScript.VFX_BEAM_DISINTEGRATE, resourceExtractor, 1, 0, 3);
      eRay = NWScript.TagEffect(eRay, $"_{NWScript.GetPCPublicCDKey(player.oid)}_MINING_BEAM");
      NWScript.ApplyEffectToObject(NWScript.DURATION_TYPE_TEMPORARY, eRay, oPlaceable, cycleDuration);

      PlayerPlugin.StartGuiTimingBar(player.oid, cycleDuration, "collect_complete");

      EventsPlugin.AddObjectToDispatchList("NWNX_ON_TIMING_BAR_CANCEL_BEFORE", "collect_cancel", player.oid);
      EventsPlugin.AddObjectToDispatchList("NWNX_ON_CLIENT_DISCONNECT_BEFORE", "collect_cancel", player.oid);
      EventsPlugin.AddObjectToDispatchList("NWNX_ON_ITEM_EQUIP_BEFORE", "collect_cancel", player.oid);
      EventsPlugin.AddObjectToDispatchList("NWNX_ON_ITEM_UNEQUIP_BEFORE", "collect_cancel", player.oid);
      EventsPlugin.AddObjectToDispatchList("NWNX_ON_START_COMBAT_ROUND_AFTER", "collect_cancel", player.oid);
      EventsPlugin.AddObjectToDispatchList("NWNX_ON_INPUT_CAST_SPELL_BEFORE", "collect_cancel", player.oid);
    }
    private static void RemoveCollectCycleCallbacks(PlayerSystem.Player player)
    {
      player.OnCollectCycleCancel = () => { };
      player.OnCollectCycleComplete = () => { };
      EventsPlugin.RemoveObjectFromDispatchList("NWNX_ON_TIMING_BAR_CANCEL_BEFORE", "collect_cancel", player.oid);
      EventsPlugin.RemoveObjectFromDispatchList("NWNX_ON_CLIENT_DISCONNECT_BEFORE", "collect_cancel", player.oid);
      EventsPlugin.RemoveObjectFromDispatchList("NWNX_ON_ITEM_EQUIP_BEFORE", "collect_cancel", player.oid);
      EventsPlugin.RemoveObjectFromDispatchList("NWNX_ON_ITEM_UNEQUIP_BEFORE", "collect_cancel", player.oid);
      EventsPlugin.RemoveObjectFromDispatchList("NWNX_ON_START_COMBAT_ROUND_AFTER", "collect_cancel", player.oid);
      EventsPlugin.RemoveObjectFromDispatchList("NWNX_ON_INPUT_CAST_SPELL_BEFORE", "collect_cancel", player.oid);
    }
    public static void AddCraftedItemProperties(NwItem craftedItem, string material)
    {
      string name = material;

      if (leatherDictionnary.Any(l => l.Value.name == material))
        name = leatherDictionnary.First(l => l.Value.name == material).Key.ToDescription();
      else if (plankDictionnary.Any(l => l.Value.name == material))
        name = plankDictionnary.First(l => l.Value.name == material).Key.ToDescription();

      craftedItem.Name = $"{craftedItem.Name} en {name}";
      craftedItem.GetLocalVariable<string>("_ITEM_MATERIAL").Value = material;
      
      foreach (Core.ItemProperty ip in GetCraftItemProperties(material, craftedItem))
      {
        //NWScript.SendMessageToPC(NWScript.GetFirstPC(), $"Adding IP : {ip}");
        NWScript.AddItemProperty(NWScript.DURATION_TYPE_PERMANENT, ip, craftedItem);
      }
    }
    public static void AddCraftedEnchantementProperties(NwItem craftedItem, string spellId)
    {
      craftedItem.AddItemProperty(GetCraftEnchantementProperties(craftedItem, spellId), EffectDuration.Permanent);
      Log.Info("Enchantement properties added");
    }
    public static bool IsItemCraftMaterial(string itemTag)
    {
      if (Enum.TryParse(itemTag, out OreType myOreType) && myOreType != OreType.Invalid)
        return true;
      if (Enum.TryParse(itemTag, out MineralType myMineralType) && myMineralType != MineralType.Invalid)
        return true;
      if (Enum.TryParse(itemTag, out WoodType myWoodType) && myWoodType != WoodType.Invalid)
        return true;
      if (Enum.TryParse(itemTag, out PlankType myPlankType) && myPlankType != PlankType.Invalid)
        return true;
      if (Array.FindIndex(badPelts, x => x == itemTag) > -1 || Array.FindIndex(commonPelts, x => x == itemTag) > -1
        || Array.FindIndex(normalPelts, x => x == itemTag) > -1)
        return true;

      return false;
    }
    public static string GetCraftMaterialItemTemplate(string itemTag)
    {
      if (Enum.TryParse(itemTag, out OreType myOreType) && myOreType != OreType.Invalid)
        return "ore";
      if (Enum.TryParse(itemTag, out MineralType myMineralType) && myMineralType != MineralType.Invalid)
        return "mineral";
      if (Enum.TryParse(itemTag, out WoodType myWoodType) && myWoodType != WoodType.Invalid)
        return "wood";
      if (Enum.TryParse(itemTag, out PlankType myPlankType) && myPlankType != PlankType.Invalid)
        return "plank";
      if (Array.FindIndex(badPelts, x => x == itemTag) > -1 || Array.FindIndex(commonPelts, x => x == itemTag) > -1
        || Array.FindIndex(normalPelts, x => x == itemTag) > -1)
        return "pelt";

      NWN.Utils.LogMessageToDMs($"Could not find item template for tag : {itemTag}");
      return "";
    }
  }
}
