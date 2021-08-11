using Discord;
using Google.Cloud.Translation.V2;
using NLog;
using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utils;
using NWN.Core.NWNX;
using NWN.Systems;

namespace ModuleService
{
  [ServiceBinding(typeof(ModuleSystem))]
  public class ModuleSystem
  {
    public static readonly Logger Log = LogManager.GetCurrentClassLogger();
    public static readonly TranslationClient googleTranslationClient = TranslationClient.Create();
    public static Dictionary<string, GoldBalance> goldBalanceMonitoring = new Dictionary<string, GoldBalance>();
    public ModuleSystem()
    {
      CreateDatabase();
      InitializeEvents();
      
      NwModule.Instance.OnModuleLoad += OnModuleLoad;
    }
    
    private void OnModuleLoad(ModuleEvents.OnModuleLoad onModuleLoad)
    {
      NwModule.Instance.GetObjectVariable<LocalVariableString>("X2_S_UD_SPELLSCRIPT").Value = "spellhook";

      NwServer.Instance.ServerInfo.PlayOptions.RestoreSpellUses = false;
      NwServer.Instance.ServerInfo.PlayOptions.ShowDMJoinMessage = false;

      SetModuleTime();
      SaveServerVault();  

      RestorePlayerCorpseFromDatabase();
      RestoreDMPersistentPlaceableFromDatabase();

      Task spawnResources = SpawnCollectableResources(1);
      Task deleteExpiredMail = DeleteExpiredMail();
    }
    private static void CreateDatabase()
    {
      SqLiteUtils.CreateQuery("CREATE TABLE IF NOT EXISTS moduleInfo" +
        "('year' INTEGER NOT NULL, 'month' INTEGER NOT NULL, 'day' INTEGER NOT NULL, 'hour' INTEGER NOT NULL, 'minute' INTEGER NOT NULL, 'second' INTEGER NOT NULL)");

      SqLiteUtils.CreateQuery("CREATE TABLE IF NOT EXISTS PlayerAccounts" +
        "('accountName' TEXT NOT NULL, 'cdKey' TEXT, 'bonusRolePlay' INTEGER NOT NULL, 'discordId' TEXT, 'rank' TEXT)");

      SqLiteUtils.CreateQuery("CREATE TABLE IF NOT EXISTS playerCharacters" +
        "('accountId' INTEGER NOT NULL, 'characterName' TEXT NOT NULL, 'dateLastSaved' TEXT NOT NULL, 'previousSPCalculation' TEXT, 'serializedLearnables' TEXT," +
        "'currentCraftJobRemainingTime' REAL, 'currentCraftJob' INTEGER NOT NULL, 'currentCraftObject' TEXT NOT NULL," +
        "currentCraftJobMaterial TEXT, areaTag TEXT, position TEXT, facing REAL," +
        "currentHP INTEGER, bankGold INTEGER, pveArenaCurrentPoints, menuOriginTop INTEGER, menuOriginLeft INTEGER, storage TEXT, alchemyCauldron TEXT)");

      SqLiteUtils.CreateQuery("CREATE TABLE IF NOT EXISTS playerLearnableSkills" +
        "('characterId' INTEGER NOT NULL, 'skillId' INTEGER NOT NULL, 'skillPoints' INTEGER NOT NULL, 'trained' INTEGER, 'active' TEXT, UNIQUE (characterId, skillId))");

      SqLiteUtils.CreateQuery("CREATE TABLE IF NOT EXISTS playerLearnableSpells" +
        "('characterId' INTEGER NOT NULL, 'skillId' INTEGER NOT NULL, 'skillPoints' INTEGER NOT NULL, 'trained' INTEGER, 'nbScrolls' INTEGER, 'active' TEXT, UNIQUE (characterId, skillId))");

      SqLiteUtils.CreateQuery("CREATE TABLE IF NOT EXISTS playerMaterialStorage" +
        "('characterId' INTEGER NOT NULL, 'materialName' TEXT NOT NULL, 'materialStock' INTEGER, UNIQUE (characterId, materialName))");

      SqLiteUtils.CreateQuery("CREATE TABLE IF NOT EXISTS playerDeathCorpses" +
        "('characterId' INTEGER NOT NULL, 'deathCorpse' TEXT NOT NULL, 'areaTag' TEXT NOT NULL, 'position' TEXT NOT NULL)");

      SqLiteUtils.CreateQuery("CREATE TABLE IF NOT EXISTS loot_containers" +
        "('chestTag' TEXT NOT NULL, 'accountID' INTEGER NOT NULL, 'serializedChest' TEXT NOT NULL, 'position' TEXT NOT NULL, 'facing' REAL NOT NULL, PRIMARY KEY(chestTag))");

      SqLiteUtils.CreateQuery("CREATE TABLE IF NOT EXISTS dm_persistant_placeable" +
        "('accountID' INTEGER NOT NULL, 'serializedPlaceable' TEXT NOT NULL, 'areaTag' TEXT NOT NULL, 'position' TEXT NOT NULL, 'facing' REAL NOT NULL)");

      SqLiteUtils.CreateQuery("CREATE TABLE IF NOT EXISTS playerMapPins" +
        "('characterId' INTEGER NOT NULL, 'mapPinId' INTEGER NOT NULL, 'areaTag' TEXT NOT NULL, 'x' REAL NOT NULL, 'y' REAL NOT NULL, 'note' TEXT, UNIQUE (characterId, mapPinId))");

      SqLiteUtils.CreateQuery("CREATE TABLE IF NOT EXISTS playerDescriptions" +
        "('characterId' INTEGER NOT NULL, 'descriptionName' TEXT NOT NULL, 'description' TEXT NOT NULL, UNIQUE (characterId, descriptionName))");

      SqLiteUtils.CreateQuery("CREATE TABLE IF NOT EXISTS areaResourceStock" +
        "('areaTag' TEXT NOT NULL, 'mining' INTEGER, 'wood' INTEGER, 'animals' INTEGER, PRIMARY KEY(areaTag))");

      SqLiteUtils.CreateQuery("CREATE TABLE IF NOT EXISTS scriptPerformance" +
        "('script' TEXT NOT NULL, 'nbExecutions' INTEGER NOT NULL, 'averageExecutionTime' REAL NOT NULL, 'cumulatedExecutionTime' REAL NOT NULL, PRIMARY KEY(script))");

      SqLiteUtils.CreateQuery("CREATE TABLE IF NOT EXISTS goldBalance" +
        "('lootedTag' TEXT NOT NULL, 'nbTimesLooted' INTEGER NOT NULL, 'averageGold' INT NOT NULL, 'cumulatedGold' INT NOT NULL, PRIMARY KEY(lootedTag))");

      SqLiteUtils.CreateQuery("CREATE TABLE IF NOT EXISTS playerAreaExplorationState" +
        "('characterId' INTEGER NOT NULL, 'areaTag' TEXT NOT NULL, 'explorationState' TEXT NOT NULL, UNIQUE (characterId, areaTag))");

      SqLiteUtils.CreateQuery("CREATE TABLE IF NOT EXISTS playerPrivateContracts" +
        "('characterId' INTEGER NOT NULL, 'expirationDate' TEXT NOT NULL, 'serializedContract' TEXT NOT NULL, 'totalValue' INTEGER NOT NULL)");

      SqLiteUtils.CreateQuery("CREATE TABLE IF NOT EXISTS playerBuyOrders" +
        "('characterId' INTEGER NOT NULL, 'expirationDate' TEXT NOT NULL, 'material' TEXT NOT NULL, 'quantity' INTEGER NOT NULL, 'unitPrice' INTEGER NOT NULL)");

      SqLiteUtils.CreateQuery("CREATE TABLE IF NOT EXISTS playerSellOrders" +
        "('characterId' INTEGER NOT NULL, 'expirationDate' TEXT NOT NULL, 'material' TEXT NOT NULL, 'quantity' INTEGER NOT NULL, 'unitPrice' INTEGER NOT NULL)");

      SqLiteUtils.CreateQuery("CREATE TABLE IF NOT EXISTS playerShops" +
        "('characterId' INTEGER NOT NULL, 'shop' TEXT NOT NULL, 'panel' TEXT NOT NULL, 'expirationDate' TEXT NOT NULL, 'areaTag' TEXT NOT NULL, 'position' TEXT NOT NULL, 'facing' REAL NOT NULL)");

      SqLiteUtils.CreateQuery("CREATE TABLE IF NOT EXISTS playerItemAppearance" +
        "('characterId' INTEGER NOT NULL, 'appearanceName' TEXT NOT NULL, 'serializedAppearance' TEXT NOT NULL, 'baseItemType' INTEGER NOT NULL, 'AC' INTEGER NOT NULL, UNIQUE (characterId, appearanceName))");

      SqLiteUtils.CreateQuery("CREATE TABLE IF NOT EXISTS playerAuctions" +
        "('characterId' INTEGER NOT NULL, 'shop' TEXT NOT NULL, 'panel' TEXT NOT NULL, 'expirationDate' TEXT NOT NULL, 'highestAuction' INTEGER NOT NULL, 'highestAuctionner' INTEGER NOT NULL, 'areaTag' TEXT NOT NULL, 'position' TEXT NOT NULL, 'facing' REAL NOT NULL)");

      SqLiteUtils.CreateQuery("CREATE TABLE IF NOT EXISTS savedNPC" +
        "('accountName' TEXT NOT NULL, 'name' TEXT NOT NULL, 'serializedCreature' TEXT NOT NULL, UNIQUE (accountName, name))");

      SqLiteUtils.CreateQuery("CREATE TABLE IF NOT EXISTS areaDescriptions" +
        "('areaTag' TEXT NOT NULL, 'description' TEXT NOT NULL, PRIMARY KEY(areaTag))");

      SqLiteUtils.CreateQuery("CREATE TABLE IF NOT EXISTS rumors" +
        "('accountId' INTEGER NOT NULL, 'title' TEXT NOT NULL, 'content' TEXT NOT NULL, UNIQUE (accountId, title))");

      SqLiteUtils.CreateQuery("CREATE TABLE IF NOT EXISTS arenaRewardShop" +
        "('id' INTEGER NOT NULL, 'shop' TEXT NOT NULL, PRIMARY KEY(id))");

      SqLiteUtils.CreateQuery("CREATE TABLE IF NOT EXISTS chatColors" +
        "('accountId' INTEGER NOT NULL, 'channel' INTEGER NOT NULL, 'color' INTEGER NOT NULL, UNIQUE (accountId, channel))");

      SqLiteUtils.CreateQuery("CREATE TABLE IF NOT EXISTS messenger" +
        "('characterId' INTEGER NOT NULL, 'senderName' TEXT NOT NULL, 'title' TEXT NOT NULL, 'message', TEXT NOT NULL, 'sentDate' TEXT NOT NULL, 'read' INTEGER NOT NULL)");

      SqLiteUtils.CreateQuery("CREATE TABLE IF NOT EXISTS playerGrimoire" +
        "('characterId' INTEGER NOT NULL, 'grimoireName' TEXT NOT NULL, 'serializedGrimoire' TEXT NOT NULL, UNIQUE (characterId, grimoireName))");

      SqLiteUtils.CreateQuery("CREATE TABLE IF NOT EXISTS playerQuickbar" +
        "('characterId' INTEGER NOT NULL, 'quickbarName' TEXT NOT NULL, 'serializedQuickbar' TEXT NOT NULL, UNIQUE (characterId, quickbarName))");

      SqLiteUtils.CreateQuery("CREATE TABLE IF NOT EXISTS dmVFX" +
        "('playerName' TEXT NOT NULL, 'vfxName' TEXT NOT NULL, 'vfxId' INTEGER NOT NULL, UNIQUE (playerName, vfxName))");

      SqLiteUtils.CreateQuery("CREATE TABLE IF NOT EXISTS dmVFXDuration" +
        "('playerName' TEXT NOT NULL, 'vfxDuration' INTEGER NOT NULL, PRIMARY KEY(playerName))");

      SqLiteUtils.CreateQuery("CREATE TABLE IF NOT EXISTS playerMutedPM" +
        "('accountId' INTEGER NOT NULL, 'mutedAccountId' INTEGER NOT NULL)");

      SqLiteUtils.CreateQuery("CREATE TABLE IF NOT EXISTS playerAlchemyRecipe" +
        "('characterId' INTEGER NOT NULL, 'recipeName' TEXT NOT NULL, 'serializedRecipe' TEXT NOT NULL, UNIQUE (characterId, recipeName))");
    }
    private void InitializeEvents()
    {
      EventsPlugin.SubscribeEvent("NWNX_ON_ITEM_SCROLL_LEARN_BEFORE", "b_learn_scroll");
      
      EventsPlugin.SubscribeEvent("NWNX_ON_INPUT_TOGGLE_PAUSE_BEFORE", "spacebar_down");
      EventsPlugin.ToggleDispatchListMode("NWNX_ON_INPUT_TOGGLE_PAUSE_BEFORE", "spacebar_down", 1);

      EventsPlugin.SubscribeEvent("NWNX_ON_ITEM_UNEQUIP_BEFORE", "b_unequip");
      EventsPlugin.ToggleDispatchListMode("NWNX_ON_ITEM_UNEQUIP_BEFORE", "b_unequip", 1);

      //EventsPlugin.SubscribeEvent("NWNX_ON_DM_POSSESS_FULL_POWER_BEFORE", "b_dm_possess");
      //EventsPlugin.SubscribeEvent("NWNX_ON_DM_POSSESS_BEFORE", "b_dm_possess");
      
      EventsPlugin.SubscribeEvent("NWNX_ON_USE_SKILL_BEFORE", "event_skillused");
      EventsPlugin.ToggleDispatchListMode("NWNX_ON_USE_SKILL_BEFORE", "event_skillused", 1);

      EventsPlugin.SubscribeEvent("NWNX_ON_TIMING_BAR_CANCEL_BEFORE", "collect_cancel");
      EventsPlugin.ToggleDispatchListMode("NWNX_ON_TIMING_BAR_CANCEL_BEFORE", "collect_cancel", 1);
      EventsPlugin.SubscribeEvent("NWNX_ON_CLIENT_DISCONNECT_BEFORE", "collect_cancel");
      EventsPlugin.ToggleDispatchListMode("NWNX_ON_CLIENT_DISCONNECT_BEFORE", "collect_cancel", 1);
      EventsPlugin.SubscribeEvent("NWNX_ON_ITEM_EQUIP_BEFORE", "collect_cancel");
      EventsPlugin.ToggleDispatchListMode("NWNX_ON_ITEM_EQUIP_BEFORE", "collect_cancel", 1);
      EventsPlugin.SubscribeEvent("NWNX_ON_ITEM_UNEQUIP_BEFORE", "collect_cancel");
      EventsPlugin.ToggleDispatchListMode("NWNX_ON_ITEM_UNEQUIP_BEFORE", "collect_cancel", 1);
      EventsPlugin.SubscribeEvent("NWNX_ON_START_COMBAT_ROUND_AFTER", "collect_cancel");
      EventsPlugin.ToggleDispatchListMode("NWNX_ON_START_COMBAT_ROUND_AFTER", "collect_cancel", 1);
      EventsPlugin.SubscribeEvent("NWNX_ON_INPUT_CAST_SPELL_BEFORE", "collect_cancel");
      EventsPlugin.ToggleDispatchListMode("NWNX_ON_INPUT_CAST_SPELL_BEFORE", "collect_cancel", 1);

      EventsPlugin.SubscribeEvent("NWNX_ON_JOURNAL_OPEN_AFTER", "on_journal_open");
      EventsPlugin.SubscribeEvent("NWNX_ON_JOURNAL_CLOSE_AFTER", "on_journal_close");

      EventsPlugin.SubscribeEvent("NWNX_ON_MAP_PIN_ADD_PIN_AFTER", "map_pin_added");
      EventsPlugin.SubscribeEvent("NWNX_ON_MAP_PIN_CHANGE_PIN_AFTER", "map_pin_changed");
      EventsPlugin.SubscribeEvent("NWNX_ON_MAP_PIN_DESTROY_PIN_AFTER", "mappin_destroyed");

      EventsPlugin.SubscribeEvent("NWNX_ON_INPUT_EMOTE_BEFORE", "on_input_emote");
      
      EventsPlugin.SubscribeEvent("NWNX_ON_CHARACTER_SHEET_OPEN_BEFORE", "pc_sheet_open");

      EventsPlugin.SubscribeEvent("NWNX_ON_EFFECT_APPLIED_BEFORE", "effect_applied");
      EventsPlugin.SubscribeEvent("NWNX_ON_EFFECT_REMOVED_BEFORE", "effect_removed");

      //EventsPlugin.SubscribeEvent("NWNX_ON_HAS_FEAT_AFTER", "event_has_feat");
    }
    private void SetModuleTime()
    {
      var query = SqLiteUtils.SelectQuery("moduleInfo",
        new List<string>() { { "year" }, { "month" }, { "day" }, { "hour" }, { "minute" }, { "second" } },
        new List<string[]>() { new string[] { "rowid", "1" } });
      
      if (query.Result != null)
          NwDateTime.Now = new NwDateTime(query.Result.GetInt(0), query.Result.GetInt(1), query.Result.GetInt(2), query.Result.GetInt(3), query.Result.GetInt(4), query.Result.GetInt(5));
      else
      {
        SqLiteUtils.InsertQuery("moduleInfo",
          new List<string[]>() { new string[] { "year", NwDateTime.Now.Year.ToString() }, new string[] { "month", NwDateTime.Now.Month.ToString() }, new string[] { "day", NwDateTime.Now.DayInTenday.ToString() }, new string[] { "hour", NwDateTime.Now.Hour.ToString() }, new string[] { "minute", NwDateTime.Now.Minute.ToString() }, new string[] { "second", NwDateTime.Now.Second.ToString() } });
      }
    }
    public static async Task SpawnCollectableResources(float delay)
    {
      if(delay > 0)
        await NwTask.WaitUntil(() => DateTime.Now.Hour == 5);

      Log.Info("Starting to spawn collectable ressources");

      foreach (NwWaypoint ressourcePoint in NwObject.FindObjectsWithTag<NwWaypoint>(new string[] { "ore_spawn_wp", "wood_spawn_wp" }).Where(l => l.Area.GetObjectVariable<LocalVariableInt>("_AREA_LEVEL").Value > 1))
      {
        int areaLevel = ressourcePoint.Area.GetObjectVariable<LocalVariableInt>("_AREA_LEVEL").Value;
        if (NwRandom.Roll(MiscUtils.random, 100) >= (areaLevel * 20) - 20)
        {
          string resRef = "";
          string name = "";

          switch (ressourcePoint.Tag)
          {
            case "ore_spawn_wp":
              resRef = "mineable_rock";
              name = Enum.GetName(typeof(OreConfig.OreType), OreConfig.GetRandomOreSpawnFromAreaLevel(areaLevel));
              break;
            case "wood_spawn_wp":
              resRef = "mineable_tree";
              name = Enum.GetName(typeof(WoodConfig.WoodType), WoodConfig.GetRandomWoodSpawnFromAreaLevel(areaLevel));
              break;
          }

          var newRock = NwPlaceable.Create(resRef, ressourcePoint.Location);
          newRock.Name = name;
          newRock.GetObjectVariable<LocalVariableInt>("_ORE_AMOUNT").Value = 50 * NwRandom.Roll(MiscUtils.random, 100);
          ressourcePoint.Destroy();

          Log.Info($"REFILL - {ressourcePoint.Area.Name} - {ressourcePoint.Name}");
        }
      }

      foreach (NwArea area in NwModule.Instance.Areas.Where(l => l.GetObjectVariable<LocalVariableInt>("_AREA_LEVEL").Value > 1))
      {
        int areaLevel = area.GetObjectVariable<LocalVariableInt>("_AREA_LEVEL").Value;

        SqLiteUtils.InsertQuery("areaResourceStock",
          new List<string[]>() { new string[] { "areaTag", area.Tag }, new string[] { "mining", (areaLevel * 2).ToString() }, new string[] { "wood", (areaLevel * 2).ToString() }, new string[] { "animals", (areaLevel * 2).ToString() } },
          new List<string>() { "areaTag" },
          new List<string[]>() { new string[] { "mining" }, new string[] { "wood" }, new string[] { "animals" } });
      }

      await NwTask.WaitUntilValueChanged(() => DateTime.Now.Day);
      await SpawnCollectableResources(1);
    }
    private void SaveServerVault()
    {
      SqLiteUtils.UpdateQuery("moduleInfo",
        new List<string[]>() { new string[] { "year", NwDateTime.Now.Year.ToString() }, { new string[] { "month", NwDateTime.Now.Month.ToString() } }, { new string[] { "day", NwDateTime.Now.DayInTenday.ToString() } }, { new string[] { "hour", NwDateTime.Now.Hour.ToString() } }, { new string[] { "minute", NwDateTime.Now.Minute.ToString() } }, { new string[] { "second", NwDateTime.Now.Second.ToString() } } },
        new List<string[]>() { new string[] { "ROWID", "1" } });

      HandleExpiredAuctions();

      Task DownloadDiscordUsers = Task.Run(async () =>
      {
        await DiscordUtils._client.DownloadUsersAsync(new List<IGuild> { { DiscordUtils._client.GetGuild(680072044364562528) } });
      });

      Task scheduleNextSave = NwTask.Run(async () =>
      {
        await NwTask.Delay(TimeSpan.FromMinutes(10));
        SaveServerVault();
        return true;
      });      
    }
    public void RestorePlayerCorpseFromDatabase()
    {
      var result = SqLiteUtils.SelectQuery("playerDeathCorpses",
        new List<string>() { { "deathCorpse" }, { "areaTag" }, { "position" }, { "characterId" } },
        new List<string[]>() );

      foreach (var pcCorpse in result.Results)
      {
        NwCreature corpse = NwCreature.Deserialize(pcCorpse.GetString(0).ToByteArray());
        corpse.Location = MiscUtils.GetLocationFromDatabase(pcCorpse.GetString(1), pcCorpse.GetString(2), 0);
        corpse.GetObjectVariable<LocalVariableInt>("_PC_ID").Value = pcCorpse.GetInt(3);

        foreach (NwItem item in corpse.Inventory.Items.Where(i => i.Tag != "item_pccorpse"))
          item.Destroy();

        PlayerSystem.SetupPCCorpse(corpse);
      }
    }
    
