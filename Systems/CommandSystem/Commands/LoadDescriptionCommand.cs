using System;
using NWN.Core;
using System.Numerics;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private static void ExecuteLoadDescriptionCommand(ChatSystem.Context ctx, Options.Result options)
    {
      if (PlayerSystem.Players.TryGetValue(ctx.oSender, out PlayerSystem.Player player))
      {
        string descriptionName = (string)options.positional[0];

        if (descriptionName.Length == 0)
          return;

        var query = NWScript.SqlPrepareQueryCampaign(ModuleSystem.database, $"SELECT description from playerDescriptions where characterId = @characterId and descriptionName = @descriptionName");
        NWScript.SqlBindInt(query, "@characterId", player.characterId);
        NWScript.SqlBindString(query, "@descriptionName", descriptionName);
        NWScript.SqlStep(query);

        string description = NWScript.SqlGetString(query, 0);

        if (description.Length == 0)
        {
          NWScript.SendMessageToPC(player.oid, $"Aucune description valide du nom {descriptionName} n'a été trouvée pour le personnage {NWScript.GetName(player.oid)}.");
          return;
        }

        NWScript.SetDescription(player.oid, description);

        NWScript.SendMessageToPC(player.oid, $"La description {descriptionName} a bien été appliquée à votre personnage.");
      }
    }
  }
}
