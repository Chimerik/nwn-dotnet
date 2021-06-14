using System;
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

          NWScript.SetObjectVisualTransform(usedSwing, NWScript.OBJECT_VISUAL_TRANSFORM_TRANSLATE_X, 0, NWScript.OBJECT_VISUAL_TRANSFORM_LERP_SMOOTHERSTEP, 2);
          NWScript.SetObjectVisualTransform(usedSwing, NWScript.OBJECT_VISUAL_TRANSFORM_ROTATE_Z, 0, NWScript.OBJECT_VISUAL_TRANSFORM_LERP_SMOOTHERSTEP, 2);
          NWScript.SetObjectVisualTransform(oPC, NWScript.OBJECT_VISUAL_TRANSFORM_TRANSLATE_X, 0, NWScript.OBJECT_VISUAL_TRANSFORM_LERP_SMOOTHERSTEP, 2);
          NWScript.SetObjectVisualTransform(oPC, NWScript.OBJECT_VISUAL_TRANSFORM_ROTATE_Z, 0, NWScript.OBJECT_VISUAL_TRANSFORM_LERP_SMOOTHERSTEP, 2);

          usedSwing.OnUsed += OnUsedBalancoire;
          usedSwing.OnLeftClick -= OnClickSwingBalancoire;
        });
      });
    }
    public static void OnClickSwingBalancoire(PlaceableEvents.OnLeftClick onClick)
    {
      NwPlaceable swing = onClick.Placeable;
      NwObject oPC = onClick.Placeable.GetLocalVariable<NwObject>("_SWING_TARGET").Value;

      if (swing.GetLocalVariable<int>("_IS_SWINGING").HasValue)
      {
        swing.GetLocalVariable<int>("_IS_SWINGING").Delete();

        Task waitSwingEnd = NwTask.Run(async () =>
        {
          await NwTask.Delay(TimeSpan.FromSeconds(2));
          NWScript.SetObjectVisualTransform(swing, NWScript.OBJECT_VISUAL_TRANSFORM_TRANSLATE_X, 0, NWScript.OBJECT_VISUAL_TRANSFORM_LERP_SMOOTHERSTEP, 1);
          NWScript.SetObjectVisualTransform(swing, NWScript.OBJECT_VISUAL_TRANSFORM_ROTATE_Z, 0, NWScript.OBJECT_VISUAL_TRANSFORM_LERP_SMOOTHERSTEP, 1);
          NWScript.SetObjectVisualTransform(oPC, NWScript.OBJECT_VISUAL_TRANSFORM_TRANSLATE_X, 0, NWScript.OBJECT_VISUAL_TRANSFORM_LERP_SMOOTHERSTEP, 1);
          NWScript.SetObjectVisualTransform(oPC, NWScript.OBJECT_VISUAL_TRANSFORM_ROTATE_Z, 0, NWScript.OBJECT_VISUAL_TRANSFORM_LERP_SMOOTHERSTEP, 1);
        });
      }
      else
      {
        swing.GetLocalVariable<int>("_IS_SWINGING").Value = 1;

        NWScript.SetObjectVisualTransform(oPC, NWScript.OBJECT_VISUAL_TRANSFORM_TRANSLATE_X, -0.75f, NWScript.OBJECT_VISUAL_TRANSFORM_LERP_SMOOTHERSTEP, 2);
        NWScript.SetObjectVisualTransform(oPC, NWScript.OBJECT_VISUAL_TRANSFORM_ROTATE_Z, 15, NWScript.OBJECT_VISUAL_TRANSFORM_LERP_SMOOTHERSTEP, 2);
        NWScript.SetObjectVisualTransform(swing, NWScript.OBJECT_VISUAL_TRANSFORM_TRANSLATE_X, 0.75f, NWScript.OBJECT_VISUAL_TRANSFORM_LERP_SMOOTHERSTEP, 2);
        NWScript.SetObjectVisualTransform(swing, NWScript.OBJECT_VISUAL_TRANSFORM_ROTATE_Z, -15, NWScript.OBJECT_VISUAL_TRANSFORM_LERP_SMOOTHERSTEP, 2);

        Task waitLoopEnd = NwTask.Run(async () =>
        {
          await NwTask.Delay(TimeSpan.FromSeconds(2));
          if(swing.GetLocalVariable<int>("_IS_SWINGING").HasValue)
            HandleSwing(swing, oPC);
        });
      }
    }
    public static void HandleSwing(NwObject swing, NwObject oPC)
    {
      NWScript.SetObjectVisualTransform(oPC, NWScript.OBJECT_VISUAL_TRANSFORM_TRANSLATE_X, -NWScript.GetObjectVisualTransform(oPC, NWScript.OBJECT_VISUAL_TRANSFORM_TRANSLATE_X), NWScript.OBJECT_VISUAL_TRANSFORM_LERP_SMOOTHERSTEP, 2);
      NWScript.SetObjectVisualTransform(oPC, NWScript.OBJECT_VISUAL_TRANSFORM_ROTATE_Z, -NWScript.GetObjectVisualTransform(oPC, NWScript.OBJECT_VISUAL_TRANSFORM_ROTATE_Z), NWScript.OBJECT_VISUAL_TRANSFORM_LERP_SMOOTHERSTEP, 2);
      NWScript.SetObjectVisualTransform(swing, NWScript.OBJECT_VISUAL_TRANSFORM_TRANSLATE_X, -NWScript.GetObjectVisualTransform(swing, NWScript.OBJECT_VISUAL_TRANSFORM_TRANSLATE_X), NWScript.OBJECT_VISUAL_TRANSFORM_LERP_SMOOTHERSTEP, 2);
      NWScript.SetObjectVisualTransform(swing, NWScript.OBJECT_VISUAL_TRANSFORM_ROTATE_Z, -NWScript.GetObjectVisualTransform(swing, NWScript.OBJECT_VISUAL_TRANSFORM_ROTATE_Z), NWScript.OBJECT_VISUAL_TRANSFORM_LERP_SMOOTHERSTEP, 2);

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
        var query = NWScript.SqlPrepareQueryCampaign(Config.database, "DELETE FROM dm_persistant_placeable where rowid = @plcID");
        NWScript.SqlBindInt(query, "@rowid", plcID);
        NWScript.SqlStep(query);
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

      onSpawn.Creature.HiliteColor = ColorConstants.Black;
      NWScript.SetObjectMouseCursor(onSpawn.Creature, NWScript.MOUSECURSOR_WALK);
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
        NWScript.SendMessageToPC(oPC, "Player is not valid.");

      var oItem = NWScript.GetFirstItemInInventory(oPC);

      if (oItem == NWScript.OBJECT_INVALID)
        NWScript.SendMessageToPC(oPC, "Item is not valid.");


      if (!ItemUtils.IsEquipable(oItem))
        NWScript.SendMessageToPC(oPC, "Item is not equipable.");

      if (NWScript.GetPlotFlag(oItem) == 1)
        NWScript.SendMessageToPC(oPC, "Cannot enchant a plot item.");


      var oSecondItem = NWScript.GetNextItemInInventory(onClose.Placeable);
      if (oSecondItem != NWScript.OBJECT_INVALID)
        NWScript.SendMessageToPC(oPC, "Invalid number of items.");

      var tag = NWScript.GetTag(oItem);
      EnchantmentBasinSystem.GetEnchantmentBasinFromTag(tag).DrawMenu(player, oItem, onClose.Placeable);
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