    public async void HandleExpiredAuctions()
    {
      var result = SqLiteUtils.SelectQuery("playerAuctions",
        new List<string>() { { "characterId" }, { "rowid" }, { "highestAuction" }, { "highestAuctionner" }, { "shop" } },
        new List<string[]>() { new string[] { "expirationDate", DateTime.Now.ToString(), ">" } });

      foreach (var auction in result.Results)
      {
        int buyerId = auction.GetInt(3);
        int sellerId = auction.GetInt(0);
        int auctionId = auction.GetInt(1);

        NwPlayer oSeller = NwModule.Instance.Players.FirstOrDefault(p => p.LoginCreature.GetObjectVariable<PersistentVariableInt>("characterId").Value == sellerId);
        NwStore store = NwObject.FindObjectsOfType<NwStore>().FirstOrDefault(p => p.GetObjectVariable<LocalVariableInt>("_AUCTION_ID").Value == auctionId);
        NwStore oStore = SqLiteUtils.StoreSerializationFormatProtection(auction, 4, NwModule.Instance.StartingLocation);
        NwItem tempItem = oStore.Items.FirstOrDefault();
        oStore.Destroy();

        if (buyerId <= 0) // pas d'acheteur
        {
          // S'il est co, on rend l'item au seller et on détruit la ligne en BDD. S'il est pas co, on attend la prochaine occurence pour lui rendre l'item
          if (oSeller != null)
          {
            tempItem.Clone(oSeller.LoginCreature);
            NwItem authorization = await NwItem.Create("auction_clearanc", oSeller.LoginCreature);
            oSeller.SendServerMessage($"Aucune enchère sur votre {tempItem.Name.ColorString(ColorConstants.Orange)}. L'objet vous a donc été restitué.");
          }
          else
          {
            MiscUtils.SendMailToPC(sellerId, "Hotel des ventes de Similisse", $"Enchère sur {tempItem.Name} expirée",
              $"Très honoré vendeur, \n\n Nous avons le regret de vous annoncer que votre mise aux enchères de {tempItem.Name} a expiré sans trouver d'acheteur. \n\n L'objet a été restitué à votre entrepot. \n\n Signé : Polpo");

            MiscUtils.SendItemToPCStorage(sellerId, tempItem);
          }
        }
        else
        {
          // Si highestAuction > 0 On donne les sous au seller et on lui envoie un message s'il est co. S'il n'est pas co on met à jour en bdd et on lui envoie un courrier
          int highestAuction = auction.GetInt(2);

          if (highestAuction > 0)
          {
            if (oSeller != null)
            {
              if (PlayerSystem.Players.TryGetValue(oSeller.LoginCreature, out PlayerSystem.Player seller))
              {
                seller.bankGold += highestAuction * 95 / 100;
                oSeller.SendServerMessage($"Votre enchère vous a permis de remporter {(highestAuction * 95 / 100).ToString().ColorString(ColorConstants.Orange)}. L'or a été versé à votre banque !");
              }

              NwItem authorization = await NwItem.Create("auction_clearanc", oSeller.LoginCreature);
            }
            else
            {
              Task delayedUpdate = NwTask.Run(async () =>
              {
                await NwTask.Delay(TimeSpan.FromSeconds(0.2));

                SqLiteUtils.UpdateQuery("playerCharacters",
                  new List<string[]>() { new string[] { "bankGold", highestAuction.ToString(), "+" } },
                  new List<string[]>() { new string[] { "ROWID", sellerId.ToString() } });
              });

              MiscUtils.SendMailToPC(sellerId, "Hotel des ventes de Similisse", $"Enchère sur {tempItem.Name} conclue",
                $"Très honoré vendeur, \n\n Nous avons l'immense plaisir de vous annoncer que votre enchère sur {tempItem.Name} a porté ses fruits. \n\n Celle-ci vous a permis d'acquérir {highestAuction} pièce(s) d'or ! \n\n Signé : Polpo");
            }
          }
          // Si le buyer est co, on lui file l'item et on détruit la ligne en BDD. S'il est pas co, on transfère dans son entrepot perso
          NwPlayer oBuyer = NwModule.Instance.Players.FirstOrDefault(p => p.LoginCreature.GetObjectVariable<PersistentVariableInt>("characterId").Value == buyerId);

          if (oBuyer != null)
          {
            tempItem.Clone(oBuyer.LoginCreature);
            oSeller.SendServerMessage($"Vous venez de remporter l'enchère sur {tempItem.Name.ColorString(ColorConstants.Orange)}. L'objet se trouve désormais dans votre inventaire.");
          }
          else
          {
            MiscUtils.SendMailToPC(sellerId, "Hotel des ventes de Similisse", $"Enchère sur {tempItem.Name} remportée",
                $"Très honoré vendeur, \n\n Nous avons l'immense plaisir de vous annoncer que vous avez remporté l'enchère sur {tempItem.Name}. \n\n L'objet a été transformé dans votre entrepôt personnel ! \n\n Signé : Polpo");

            MiscUtils.SendItemToPCStorage(buyerId, tempItem);
          }
        }

        // On détruit la shop et le panel s'ils ne sont pas null
        if (store != null)
          store.Destroy();

        NwPlaceable panel = NwObject.FindObjectsOfType<NwPlaceable>().FirstOrDefault(p => p.GetObjectVariable<LocalVariableInt>("_AUCTION_ID").Value == auctionId);
        if (panel != null)
          panel.Destroy();

        tempItem.Destroy();

        Task delayedDeletion = NwTask.Run(async () =>
        {
          await NwTask.Delay(TimeSpan.FromSeconds(0.2));
          DeleteExpiredAuction(auctionId);
        });
      }
    }
    private static void DeleteExpiredAuction(int auctionId)
    {
      SqLiteUtils.DeletionQuery("playerAuctions",
          new Dictionary<string, string>() { { "rowid", auctionId.ToString() } });
    }
    private void UpdateExpiredAuction(int auctionId)
    {
      SqLiteUtils.UpdateQuery("playerAuctions",
        new List<string[]>() { new string[] { "highestAuction", "0" } },
        new List<string[]>() { new string[] { "ROWID", auctionId.ToString() } });
    }
    public void RestoreDMPersistentPlaceableFromDatabase()
    {
      var result = SqLiteUtils.SelectQuery("dm_persistant_placeable",
        new List<string>() { { "serializedPlaceable" }, { "areaTag" }, { "position" }, { "facing" } },
        new List<string[]>() );
      
      foreach (var plc in result.Results)
        SqLiteUtils.PlaceableSerializationFormatProtection(plc, 0, MiscUtils.GetLocationFromDatabase(plc.GetString(1), plc.GetString(2), plc.GetFloat(3)));
    }
    public static async Task DeleteExpiredMail()
    {
      await NwTask.WaitUntil(() => DateTime.Now.Hour == 5);

      Log.Info("Deleting expired mails");

      SqLiteUtils.DeletionQuery("messenger",
          new Dictionary<string, string>() { { "expirationDate", DateTime.Now.AddDays(30).ToLongDateString() } }, ">");

      await NwTask.WaitUntilValueChanged(() => DateTime.Now.Day);
      await DeleteExpiredMail();
    }
  }
}
