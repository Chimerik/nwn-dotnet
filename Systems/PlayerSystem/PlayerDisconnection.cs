using NWN.API;
using NWN.API.Events;
using NWN.Core;
using System.Linq;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    private void HandlePlayerLeave(OnClientDisconnect onPCDisconnect)
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

        if (player.oid.Area != null && player.oid.Area.Tag == $"entrepotpersonnel_{player.oid.CDKey}")
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
  }
}
