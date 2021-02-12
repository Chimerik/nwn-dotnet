using System;
using NWN.API;
using NWN.API.Events;
using NWN.Core;
using NWN.Core.NWNX;
using NWN.Services;
using NWN.API.Constants;
using System.Linq;

namespace NWN.Systems
{
  [ServiceBinding(typeof(PlaceableSystem))]
  public class PlaceableSystem
  {
        public static NativeEventService nativeEventService;
        public PlaceableSystem(NativeEventService eventService)
        {
            nativeEventService = eventService;

            foreach (NwDoor door in NwModule.FindObjectsOfType<NwDoor>())
                eventService.Subscribe<NwDoor, DoorEvents.OnOpen>(door, HandleDoorAutoClose);

            foreach (NwPlaceable bassin in NwModule.FindObjectsWithTag<NwPlaceable>("ench_bsn"))
                eventService.Subscribe<NwPlaceable, PlaceableEvents.OnClose>(bassin, HandleCloseEnchantementBassin);

            foreach (NwPlaceable plc in NwModule.FindObjectsWithTag<NwPlaceable>(Arena.Config.PVE_ARENA_PULL_ROPE_CHAIN_TAG, "portal_storage_out", "portal_storage_in", "portal_start", "respawn_neutral", "respawn_dire", "respawn_radiant", "theater_rope"))
                eventService.Subscribe<NwPlaceable, PlaceableEvents.OnUsed>(plc, HandlePlaceableUsed);

            foreach (NwCreature statue in NwModule.FindObjectsWithTag<NwCreature>("Statuereptilienne", "statue_tiamat"))
            {
                eventService.Subscribe<NwCreature, CreatureEvents.OnConversation>(statue, HandleCancelStatueConversation);
                eventService.Subscribe<NwCreature, CreatureEvents.OnPerception>(statue, HandleStatufyCreature);
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
            NwArea area = NwArea.Create("entrepotperso", $"entrepotpersonnel_{NWScript.GetPCPublicCDKey(player.oid)}", $"Entrepot dimensionnel de {NWScript.GetName(player.oid)}");
            NwPlaceable storage = area.FindObjectsOfTypeInArea<NwPlaceable>().Where(s => s.Tag == "ps_entrepot").FirstOrDefault();

            var query = NWScript.SqlPrepareQueryCampaign(Config.database, $"SELECT storage from playerCharacters where rowid = @characterId");
            NWScript.SqlBindInt(query, "@characterId", player.characterId);
            NWScript.SqlStep(query);

            NWScript.SqlGetObject(query, 0, NWScript.GetLocation(storage));
            storage.Destroy();

            player.oid.Location = area.FindObjectsOfTypeInArea<NwWaypoint>().Where(w => w.Tag == "wp_inentrepot").FirstOrDefault().Location;
            nativeEventService.Subscribe<NwPlaceable, PlaceableEvents.OnUsed>(area.FindObjectsOfTypeInArea<NwPlaceable>().Where(p => p.Tag == "portal_storage_out").FirstOrDefault(), HandlePlaceableUsed);
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
          await NwTask.Delay(TimeSpan.FromSeconds(0.3));
          FreezeCreature(onPerception.Creature);
        }
        else
          FreezeCreature(onPerception.Creature);

        onPerception.Creature.AiLevel = AiLevel.VeryLow;
        nativeEventService.Unsubscribe<NwCreature, CreatureEvents.OnPerception>(onPerception.Creature, HandleStatufyCreature);
      }
    }
    private static void FreezeCreature(NwCreature creature)
    {
      API.Effect eff = API.Effect.VisualEffect(VfxType.DurFreezeAnimation);
      eff.SubType = EffectSubType.Supernatural;
      creature.ApplyEffect(EffectDuration.Permanent, eff);

      eff = API.Effect.VisualEffect(VfxType.DurIceskin);
      eff.SubType = EffectSubType.Supernatural;
      creature.ApplyEffect(EffectDuration.Permanent, eff);
      creature.HiliteColor = Color.WHITE;
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
    }
}
