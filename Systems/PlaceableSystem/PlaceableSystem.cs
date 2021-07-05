﻿using System;
using NWN.API;
using NWN.API.Events;
using NWN.Core;
using NWN.Core.NWNX;
using NWN.Services;
using NWN.API.Constants;
using System.Linq;
using NLog;
using System.Threading.Tasks;
using static NWN.Systems.PlayerSystem;
using NWN.System;
using System.Collections.Generic;
using System.Numerics;

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

      foreach (NwPlaceable dicePoker in NwObject.FindObjectsWithTag<NwPlaceable>("dice_poker"))
        dicePoker.OnUsed += OnUsedDicePoker;

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
    }
    public static void OnUsedStoragePortalIn(PlaceableEvents.OnUsed onUsed)
    {
      if (!Players.TryGetValue(onUsed.UsedBy.ControllingPlayer.LoginCreature, out Player player))
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

        usedSwing.GetLocalVariable<NwObject>("_SWING_TARGET").Value = oPC;

        Task onMovementCancelSwing = NwTask.Run(async () =>
        {
          await NwTask.Delay(TimeSpan.FromSeconds(1));
          await NwTask.WaitUntilValueChanged(() => oPC.Position);

          usedSwing.GetLocalVariable<NwObject>("_SWING_TARGET").Delete();
          usedSwing.GetLocalVariable<int>("_IS_SWINGING").Delete();

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
      NwCreature oPC = (NwCreature)onClick.Placeable.GetLocalVariable<NwObject>("_SWING_TARGET").Value;

      if (swing.GetLocalVariable<int>("_IS_SWINGING").HasValue)
      {
        swing.GetLocalVariable<int>("_IS_SWINGING").Delete();

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
        swing.GetLocalVariable<int>("_IS_SWINGING").Value = 1;

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
          if(swing.GetLocalVariable<int>("_IS_SWINGING").HasValue)
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

        if (swing.GetLocalVariable<int>("_IS_SWINGING").HasValue)
          HandleSwing(swing, oPC);
      });
    }
    public static void HandleCleanDMPLC(PlaceableEvents.OnDeath onDeath)
    {
      NwPlaceable plc = onDeath.KilledObject;
      int plcID = plc.GetLocalVariable<int>("_ID").Value;
      if (plcID > 0)
      {
        SqLiteUtils.DeletionQuery("dm_persistant_placeable",
          new Dictionary<string, string>() { { "rowid", plcID.ToString() } });
      }
      else
        Utils.LogMessageToDMs($"Persistent placeable {plc.Name} in area {plc.Area.Name} does not have a valid ID !");
    }
    public static void HandlePlaceableUsed(PlaceableEvents.OnUsed onUsed)
    {
      Log.Info($"{onUsed.UsedBy.Name} used {onUsed.Placeable.Tag}");
      if (!Players.TryGetValue(onUsed.UsedBy.ControllingPlayer.ControlledCreature, out Player player))
        return;

      switch (onUsed.Placeable.Tag)
      {
        case "respawn_neutral":
          Respawn(player, "neutral");
          break;
        case "respawn_radiant":
          Respawn(player, "radiant");
          break;
        case "respawn_dire":
          Respawn(player, "radiant");
          break;
        case "theater_rope":
          int visibilty;
          if (onUsed.UsedBy.Area.GetLocalVariable<int>("_THEATER_CURTAIN_OPEN").HasNothing)
          {
            visibilty = VisibilityPlugin.NWNX_VISIBILITY_HIDDEN;
            onUsed.UsedBy.Area.GetLocalVariable<int>("_THEATER_CURTAIN_OPEN").Value = 1;
          }
          else
          {
            visibilty = VisibilityPlugin.NWNX_VISIBILITY_VISIBLE;
            onUsed.UsedBy.Area.GetLocalVariable<int>("_THEATER_CURTAIN_OPEN").Delete();
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
      API.Effect eff = API.Effect.CutsceneGhost();
      eff.SubType = EffectSubType.Supernatural;
      onSpawn.Creature.ApplyEffect(EffectDuration.Permanent, eff);

      eff = API.Effect.VisualEffect(VfxType.DurFreezeAnimation);
      eff.Tag = "_FREEZE_EFFECT";
      eff.SubType = EffectSubType.Supernatural;
      onSpawn.Creature.ApplyEffect(EffectDuration.Permanent, eff);

      if (onSpawn.Creature.Tag != "statue_tiamat")
      {
        eff = API.Effect.VisualEffect((VfxType)927);
        eff.SubType = EffectSubType.Supernatural;
        onSpawn.Creature.ApplyEffect(EffectDuration.Permanent, eff);
      }

      onSpawn.Creature.HighlightColor = ColorConstants.Black;
      onSpawn.Creature.MouseCursor = MouseCursor.Walk;
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

      if (!Players.TryGetValue(oPC, out Player player))
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
      onSpawn.Creature.ApplyEffect(EffectDuration.Instant, API.Effect.Death());
    }
    public static void OnUsedPlayerOwnedShop(PlaceableEvents.OnUsed onUsed)
    {
      if (!Players.TryGetValue(onUsed.UsedBy, out Player player))
        return;

      if (onUsed.Placeable.GetLocalVariable<int>("_OWNER_ID").Value == player.characterId)
      {
        PlayerOwnedShop.DrawMainPage(player, onUsed.Placeable);
      }
      else
      {
        NwStore shop = onUsed.Placeable.GetNearestObjectsByType<NwStore>().FirstOrDefault(s => s.GetLocalVariable<int>("_SHOP_ID").Value == onUsed.Placeable.GetLocalVariable<int>("_SHOP_ID").Value);
        
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
      if (!Players.TryGetValue(onUsed.UsedBy, out Player player))
        return;

      if (onUsed.Placeable.GetLocalVariable<int>("_OWNER_ID").Value == player.characterId)
      {
        PlayerOwnedAuction.DrawMainPage(player, onUsed.Placeable);
      }
      else
      {
        NwStore shop = onUsed.Placeable.GetNearestObjectsByType<NwStore>().FirstOrDefault(s => s.GetLocalVariable<int>("_AUCTION_ID").Value == onUsed.Placeable.GetLocalVariable<int>("_AUCTION_ID").Value);

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
      if (!Players.TryGetValue(onUsed.UsedBy, out Player player))
        return;

      NwStore storage = onUsed.Placeable.GetNearestObjectsByType<NwStore>().FirstOrDefault();

      if(storage == null)
      {
        Utils.LogMessageToDMs($"Entrepôt personnel non initialisé pour : {onUsed.UsedBy.Name}");
        return;
      }

      storage.Tag = "_PLAYER_STORAGE";
      storage.GetLocalVariable<int>("_OWNER_ID").Value = player.characterId;
      storage.OnOpen += StoreSystem.OnOpenPersonnalStorage;
      storage.Open(player.oid);
    }
    public static void OnUsedDicePoker(PlaceableEvents.OnUsed onUsed)
    {
      if (!Players.TryGetValue(onUsed.UsedBy, out Player player))
        return;

      if (onUsed.Placeable.GetLocalVariable<int>("_AVAILABLE_SLOTS").HasNothing)
        onUsed.Placeable.GetLocalVariable<int>("_AVAILABLE_SLOTS").Value = 2;

      int availableSlots = onUsed.Placeable.GetLocalVariable<int>("_AVAILABLE_SLOTS").Value;

      if (availableSlots == 0)
      {
        player.oid.SendServerMessage("Aucune place n'est disponible sur ce plateau de dés !");
        return;
      }
      else if(availableSlots == 1)
      {
        onUsed.Placeable.GetLocalVariable<int>("_AVAILABLE_SLOTS").Value = 0;
        onUsed.Placeable.GetLocalVariable<NwObject>("_PLAYER_TWO").Value = player.oid.ControlledCreature;
      }
      else if (availableSlots == 2)
      {
        new DicePoker.DicePoker(player, onUsed.Placeable);
      }
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
  }
}
