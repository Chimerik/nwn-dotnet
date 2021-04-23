using System;
using System.Collections.Generic;
using NWN.Core;
using NWN.Core.NWNX;
using static NWN.Systems.Craft.Collect.Config;
using static NWN.Systems.Craft.Blueprint;
using NWN.API;
using NLog;
using System.Linq;
using NWN.API.Constants;
using Action = System.Action;
using System.Threading.Tasks;
using NWN.API.Events;
using System.Threading;

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
      {-13, CustomFeats.Research },
      {-12, CustomFeats.Metallurgy },
      {-11, CustomFeats.BlueprintCopy },
      {-9, CustomFeats.CraftClothing },
      {-8, CustomFeats.CraftFullPlate },
      {-7, CustomFeats.CraftHalfPlate },
      {-6, CustomFeats.CraftSplintMail },
      {-5, CustomFeats.CraftBreastPlate },
      {-4, CustomFeats.CraftScaleMail },
      {-3, CustomFeats.CraftStuddedLeather },
      {-2, CustomFeats.CraftLeatherArmor },
      {-1, CustomFeats.CraftPaddedArmor },
      {0, CustomFeats.CraftShortsword },
      {1, CustomFeats.CraftLongsword },
      {2, CustomFeats.CraftBattleAxe },
      {3, CustomFeats.CraftBastardSword },
      {4, CustomFeats.CraftLightFlail },
      {5, CustomFeats.CraftWarHammer },
      {6, CustomFeats.CraftHeavyCrossbow },
      {7, CustomFeats.CraftLightCrossbow },
      {8, CustomFeats.CraftLongBow },
      {9, CustomFeats.CraftLightMace },
      {10, CustomFeats.CraftHalberd },
      {11, CustomFeats.CraftShortBow },
      {12, CustomFeats.CraftTwoBladedSword },
      {13, CustomFeats.CraftGreatSword },
      {14, CustomFeats.CraftSmallShield },
      {15, CustomFeats.CraftTorch },
      {17, CustomFeats.CraftHelmet },
      {18, CustomFeats.CraftGreatAxe },
      {19, CustomFeats.CraftAmulet },
      {20, CustomFeats.CraftArrow },
      {21, CustomFeats.CraftBelt },
      {22, CustomFeats.CraftDagger },
      {25, CustomFeats.CraftBolt },
      {26, CustomFeats.CraftBoots },
      {27, CustomFeats.CraftBullets },
      {28, CustomFeats.CraftClub },
      {31, CustomFeats.CraftDarts },
      {32, CustomFeats.CraftDireMace },
      {33, CustomFeats.CraftDoubleAxe },
      {35, CustomFeats.CraftHeavyFlail },
      {36, CustomFeats.CraftGloves },
      {37, CustomFeats.CraftLightHammer },
      {38, CustomFeats.CraftHandAxe },
      {40, CustomFeats.CraftKama },
      {41, CustomFeats.CraftKatana },
      {42, CustomFeats.CraftKukri },
      {44, CustomFeats.CraftMagicRod },
      {45, CustomFeats.CraftStaff },
      {46, CustomFeats.CraftMagicWand },
      {47, CustomFeats.CraftMorningStar },
      {49, CustomFeats.CraftPotion },
      {50, CustomFeats.CraftQuarterstaff },
      {51, CustomFeats.CraftRapier },
      {52, CustomFeats.CraftRing },
      {53, CustomFeats.CraftScimitar },
      {55, CustomFeats.CraftScythe },
      {56, CustomFeats.CraftLargeShield },
      {57, CustomFeats.CraftTowerShield },
      {58, CustomFeats.CraftShortSpear },
      {59, CustomFeats.CraftShuriken },
      {60, CustomFeats.CraftSickle },
      {61, CustomFeats.CraftSling },
      {63, CustomFeats.CraftThrowingAxe },
      {75, CustomFeats.CraftSpellScroll },
      {78, CustomFeats.CraftBracer },
      {80, CustomFeats.CraftCloak },
      {95, CustomFeats.CraftTrident },
      {108, CustomFeats.CraftDwarvenWarAxe },
      {111, CustomFeats.CraftWhip },
      {114, CustomFeats.CraftForgeHammer },
      {115, CustomFeats.CraftOreExtractor },
    };
    public static int[] forgeBasicBlueprints = new int[] { -4, 114, 115, NWScript.BASE_ITEM_LIGHTMACE, NWScript.BASE_ITEM_HELMET, NWScript.BASE_ITEM_DAGGER, NWScript.BASE_ITEM_MORNINGSTAR, NWScript.BASE_ITEM_SHORTSPEAR, NWScript.BASE_ITEM_SICKLE };
    public static int[] woodBasicBlueprints = new int[] { 114, 115, NWScript.BASE_ITEM_SMALLSHIELD, NWScript.BASE_ITEM_CLUB, NWScript.BASE_ITEM_DART, NWScript.BASE_ITEM_BULLET, NWScript.BASE_ITEM_HEAVYCROSSBOW, NWScript.BASE_ITEM_LIGHTCROSSBOW, NWScript.BASE_ITEM_QUARTERSTAFF, NWScript.BASE_ITEM_SLING, NWScript.BASE_ITEM_ARROW, NWScript.BASE_ITEM_BOLT };
    public static int[] leatherBasicBlueprints = new int[] { -1, -2, -3, -9, 114, 115, NWScript.BASE_ITEM_BELT, NWScript.BASE_ITEM_BOOTS, NWScript.BASE_ITEM_BRACER, NWScript.BASE_ITEM_CLOAK, NWScript.BASE_ITEM_GLOVES, NWScript.BASE_ITEM_WHIP };
    public static int[] lowBlueprints = new int[] { -1, -2, -3, -9, 114, 115, NWScript.BASE_ITEM_LIGHTMACE, NWScript.BASE_ITEM_HELMET, NWScript.BASE_ITEM_DAGGER, NWScript.BASE_ITEM_MORNINGSTAR, NWScript.BASE_ITEM_SHORTSPEAR, NWScript.BASE_ITEM_SICKLE, NWScript.BASE_ITEM_ARROW, NWScript.BASE_ITEM_BELT, NWScript.BASE_ITEM_AMULET, NWScript.BASE_ITEM_BOLT, NWScript.BASE_ITEM_BOOTS, NWScript.BASE_ITEM_BRACER, NWScript.BASE_ITEM_BULLET, NWScript.BASE_ITEM_CLOAK, NWScript.BASE_ITEM_CLUB, NWScript.BASE_ITEM_DART, NWScript.BASE_ITEM_GLOVES, NWScript.BASE_ITEM_HEAVYCROSSBOW, NWScript.BASE_ITEM_LIGHTCROSSBOW, NWScript.BASE_ITEM_QUARTERSTAFF, NWScript.BASE_ITEM_RING, NWScript.BASE_ITEM_SHURIKEN, NWScript.BASE_ITEM_SLING, NWScript.BASE_ITEM_SMALLSHIELD, NWScript.BASE_ITEM_TORCH };
    public static int[] mediumBlueprints = new int[] { -4, -5, NWScript.BASE_ITEM_BATTLEAXE, NWScript.BASE_ITEM_GREATAXE, NWScript.BASE_ITEM_GREATSWORD, NWScript.BASE_ITEM_HALBERD, NWScript.BASE_ITEM_HANDAXE, NWScript.BASE_ITEM_HEAVYFLAIL, NWScript.BASE_ITEM_LARGESHIELD, NWScript.BASE_ITEM_LIGHTFLAIL, NWScript.BASE_ITEM_LIGHTHAMMER, NWScript.BASE_ITEM_LONGBOW, NWScript.BASE_ITEM_LONGSWORD, NWScript.BASE_ITEM_RAPIER, NWScript.BASE_ITEM_SCIMITAR, NWScript.BASE_ITEM_SHORTBOW, NWScript.BASE_ITEM_SHORTSWORD, NWScript.BASE_ITEM_SHURIKEN, NWScript.BASE_ITEM_THROWINGAXE, NWScript.BASE_ITEM_TRIDENT, NWScript.BASE_ITEM_WARHAMMER };
    public static int[] highBlueprints = new int[] { -6, -7, -8, NWScript.BASE_ITEM_WHIP, NWScript.BASE_ITEM_TWOBLADEDSWORD, NWScript.BASE_ITEM_TOWERSHIELD, NWScript.BASE_ITEM_SCYTHE, NWScript.BASE_ITEM_KUKRI, NWScript.BASE_ITEM_KATANA, NWScript.BASE_ITEM_KAMA, NWScript.BASE_ITEM_DWARVENWARAXE, NWScript.BASE_ITEM_DOUBLEAXE, NWScript.BASE_ITEM_DIREMACE, NWScript.BASE_ITEM_BASTARDSWORD };

    public static Dictionary<int, Blueprint> blueprintDictionnary = new Dictionary<int, Blueprint>();

    public static void StartCollectCycle(PlayerSystem.Player player, Action completeCallback, NwGameObject oTarget = null)
    {
      if (player.oid.GetLocalVariable<int>("_COLLECT_IN_PROGRESS").HasValue)
      {
        player.oid.GetLocalVariable<int>("_COLLECT_CANCELLED").Value = 1;
        player.oid.SendServerMessage("Annulation de la tâche en cours.", Color.ORANGE);
        return;
      }
      
      NwItem resourceExtractor = player.oid.GetItemInSlot(InventorySlot.RightHand);
      float cycleDuration = 180.0f;
      if (Systems.Config.env == Systems.Config.Env.Chim)
        cycleDuration = 10.0f;
      
      if (resourceExtractor != null) // TODO : Idée pour plus tard, le strip miner le plus avancé pourra équipper un cristal de spécialisation pour extraire deux fois plus de minerai en un cycle sur son minerai de spécialité
      {
        cycleDuration = cycleDuration - (cycleDuration * resourceExtractor.GetLocalVariable<int>("_ITEM_LEVEL").Value * 2 / 100);
      }
      
      if (oTarget != null)
      {
        Core.Effect eRay = NWScript.EffectBeam(NWScript.VFX_BEAM_DISINTEGRATE, resourceExtractor, 1, 0, 3);
        eRay = NWScript.TagEffect(eRay, $"_{player.oid.CDKey}_MINING_BEAM");
        NWScript.ApplyEffectToObject(NWScript.DURATION_TYPE_TEMPORARY, eRay, oTarget, cycleDuration);
      }
     
      PlayerPlugin.StartGuiTimingBar(player.oid, cycleDuration);

      player.oid.OnServerDisconnect += OnDisconnectCancelCollect;
      EventsPlugin.AddObjectToDispatchList("NWNX_ON_TIMING_BAR_CANCEL_BEFORE", "collect_cancel", player.oid);
      EventsPlugin.AddObjectToDispatchList("NWNX_ON_ITEM_EQUIP_BEFORE", "collect_cancel", player.oid);
      EventsPlugin.AddObjectToDispatchList("NWNX_ON_ITEM_UNEQUIP_BEFORE", "collect_cancel", player.oid);
      EventsPlugin.AddObjectToDispatchList("NWNX_ON_START_COMBAT_ROUND_AFTER", "collect_cancel", player.oid);
      EventsPlugin.AddObjectToDispatchList("NWNX_ON_INPUT_CAST_SPELL_BEFORE", "collect_cancel", player.oid);
      
      Task waitForCollectCompletion = NwTask.Run(async () =>
      {
        player.oid.GetLocalVariable<int>("_COLLECT_IN_PROGRESS").Value = 1;

        CancellationTokenSource tokenSource = new CancellationTokenSource();
        Task collectCancelled = NwTask.WaitUntil(() => player.oid.GetLocalVariable<int>("_COLLECT_CANCELLED").Value == 1, tokenSource.Token);
        Task onMovementCancelCollect = NwTask.WaitUntilValueChanged(() => player.oid.Position, tokenSource.Token);
        Task collectCompleted = NwTask.Delay(TimeSpan.FromSeconds(cycleDuration), tokenSource.Token);
        await NwTask.WhenAny(collectCancelled, onMovementCancelCollect, collectCompleted);
        tokenSource.Cancel();

        if (collectCancelled.IsCompletedSuccessfully || onMovementCancelCollect.IsCompletedSuccessfully)
        {
          player.oid.GetLocalVariable<int>("_COLLECT_CANCELLED").Delete();

          if (oTarget != null)
            Utils.RemoveTaggedEffect(oTarget, $"_{player.oid.CDKey}_MINING_BEAM");

          RemoveCollectCycleCallbacks(player.oid);
          PlayerPlugin.StopGuiTimingBar(player.oid);
    
          player.oid.GetLocalVariable<int>("_COLLECT_IN_PROGRESS").Delete();
          return;
        }

        completeCallback();
        RemoveCollectCycleCallbacks(player.oid);
        PlayerPlugin.StopGuiTimingBar(player.oid);
        player.oid.GetLocalVariable<int>("_COLLECT_IN_PROGRESS").Delete();
      });
    }
    private static void OnDisconnectCancelCollect(OnClientDisconnect onDisconnect)
    {
      onDisconnect.Player.GetLocalVariable<int>("_COLLECT_CANCELLED").Delete();
      onDisconnect.Player.GetLocalVariable<int>("_COLLECT_IN_PROGRESS").Delete();
      RemoveCollectCycleCallbacks(onDisconnect.Player);
      PlayerPlugin.StopGuiTimingBar(onDisconnect.Player);
    }
    private static void RemoveCollectCycleCallbacks(NwPlayer oPC)
    {
      oPC.OnServerDisconnect -= OnDisconnectCancelCollect;
      EventsPlugin.RemoveObjectFromDispatchList("NWNX_ON_TIMING_BAR_CANCEL_BEFORE", "collect_cancel", oPC);
      EventsPlugin.RemoveObjectFromDispatchList("NWNX_ON_ITEM_EQUIP_BEFORE", "collect_cancel", oPC);
      EventsPlugin.RemoveObjectFromDispatchList("NWNX_ON_ITEM_UNEQUIP_BEFORE", "collect_cancel", oPC);
      EventsPlugin.RemoveObjectFromDispatchList("NWNX_ON_START_COMBAT_ROUND_AFTER", "collect_cancel", oPC);
      EventsPlugin.RemoveObjectFromDispatchList("NWNX_ON_INPUT_CAST_SPELL_BEFORE", "collect_cancel", oPC);
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
      
      foreach (API.ItemProperty ip in GetCraftItemProperties(material, craftedItem))
      {
        //NWScript.SendMessageToPC(NWScript.GetFirstPC(), $"Adding IP : {ip}");
        craftedItem.AddItemProperty(ip, EffectDuration.Permanent);
      }
    }
    public static void AddCraftedEnchantementProperties(NwItem craftedItem, string spellId, int boost)
    {
      craftedItem.AddItemProperty(GetCraftEnchantementProperties(craftedItem, spellId, boost), EffectDuration.Permanent);
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

      Utils.LogMessageToDMs($"Could not find item template for tag : {itemTag}");
      return "";
    }
  }
}
