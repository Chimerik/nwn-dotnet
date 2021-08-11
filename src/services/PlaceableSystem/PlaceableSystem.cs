using System;
using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;
using System.Linq;
using NLog;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Numerics;
using Utils;
using NWN.Core.NWNX;
using NWN.Core;

namespace NWN.Systems
{
  [ServiceBinding(typeof(PlaceableSystem))]
  public class PlaceableSystem
  {
    public static readonly Logger Log = LogManager.GetCurrentClassLogger();
    public PlaceableSystem()
    {
      foreach (NwDoor door in NwObject.FindObjectsOfType<NwDoor>())
        door.OnOpen += HandleDoorAutoClose;
      
      foreach (NwPlaceable bassin in NwObject.FindObjectsWithTag<NwPlaceable>("ench_bsn"))
        bassin.OnClose += HandleCloseEnchantementBassin;

      foreach (NwPlaceable balancoire in NwObject.FindObjectsWithTag<NwPlaceable>("balancoire"))
        balancoire.OnUsed += OnUsedBalancoire;

      foreach (NwPlaceable portal in NwObject.FindObjectsWithTag<NwPlaceable>("portal_storage_in"))
        portal.OnUsed += OnUsedStoragePortalIn;

      foreach (NwPlaceable goplouf in NwObject.FindObjectsWithTag<NwPlaceable>("go_plouf"))
        goplouf.OnUsed += OnUsedGoPlouf;

      foreach (NwPlaceable stopplouf in NwObject.FindObjectsWithTag<NwPlaceable>("stop_plouf"))
        stopplouf.OnUsed += OnUsedStopPlouf;

      foreach (NwPlaceable plc in NwObject.FindObjectsWithTag<NwPlaceable>("portal_start", "respawn_neutral", "respawn_dire", "respawn_radiant", "theater_rope"))
        plc.OnUsed += HandlePlaceableUsed;

      foreach (NwCreature statue in NwObject.FindObjectsWithTag<NwCreature>("Statuereptilienne", "statue_tiamat"))
      {
        statue.OnConversation += HandleCancelStatueConversation;
        statue.OnSpawn += HandleSpawnStatufy;
      }

      foreach (NwCreature corpse in NwObject.FindObjectsWithTag<NwCreature>("dead_wererat"))
      {
        corpse.OnConversation += HandleCancelStatueConversation;
        corpse.OnSpawn += HandleSetUpDeadCreatureCorpse;
      }

      foreach (NwCreature trainer in NwObject.FindObjectsWithTag<NwCreature>("damage_trainer"))
      {
        trainer.MaxHP = 9999;
        trainer.HP = 9999;
        trainer.BaseAC = (sbyte)trainer.GetObjectVariable<LocalVariableInt>("AC").Value;

        trainer.OnConversation += HandleCancelStatueConversation;
        trainer.OnSpawn += HandleSpawnTrainingDummy;
      }

      RestorePlayerShopsFromDatabase();
      RestorePlayerAuctionsFromDatabase();
    }
    public static void OnUsedStoragePortalIn(PlaceableEvents.OnUsed onUsed)
    {
      if (!PlayerSystem.Players.TryGetValue(onUsed.UsedBy.ControllingPlayer.LoginCreature, out PlayerSystem.Player player))
        return;
      
      NwArea area = AreaSystem.CreatePersonnalStorageArea(onUsed.UsedBy, player.characterId);
      
      onUsed.UsedBy.Location = area.FindObjectsOfTypeInArea<NwWaypoint>().FirstOrDefault(w => w.Tag == "wp_inentrepot").Location;
    }
    public static void OnUsedStoragePortalOut(PlaceableEvents.OnUsed onUsed)
    {
      onUsed.UsedBy.Location = NwModule.FindObjectsWithTag<NwWaypoint>("wp_outentrepot").FirstOrDefault().Location;
    }
    public static void OnUsedBalancoire(PlaceableEvents.OnUsed onUsed)
    {
      Task waitForAnimation = NwTask.Run(async () =>
      {
        NwPlaceable sitter = onUsed.Placeable.GetNearestObjectsByType<NwPlaceable>().FirstOrDefault(p => p.Tag == "balancoiresitter");
        NwPlaceable usedSwing = onUsed.Placeable;
        NwCreature oPC = onUsed.UsedBy;

        await oPC.ActionSit(sitter);

        usedSwing.OnUsed -= OnUsedBalancoire;
        usedSwing.OnLeftClick += OnClickSwingBalancoire;

        usedSwing.GetObjectVariable<LocalVariableObject<NwCreature>>("_SWING_TARGET").Value = oPC;

        Task onMovementCancelSwing = NwTask.Run(async () =>
        {
          await NwTask.Delay(TimeSpan.FromSeconds(1));
          await NwTask.WaitUntilValueChanged(() => oPC.Position);

          usedSwing.GetObjectVariable<LocalVariableObject<NwCreature>>("_SWING_TARGET").Delete();
          usedSwing.GetObjectVariable<LocalVariableInt>("_IS_SWINGING").Delete();

          VisualTransformLerpSettings vtLerpSettings = new VisualTransformLerpSettings 
          {
            LerpType = VisualTransformLerpType.SmootherStep,
            Duration = TimeSpan.FromSeconds(2),
            PauseWithGame = true,
            ReturnDestinationTransform = true
          };

          usedSwing.VisualTransform.Lerp(vtLerpSettings, transform =>
          {
            transform.Translation = new Vector3(0, 0, 0);
            transform.Rotation = new Vector3(0, 0, 0);
          });

          oPC.VisualTransform.Lerp(vtLerpSettings, transform =>
          {
            transform.Translation = new Vector3(0, 0, 0);
            transform.Rotation = new Vector3(0, 0, 0);
          });

          usedSwing.OnUsed += OnUsedBalancoire;
          usedSwing.OnLeftClick -= OnClickSwingBalancoire;
        });
      });
    }
    public static void OnClickSwingBalancoire(PlaceableEvents.OnLeftClick onClick)
    {
      NwPlaceable swing = onClick.Placeable;
      NwCreature oPC = onClick.Placeable.GetObjectVariable<LocalVariableObject<NwCreature>>("_SWING_TARGET").Value;

      if (swing.GetObjectVariable<LocalVariableInt>("_IS_SWINGING").HasValue)
      {
        swing.GetObjectVariable<LocalVariableInt>("_IS_SWINGING").Delete();

        Task waitSwingEnd = NwTask.Run(async () =>
        {
          await NwTask.Delay(TimeSpan.FromSeconds(2));
          
          VisualTransformLerpSettings vtLerpSettings = new VisualTransformLerpSettings
          {
            LerpType = VisualTransformLerpType.SmootherStep,
            Duration = TimeSpan.FromSeconds(1),
            PauseWithGame = true,
            ReturnDestinationTransform = true
          };

          swing.VisualTransform.Lerp(vtLerpSettings, transform =>
          {
            transform.Translation = new Vector3(0, 0, 0);
            transform.Rotation = new Vector3(0, 0, 0);
          });

          oPC.VisualTransform.Lerp(vtLerpSettings, transform =>
          {
            transform.Translation = new Vector3(0, 0, 0);
            transform.Rotation = new Vector3(0, 0, 0);
          });
        });
      }
      else
      {
        swing.GetObjectVariable<LocalVariableInt>("_IS_SWINGING").Value = 1;

        VisualTransformLerpSettings vtLerpSettings = new VisualTransformLerpSettings
        {
          LerpType = VisualTransformLerpType.SmootherStep,
          Duration = TimeSpan.FromSeconds(2),
          PauseWithGame = true,
          ReturnDestinationTransform = true
        };

        oPC.VisualTransform.Lerp(vtLerpSettings, transform =>
        {
          transform.Translation = new Vector3(-0.75f, 0, 0);
          transform.Rotation = new Vector3(0, 0, 15);
        });

        swing.VisualTransform.Lerp(vtLerpSettings, transform =>
        {
          transform.Translation = new Vector3(0.75f, 0, 0);
          transform.Rotation = new Vector3(0, 0, -15);
        });

        Task waitLoopEnd = NwTask.Run(async () =>
        {
          await NwTask.Delay(TimeSpan.FromSeconds(2));
          if(swing.GetObjectVariable<LocalVariableInt>("_IS_SWINGING").HasValue)
            HandleSwing(swing, oPC);
        });
      }
    }
    public static void HandleSwing(NwPlaceable swing, NwCreature oPC)
    {
      VisualTransformLerpSettings vtLerpSettings = new VisualTransformLerpSettings
      {
        LerpType = VisualTransformLerpType.SmootherStep,
        Duration = TimeSpan.FromSeconds(2),
        PauseWithGame = true,
        ReturnDestinationTransform = true
      };

      oPC.VisualTransform.Lerp(vtLerpSettings, transform =>
      {
        transform.Translation = new Vector3(-oPC.VisualTransform.Translation.X, 0, 0);
        transform.Rotation = new Vector3(0, 0, -oPC.VisualTransform.Rotation.Z);
      });

      swing.VisualTransform.Lerp(vtLerpSettings, transform =>
      {
        transform.Translation = new Vector3(-swing.VisualTransform.Translation.X, 0, 0);
        transform.Rotation = new Vector3(0, 0, -swing.VisualTransform.Rotation.Z);
      });

      Task waitLoopEnd = NwTask.Run(async () =>
      {
        await NwTask.Delay(TimeSpan.FromSeconds(2));

        if (swing.GetObjectVariable<LocalVariableInt>("_IS_SWINGING").HasValue)
          HandleSwing(swing, oPC);
      });
    }
    public static void HandleCleanDMPLC(PlaceableEvents.OnDeath onDeath)
    {
      NwPlaceable plc = onDeath.KilledObject;
      int plcID = plc.GetObjectVariable<LocalVariableInt>("_ID").Value;
      if (plcID > 0)
      {
        SqLiteUtils.DeletionQuery("dm_persistant_placeable",
          new Dictionary<string, string>() { { "rowid", plcID.ToString() } });
      }
      else
        MiscUtils.LogMessageToDMs($"Persistent placeable {plc.Name} in area {plc.Area.Name} does not have a valid ID !");
    }
    public static void HandlePlaceableUsed(PlaceableEvents.OnUsed onUsed)
    {
      Log.Info($"{onUsed.UsedBy.Name} used {onUsed.Placeable.Tag}");
      if (!PlayerSystem.Players.TryGetValue(onUsed.UsedBy.ControllingPlayer.ControlledCreature, out PlayerSystem.Player player))
        return;

      switch (onUsed.Placeable.Tag)
      {
        case "respawn_neutral":
          player.Respawn();
          break;
        case "respawn_radiant":
          player.Respawn();
          break;
        case "respawn_dire":
          player.Respawn();
          break;
        case "theater_rope":
          int visibilty;
          if (onUsed.UsedBy.Area.GetObjectVariable<LocalVariableInt>("_THEATER_CURTAIN_OPEN").HasNothing)
          {
            visibilty = VisibilityPlugin.NWNX_VISIBILITY_HIDDEN;
            onUsed.UsedBy.Area.GetObjectVariable<LocalVariableInt>("_THEATER_CURTAIN_OPEN").Value = 1;
          }
          else
          {
            visibilty = VisibilityPlugin.NWNX_VISIBILITY_VISIBLE;
            onUsed.UsedBy.Area.GetObjectVariable<LocalVariableInt>("_THEATER_CURTAIN_OPEN").Delete();
          }

          foreach (NwPlaceable plc in onUsed.UsedBy.Area.FindObjectsOfTypeInArea<NwPlaceable>().Where(o => o.Tag == "theater_curtain"))
            VisibilityPlugin.SetVisibilityOverride(NWScript.OBJECT_INVALID, plc, visibilty);
          break;
        case "portal_start":
          onUsed.UsedBy.Location = NwObject.FindObjectsWithTag<NwWaypoint>("WP_START_NEW_CHAR").FirstOrDefault().Location;
          break;
      }
    }
    private void HandleCancelStatueConversation(CreatureEvents.OnConversation onConversation)
    {

    }
    private void HandleSpawnStatufy(CreatureEvents.OnSpawn onSpawn)
    {
      Effect eff = Effect.CutsceneGhost();
      eff.SubType = EffectSubType.Supernatural;
      onSpawn.Creature.ApplyEffect(EffectDuration.Permanent, eff);

      eff = Effect.VisualEffect(VfxType.DurFreezeAnimation);
      eff.Tag = "_FREEZE_EFFECT";
      eff.SubType = EffectSubType.Supernatural;
      onSpawn.Creature.ApplyEffect(EffectDuration.Permanent, eff);

      if (onSpawn.Creature.Tag != "statue_tiamat")
      {
        eff = Effect.VisualEffect((VfxType)927);
        eff.SubType = EffectSubType.Supernatural;
        onSpawn.Creature.ApplyEffect(EffectDuration.Permanent, eff);
      }

      onSpawn.Creature.HighlightColor = ColorConstants.Black;
      onSpawn.Creature.MouseCursor = MouseCursor.Walk;
      onSpawn.Creature.AiLevel = AiLevel.VeryLow;
    }
    private void HandleSpawnTrainingDummy(CreatureEvents.OnSpawn onSpawn)
    {
      Effect eff = Effect.CutsceneGhost();
      eff.SubType = EffectSubType.Supernatural;
      onSpawn.Creature.ApplyEffect(EffectDuration.Permanent, eff);

      eff = Effect.CutsceneParalyze();
      eff.Tag = "_FREEZE_EFFECT";
      eff.SubType = EffectSubType.Supernatural;
      onSpawn.Creature.ApplyEffect(EffectDuration.Permanent, eff);
      onSpawn.Creature.AiLevel = AiLevel.VeryLow;
    }
    private async void HandleDoorAutoClose(DoorEvents.OnOpen onOpen)
    {
      await NwTask.Delay(TimeSpan.FromSeconds(10));
      await onOpen.Door.Close();
      if (onOpen.Door.TransitionTarget is NwDoor)
        await ((NwDoor)onOpen.Door.TransitionTarget).Close();
    }
    public void HandleCloseEnchantementBassin(PlaceableEvents.OnClose onClose)
    {
      NwCreature oPC = onClose.ClosedBy;

      if (!PlayerSystem.Players.TryGetValue(oPC, out PlayerSystem.Player player))
        return;

      NwItem oItem = onClose.Placeable.Inventory.Items.FirstOrDefault();

      if (oItem == null)
      {
        player.oid.SendServerMessage("Aucun objet valide n'a été déposé dans le bassin.");
        return;
      }

      if (!BaseItems2da.baseItemTable.GetBaseItemDataEntry(oItem.BaseItemType).IsEquippable)
      {
        player.oid.SendServerMessage("Impossible d'enchanter un objet non équippable.");
        return;
      }

      if (oItem.PlotFlag)
      {
        player.oid.SendServerMessage("Impossible d'enchanter cet objet.");
        return;
      }

      EnchantmentBasinSystem.GetEnchantmentBasinFromTag(oItem.Tag).DrawMenu(player, oItem, onClose.Placeable);
    }
    private void HandleSetUpDeadCreatureCorpse(CreatureEvents.OnSpawn onSpawn)
    {
      onSpawn.Creature.ApplyEffect(EffectDuration.Instant, Effect.Death());
    }
    public static void OnUsedPlayerOwnedShop(PlaceableEvents.OnUsed onUsed)
    {
      if (!PlayerSystem.Players.TryGetValue(onUsed.UsedBy, out PlayerSystem.Player player))
        return;

      if (onUsed.Placeable.GetObjectVariable<LocalVariableInt>("_OWNER_ID").Value == player.characterId)
      {
        PlayerOwnedShop.DrawMainPage(player, onUsed.Placeable);
      }
      else
      {
        NwStore shop = onUsed.Placeable.GetNearestObjectsByType<NwStore>().FirstOrDefault(s => s.GetObjectVariable<LocalVariableInt>("_SHOP_ID").Value == onUsed.Placeable.GetObjectVariable<LocalVariableInt>("_SHOP_ID").Value);
        
        if (shop == null)
        {
          player.oid.SendServerMessage("Cette boutique n'est pas accessible pour le moment.", ColorConstants.Orange);
          return;
        }

        shop.OnOpen -= StoreSystem.OnOpenOtherPlayerShop;
        shop.OnOpen += StoreSystem.OnOpenOtherPlayerShop;
        shop.Open(player.oid);
      }
    }
    public static void OnUsedPlayerOwnedAuction(PlaceableEvents.OnUsed onUsed)
    {
      if (!PlayerSystem.Players.TryGetValue(onUsed.UsedBy, out PlayerSystem.Player player))
        return;

      if (onUsed.Placeable.GetObjectVariable<LocalVariableInt>("_OWNER_ID").Value == player.characterId)
      {
        PlayerOwnedAuction.DrawMainPage(player, onUsed.Placeable);
      }
      else
      {
        NwStore shop = onUsed.Placeable.GetNearestObjectsByType<NwStore>().FirstOrDefault(s => s.GetObjectVariable<LocalVariableInt>("_AUCTION_ID").Value == onUsed.Placeable.GetObjectVariable<LocalVariableInt>("_AUCTION_ID").Value);

        if (shop == null)
        {
          player.oid.SendServerMessage("Cette enchère n'est pas accessible pour le moment.", ColorConstants.Orange);
          return;
        }

        shop.OnOpen -= StoreSystem.OnOpenOtherPlayerAuction;
        shop.OnOpen += StoreSystem.OnOpenOtherPlayerAuction;
        shop.Open(player.oid);
        PlayerOwnedAuction.GetAuctionPrice(player, shop, onUsed.Placeable);
      }
    }
    public static void OnUsedPersonnalStorage(PlaceableEvents.OnUsed onUsed)
    {
      if (!PlayerSystem.Players.TryGetValue(onUsed.UsedBy, out PlayerSystem.Player player))
        return;

      NwStore storage = onUsed.Placeable.GetNearestObjectsByType<NwStore>().FirstOrDefault();

      if(storage == null)
      {
        MiscUtils.LogMessageToDMs($"Entrepôt personnel non initialisé pour : {onUsed.UsedBy.Name}");
        return;
      }

      storage.Tag = "_PLAYER_STORAGE";
      storage.GetObjectVariable<LocalVariableInt>("_OWNER_ID").Value = player.characterId;
      storage.OnOpen += StoreSystem.OnOpenPersonnalStorage;
      storage.Open(player.oid);
    }
    
