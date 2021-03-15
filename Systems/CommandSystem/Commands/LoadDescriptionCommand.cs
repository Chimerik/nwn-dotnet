using System;
using NWN.Core;
using System.Numerics;
using NWN.API;

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

        var query = NWScript.SqlPrepareQueryCampaign(Config.database, $"SELECT description from playerDescriptions where characterId = @characterId and descriptionName = @descriptionName");
        NWScript.SqlBindInt(query, "@characterId", player.characterId);
        NWScript.SqlBindString(query, "@descriptionName", descriptionName);
        NWScript.SqlStep(query);

        string description = NWScript.SqlGetString(query, 0);

        if (description.Length == 0)
        {
          player.oid.SendServerMessage($"Aucune description valide du nom {descriptionName.ColorString(Color.WHITE)} n'a été trouvée pour le personnage {ctx.oSender.Name}.", Color.ORANGE);
          return;
        }

        ctx.oSender.Description = description;
        ctx.oSender.SendServerMessage($"La description {descriptionName.ColorString(Color.WHITE)} a bien été appliquée à votre personnage.", Color.BLUE);
      }
    }
  }
}
