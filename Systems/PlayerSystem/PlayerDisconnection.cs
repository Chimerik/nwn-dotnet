using NWN.API;
using NWN.Core;
using NWN.Services;
using NWNX.API.Events;
using NWNX.Services;
using System;
using System.Linq;

namespace NWN.Systems
{
  [ServiceBinding(typeof(PlayerDisconnection))]
  class PlayerDisconnection
  {
    public PlayerDisconnection(NWNXEventService nwnxEventService)
    {
      nwnxEventService.Subscribe<ClientEvents.OnClientDisconnectBefore>(OnPlayerDisconnectBefore);
    }
    private void OnPlayerDisconnectBefore(ClientEvents.OnClientDisconnectBefore onPCDisconnect)
    {
      if (onPCDisconnect.Player == null)
        return;

      NWScript.WriteTimestampedLogEntry($"{onPCDisconnect.Player.Name} disconnecting.");

      if (PlayerSystem.Players.TryGetValue(onPCDisconnect.Player, out PlayerSystem.Player player))
      {
        player.isConnected = false;

        if(player.menu.isOpen)
          player.menu.Close();

        NWScript.WriteTimestampedLogEntry($"menu closed");

        player.UnloadMenuQuickbar();

        NWScript.WriteTimestampedLogEntry($"quickbar unloaded");

        onPCDisconnect.Player.VisualTransform.Rotation.X = 0.0f;
        onPCDisconnect.Player.VisualTransform.Translation.X = 0.0f;
        onPCDisconnect.Player.VisualTransform.Translation.Y = 0.0f;
        onPCDisconnect.Player.VisualTransform.Translation.Z = 0.0f;
        player.setValue = 0;
        player.OnKeydown -= player.menu.HandleMenuFeatUsed;

        RemovePartyBuffOnDisconnect(onPCDisconnect.Player);

        NWScript.WriteTimestampedLogEntry($"Party buff removed");

        if (player.oid.Area.Tag == $"entrepotpersonnel_{player.oid.CDKey}")
        {
          var saveStorage = NWScript.SqlPrepareQueryCampaign(Systems.Config.database,
            $"UPDATE playerCharacters set storage = @storage where rowid = @characterId");
          NWScript.SqlBindInt(saveStorage, "@characterId", player.characterId);
          NWScript.SqlBindObject(saveStorage, "@storage", player.oid.Area.FindObjectsOfTypeInArea<NwPlaceable>().Where(c => c.Tag == "ps_entrepot").FirstOrDefault());
          NWScript.SqlStep(saveStorage);
          player.location = NwObject.FindObjectsWithTag<NwPlaceable>("portal_storage_in").FirstOrDefault()?.Location;

          NWScript.WriteTimestampedLogEntry($"Saved personnal storage");
        }
      }
    }
    private void RemovePartyBuffOnDisconnect(NwPlayer player)
    {
      API.Effect eParty = Party.GetPartySizeEffect(player.PartyMembers.Count<NwPlayer>() - 1);

      foreach (NwPlayer partyMember in player.PartyMembers.Where<NwPlayer>(p => !p.IsPlayerDM))
      {
        API.Effect eff = partyMember.ActiveEffects.Where(e => e.Tag == "PartyEffect").FirstOrDefault();
        if (eff != null)
        {
          player.RemoveEffect(eff);

          if (player != partyMember)
            partyMember.ApplyEffect(EffectDuration.Permanent, eParty);
        }
      }
    }
  }
}
