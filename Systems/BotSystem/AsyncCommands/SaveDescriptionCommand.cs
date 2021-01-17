using System;
using NWN.Core;
using NWN.Core.NWNX;

namespace NWN.Systems
{
  public static partial class BotSystem
  {
    private static void ExecuteSaveDescriptionCommand(string sayText)
    {
      string text = sayText.Remove(0, sayText.IndexOf("_") + 1);
      if (int.TryParse(text.Substring(0, text.IndexOf("_")), out int pcId))
      {
        string descriptionName = (text.Remove(0, text.IndexOf("_") + 1));
        descriptionName = descriptionName.Remove(descriptionName.IndexOf("_"));
        string description = text.Remove(0, text.IndexOf("_") + 1);
        description = description.Remove(0, description.IndexOf("_") + 1);

        var query = NWScript.SqlPrepareQueryCampaign(ModuleSystem.database, "INSERT INTO playerDescriptions (characterId, descriptionName, description)" +
          "VALUES  (@characterId, @descriptionName, @description)" +
          $"ON CONFLICT(characterId, descriptionName) DO UPDATE SET description = @description");
        NWScript.SqlBindInt(query, "@characterId", pcId);
        NWScript.SqlBindString(query, "@descriptionName", descriptionName);
        NWScript.SqlBindString(query, "@description", description);
        NWScript.SqlStep(query);
      }
    }
  }
}