    public static void OnUsedGoPlouf(PlaceableEvents.OnUsed onUsed)
    {
      NwPlaceable stopplouf = NwObject.FindObjectsWithTag<NwPlaceable>("stop_plouf").FirstOrDefault();
      onUsed.UsedBy.Location = stopplouf.Location;
    }
    public static void OnUsedStopPlouf(PlaceableEvents.OnUsed onUsed)
    {
      NwPlaceable goplouf = NwObject.FindObjectsWithTag<NwPlaceable>("go_plouf").FirstOrDefault();
      onUsed.UsedBy.Location = goplouf.Location;
    }
    public void RestorePlayerShopsFromDatabase()
    {
      // TODO : envoyer un mp discord + courrier aux joueurs 7 jours avant expiration + 1 jour avant expiration

      var result = SqLiteUtils.SelectQuery("playerShops",
        new List<string>() { { "shop" }, { "panel" }, { "characterId" }, { "rowid" }, { "expirationDate" }, { "areaTag" }, { "position" }, { "facing" } },
        new List<string[]>());

      foreach (var playerShop in result.Results)
      {
        NwStore shop = SqLiteUtils.StoreSerializationFormatProtection(playerShop, 0, MiscUtils.GetLocationFromDatabase(playerShop.GetString(5), playerShop.GetString(6), playerShop.GetFloat(7)));
        NwPlaceable panel = SqLiteUtils.PlaceableSerializationFormatProtection(playerShop, 1, MiscUtils.GetLocationFromDatabase(playerShop.GetString(5), playerShop.GetString(6), playerShop.GetFloat(7)));
        shop.GetObjectVariable<LocalVariableInt>("_OWNER_ID").Value = playerShop.GetInt(2);
        shop.GetObjectVariable<LocalVariableInt>("_SHOP_ID").Value = playerShop.GetInt(3);
        panel.GetObjectVariable<LocalVariableInt>("_OWNER_ID").Value = playerShop.GetInt(2);
        panel.GetObjectVariable<LocalVariableInt>("_SHOP_ID").Value = playerShop.GetInt(3);
        double expirationTime = (DateTime.Now - DateTime.Parse(playerShop.GetString(4))).TotalDays;

        int ownerId = shop.GetObjectVariable<LocalVariableInt>("_OWNER_ID").Value;

        if (expirationTime < 0)
        {
          MiscUtils.SendMailToPC(ownerId, "Hôtel des ventes de Similisse", "Expiration du certificat de votre échoppe",
            $"Cher Marchand, \n\n Nous sommes au regret de vous informer que votre échoppe {panel.Name} a expiré. Nos hommes n'étant plus en mesure de la protéger, il se peut qu'elle ait été pillée par des vandales de passage ! \n\n Nous vous enjoignons à renouveller au plus vite votre certificat auprès de nos service. \n\n Signé : Polpo");

          DeleteExpiredShop(ownerId);
          MiscUtils.SendDiscordPMToPlayer(ownerId, $"Cher Marchand, \n\n Nous sommes au regret de vous informer que votre échoppe { panel.Name} a expiré. Nos hommes n'étant plus en mesure de la protéger, il se peut qu'elle ait été pillée par des vandales de passage! \n\n Nous vous enjoignons à renouveller au plus vite votre certificat auprès de nos service. \n\n Signé : Polpo");
        }
        if (expirationTime < 2)
        {
          MiscUtils.SendMailToPC(ownerId, "Hôtel des ventes de Similisse", "Expiration prochaine du certificat de votre échoppe",
            $"Cher Marchand, \n\n Nous sommes au devoir de vous informer que le certificat de votre échoppe {panel.Name} aura expiré dès demain. Nos hommes ne seront alors plus en mesure de la protéger, c'est courir le risque de la voir pillée par des vandales de passage ! \n\n Nous vous enjoignons à renouveller au plus vite votre certificat auprès de nos service. \n\n Signé : Polpo");

          MiscUtils.SendDiscordPMToPlayer(ownerId, $"Cher Marchand, \n\n Nous sommes au regret de vous informer que votre échoppe { panel.Name} aura expiré dès demain. Nos hommes ne seront alors plus en mesure de la protéger, c'est courir le risque de la voir pillée par des vandales de passage ! \n\n Nous vous enjoignons à renouveller au plus vite votre certificat auprès de nos service. \n\n Signé : Polpo");
        }
        else if (expirationTime < 8)
        {
          MiscUtils.SendMailToPC(ownerId, "Hôtel des ventes de Similisse", "Expiration prochaine du certificat de votre échoppe",
            $"Cher Marchand, \n\n Nous sommes au devoir de vous informer que le certificat de votre échoppe {panel.Name} aura expiré dès la semaine prochaine. Nos hommes ne seront alors plus en mesure de la protéger, c'est courir le risque de la voir pillée par des vandales de passage ! \n\n Nous vous enjoignons à renouveller au plus vite votre certificat auprès de nos service. \n\n Signé : Polpo");

          MiscUtils.SendDiscordPMToPlayer(ownerId, $"Cher Marchand, \n\n Nous sommes au regret de vous informer que votre échoppe { panel.Name} aura expiré dès la semaine prochaine. Nos hommes ne seront alors plus en mesure de la protéger, c'est courir le risque de la voir pillée par des vandales de passage ! \n\n Nous vous enjoignons à renouveller au plus vite votre certificat auprès de nos service. \n\n Signé : Polpo");
        }

        panel.OnUsed += OnUsedPlayerOwnedShop;

        foreach (NwItem item in shop.Items)
          item.BaseGoldValue = (uint)item.GetObjectVariable<LocalVariableInt>("_SET_SELL_PRICE").Value;
      }
    }
    private async void DeleteExpiredShop(int rowid)
    {
      await NwTask.Delay(TimeSpan.FromSeconds(0.2));

      SqLiteUtils.DeletionQuery("playerShops",
          new Dictionary<string, string>() { { "rowid", rowid.ToString() } });
    }
    private void RestorePlayerAuctionsFromDatabase()
    {
      var result = SqLiteUtils.SelectQuery("playerAuctions",
        new List<string>() { { "shop" }, { "panel" }, { "characterId" }, { "rowid" }, { "expirationDate" }, { "highestAuction" }, { "highestAuctionner" }, { "areaTag" }, { "position" }, { "facing" } },
        new List<string[]>() { new string[] { "shop", "deleted", "!=" } });

      foreach (var auction in result.Results)
      {
        NwStore shop = SqLiteUtils.StoreSerializationFormatProtection(auction, 0, MiscUtils.GetLocationFromDatabase(auction.GetString(7), auction.GetString(8), auction.GetFloat(9)));
        NwPlaceable panel = SqLiteUtils.PlaceableSerializationFormatProtection(auction, 1, MiscUtils.GetLocationFromDatabase(auction.GetString(7), auction.GetString(8), auction.GetFloat(9)));
        shop.GetObjectVariable<LocalVariableInt>("_OWNER_ID").Value = auction.GetInt(2);
        shop.GetObjectVariable<LocalVariableInt>("_SHOP_ID").Value = auction.GetInt(3);
        shop.GetObjectVariable<LocalVariableInt>("_CURRENT_AUCTION").Value = auction.GetInt(5);
        shop.GetObjectVariable<LocalVariableInt>("_CURRENT_AUCTIONNER").Value = auction.GetInt(6);
        panel.GetObjectVariable<LocalVariableInt>("_OWNER_ID").Value = auction.GetInt(2);
        panel.GetObjectVariable<LocalVariableInt>("_SHOP_ID").Value = auction.GetInt(3);

        panel.OnUsed += OnUsedPlayerOwnedAuction;

        foreach (NwItem item in shop.Items)
          item.BaseGoldValue = (uint)item.GetObjectVariable<LocalVariableInt>("_CURRENT_AUCTION").Value;
      }
    }
  }
}
