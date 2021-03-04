using NLog;
using NWN.API;
using NWN.API.Events;
using NWN.Core;
using NWN.Services;
using NWNX.API.Events;
using NWNX.Services;
using System.Linq;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    private void HandlePlayerLeave(ModuleEvents.OnClientLeave onPCDisconnect)
    {
      if (onPCDisconnect.Player == null)
        return;

      Log.Info($"{onPCDisconnect.Player.Name} disconnecting.");

      if (Players.TryGetValue(onPCDisconnect.Player, out Player player))
      {
        onPCDisconnect.Player.GetLocalVariable<int>("_DISCONNECTING").Value = 1;

        if (player.menu.isOpen)
          player.menu.Close();

        player.UnloadMenuQuickbar();

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
          var saveStorage = NWScript.SqlPrepareQueryCampaign(Config.database,
            $"UPDATE playerCharacters set storage = @storage where rowid = @characterId");
          NWScript.SqlBindInt(saveStorage, "@characterId", player.characterId);
          NWScript.SqlBindObject(saveStorage, "@storage", player.oid.Area.FindObjectsOfTypeInArea<NwPlaceable>().Where(c => c.Tag == "ps_entrepot").FirstOrDefault());
          NWScript.SqlStep(saveStorage);

          player.location = NwModule.FindObjectsWithTag<NwWaypoint>("wp_outentrepot").FirstOrDefault().Location;

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
