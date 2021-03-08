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

namespace NWN.Systems
{
  [ServiceBinding(typeof(PlaceableSystem))]
  public class PlaceableSystem
  {
    public static readonly Logger Log = LogManager.GetCurrentClassLogger();
    public PlaceableSystem()
    {
      foreach (NwDoor door in NwModule.FindObjectsOfType<NwDoor>())
        door.OnOpen += HandleDoorAutoClose;

      foreach (NwPlaceable bassin in NwModule.FindObjectsWithTag<NwPlaceable>("ench_bsn"))
        bassin.OnClose += HandleCloseEnchantementBassin;

      foreach (NwPlaceable plc in NwModule.FindObjectsWithTag<NwPlaceable>(Arena.Config.PVE_ARENA_PULL_ROPE_CHAIN_TAG, "portal_storage_out", "portal_storage_in", "portal_start", "respawn_neutral", "respawn_dire", "respawn_radiant", "theater_rope"))
        plc.OnUsed += HandlePlaceableUsed;

      foreach (NwCreature statue in NwModule.FindObjectsWithTag<NwCreature>("Statuereptilienne", "Statuereptilienne2", "statue_tiamat"))
      {
        statue.OnConversation += HandleCancelStatueConversation;
        statue.OnPerception += HandleStatufyCreature;
      }

      foreach (NwCreature corpse in NwModule.FindObjectsWithTag<NwCreature>("dead_wererat"))
      {
        corpse.OnConversation += HandleCancelStatueConversation;
        corpse.OnSpawn += HandleSetUpDeadCreatureCorpse;
      }
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
        NWN.Utils.LogMessageToDMs($"Persistent placeable {plc.Name} in area {plc.Area.Name} does not have a valid ID !");
    }
    public static void HandlePlaceableUsed(PlaceableEvents.OnUsed onUsed)
    {
      Log.Info($"{onUsed.UsedBy.Name} used {onUsed.Placeable.Tag}");
      if (PlayerSystem.Players.TryGetValue(onUsed.UsedBy, out PlayerSystem.Player player))
        switch (onUsed.Placeable.Tag)
        {
          case "respawn_neutral":
            PlayerSystem.Respawn(player, "neutral");
            break;
          case "respawn_radiant":
            PlayerSystem.Respawn(player, "radiant");
            break;
          case "respawn_dire":
            PlayerSystem.Respawn(player, "radiant");
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
            player.oid.Location = NwModule.FindObjectsWithTag<NwWaypoint>("WP_START_NEW_CHAR").FirstOrDefault().Location;
            break;
          case "portal_storage_in":
            string uniqueTag = $"entrepotpersonnel_{player.oid.CDKey}";
            string name = $"Entrepot dimensionnel de {player.oid.Name}";

            NwArea area = NwArea.Create("entrepotperso", uniqueTag, name);

            if (area != null)
            {
              NwWaypoint waypoint = area.FindObjectsOfTypeInArea<NwWaypoint>().FirstOrDefault(w => w.Tag == "wp_inentrepot");

              if (waypoint != null)
              {
                NWScript.AssignCommand(player.oid, () => NWScript.JumpToLocation(waypoint.Location));
                //player.oid.Location = waypoint.Location;
              }
              else
              {
                Log.Warn("Waypoint is null");
              }

              Task spawnResources = NwTask.Run(async () =>
              {
                await NwTask.WaitUntil(() => player.oid.Area != null);

                NwPlaceable placeable = area.FindObjectsOfTypeInArea<NwPlaceable>().FirstOrDefault(p => p.Tag == "portal_storage_out");

                if (placeable != null)
                {
                  area.OnExit += AreaSystem.OnAreaExit;
                }
                else
                {
                  Log.Warn("Placeable is null");
                }

                area.GetLocalVariable<int>("_AREA_LEVEL").Value = 0;

                NwPlaceable storage = area.FindObjectsOfTypeInArea<NwPlaceable>().FirstOrDefault(s => s.Tag == "ps_entrepot");

                var query = NWScript.SqlPrepareQueryCampaign(Config.database, $"SELECT storage from playerCharacters where rowid = @characterId");
                NWScript.SqlBindInt(query, "@characterId", player.characterId);
                NWScript.SqlStep(query);

                NWScript.SqlGetObject(query, 0, NWScript.GetLocation(storage));
                storage.Destroy();

                NwPlaceable portalOut = area.FindObjectsOfTypeInArea<NwPlaceable>().FirstOrDefault(p => p.Tag == "portal_storage_out");

                if (portalOut != null)
                {
                  portalOut.OnUsed += HandlePlaceableUsed;
                }
                else
                {
                  Log.Warn("portalOut is null");
                }

                NwPlaceable auctionHouse = area.FindObjectsOfTypeInArea<NwPlaceable>().FirstOrDefault(p => p.Tag == "hventes");
                auctionHouse.OnUsed += DialogSystem.StartAuctionHouseDialog;

                NwCreature messenger = area.FindObjectsOfTypeInArea<NwCreature>().FirstOrDefault(p => p.Tag == "bal_system");
                messenger.OnConversation += DialogSystem.StartStorageDialog;
              });
            }
            else
            {
              Log.Warn($"Could not create {name}");
            }

            break;
          case "portal_storage_out":
            player.oid.Location = NwModule.FindObjectsWithTag<NwWaypoint>("wp_outentrepot").FirstOrDefault().Location;
            break;
          case Arena.Config.PVE_ARENA_PULL_ROPE_CHAIN_TAG:
            Arena.ScriptHandlers.HandlePullRopeChainUse();
            break;
        }
    }
    private void HandleCancelStatueConversation(CreatureEvents.OnConversation onConversation)
    {

    }
    private async void HandleStatufyCreature(CreatureEvents.OnPerception onPerception)
    {
      if (onPerception.PerceivedCreature is NwPlayer)
      {
        if (onPerception.Creature.Tag != "statue_tiamat")
        {
          await onPerception.Creature.PlayAnimation((Animation)NWN.Utils.random.Next(100, 116), 3);
          await NwTask.Delay(TimeSpan.FromSeconds(0.5));
          FreezeCreature(onPerception.Creature);
        }
        else
          FreezeCreature(onPerception.Creature);

        onPerception.Creature.AiLevel = AiLevel.VeryLow;
        onPerception.Creature.OnPerception -= HandleStatufyCreature;
      }
    }
    private static void FreezeCreature(NwCreature creature)
    {
      API.Effect eff = API.Effect.VisualEffect(VfxType.DurFreezeAnimation);
      eff.SubType = EffectSubType.Supernatural;
      creature.ApplyEffect(EffectDuration.Permanent, eff);

      if (creature.Tag != "statue_tiamat")
      {
        eff = API.Effect.VisualEffect(VfxType.DurProtGreaterStoneskin);
        eff.SubType = EffectSubType.Supernatural;
        creature.ApplyEffect(EffectDuration.Permanent, eff);
      }

      creature.HiliteColor = Color.BLACK;
      NWScript.SetObjectMouseCursor(creature, NWScript.MOUSECURSOR_WALK);
      creature.PlotFlag = true;
    }
    private async void HandleDoorAutoClose(DoorEvents.OnOpen onOpen)
    {
      await NwTask.Delay(TimeSpan.FromSeconds(5));
      await onOpen.Door.PlayAnimation(Animation.DoorClose, 1);
    }
    public void HandleCloseEnchantementBassin(PlaceableEvents.OnClose onClose)
    {
      NwCreature oPC = onClose.LastClosedBy;

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
      NwItem.Create("undroppable_item", onSpawn.Creature).Droppable = true;
      onSpawn.Creature.Lootable = true;
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
          player.oid.SendServerMessage("Cette boutique n'est pas accessible pour le moment.", Color.ORANGE);
          return;
        }

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
          player.oid.SendServerMessage("Cette enchère n'est pas accessible pour le moment.", Color.ORANGE);
          return;
        }

        shop.Open(player.oid);
        PlayerOwnedAuction.GetAuctionPrice(player, shop, onUsed.Placeable);
      }
    }
  }
}
