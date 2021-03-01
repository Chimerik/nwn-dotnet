using NLog;
using NWN.API;
using NWN.Core;
using NWN.Services;
using NWNX.API.Events;
using NWNX.Services;
using System.Linq;

namespace NWN.Systems
{
  [ServiceBinding(typeof(PlayerDisconnection))]
  class PlayerDisconnection
  {
    private static readonly Logger Log = LogManager.GetCurrentClassLogger();
    public PlayerDisconnection(NWNXEventService nwnxEventService)
    {
      nwnxEventService.Subscribe<ClientEvents.OnClientDisconnectBefore>(OnPlayerDisconnectBefore);
    }
    private void OnPlayerDisconnectBefore(ClientEvents.OnClientDisconnectBefore onPCDisconnect)
    {
      if (onPCDisconnect.Player == null)
        return;

      Log.Info($"{onPCDisconnect.Player.Name} disconnecting.");

      if (PlayerSystem.Players.TryGetValue(onPCDisconnect.Player, out PlayerSystem.Player player))
      {
        onPCDisconnect.Player.GetLocalVariable<int>("_DISCONNECTING").Value = 1;

        if (player.menu.isOpen)
          player.menu.Close();

        Log.Info($"menu closed");

        player.UnloadMenuQuickbar();

        Log.Info($"quickbar unloaded");

        onPCDisconnect.Player.VisualTransform.Rotation.X = 0.0f;
        onPCDisconnect.Player.VisualTransform.Translation.X = 0.0f;
        onPCDisconnect.Player.VisualTransform.Translation.Y = 0.0f;
        onPCDisconnect.Player.VisualTransform.Translation.Z = 0.0f;
        player.setValue = Config.invalidInput;
        player.OnKeydown -= player.menu.HandleMenuFeatUsed;

        RemovePartyBuffOnDisconnect(onPCDisconnect.Player);

        Log.Info($"Party buff removed");

        if (player.oid.Area.Tag == $"entrepotpersonnel_{player.oid.CDKey}")
        {
          var saveStorage = NWScript.SqlPrepareQueryCampaign(Systems.Config.database,
            $"UPDATE playerCharacters set storage = @storage where rowid = @characterId");
          NWScript.SqlBindInt(saveStorage, "@characterId", player.characterId);
          NWScript.SqlBindObject(saveStorage, "@storage", player.oid.Area.FindObjectsOfTypeInArea<NwPlaceable>().Where(c => c.Tag == "ps_entrepot").FirstOrDefault());
          NWScript.SqlStep(saveStorage);
          player.location = NwObject.FindObjectsWithTag<NwPlaceable>("portal_storage_in").FirstOrDefault()?.Location;

          Log.Info($"Saved personnal storage");
        }
      }
    }
    private void RemovePartyBuffOnDisconnect(NwPlayer player)
    {
      Log.Info($"Removing party buff on disconnection for {player.Name}");

      API.Effect eParty = Party.GetPartySizeEffect(player.PartyMembers.Count<NwPlayer>() - 1);

      foreach (NwPlayer partyMember in player.PartyMembers.Where<NwPlayer>(p => !p.IsPlayerDM))
      {
        API.Effect eff = partyMember.ActiveEffects.Where(e => e.Tag == "PartyEffect").FirstOrDefault();
        if (eff != null)
        {
          player.RemoveEffect(eff);
          Log.Info($"Removing party buff {eff.EffectType.ToString()} on disconnection for {partyMember.Name}");

          if (player != partyMember)
            partyMember.ApplyEffect(EffectDuration.Permanent, eParty);
        }
      }
    }
  }
}
