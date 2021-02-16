using System;
using System.Collections.Generic;
using NWN.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class LootSystem
  {
    private void UpdateDB(uint oChest)
    {
      if (PlayerSystem.Players.TryGetValue(NWScript.GetLastClosedBy(), out PlayerSystem.Player oPC))
      {
        var query = NWScript.SqlPrepareQueryCampaign(Systems.Config.database, $"INSERT INTO {SQL_TABLE}(chestTag, accountId, serializedChest, position, facing)" +
      " VALUES(@chestTag, @accountId, @serializedChest, @position, @facing)" +
      " ON CONFLICT(chestTag) DO UPDATE SET serializedChest = @serializedChest, position = @position, facing = @facing;");
        NWScript.SqlBindString(query, "@chestTag", NWScript.GetTag(oChest));
        NWScript.SqlBindInt(query, "@accountId", oPC.accountId);
        NWScript.SqlBindObject(query, "@serializedChest", oChest);
        NWScript.SqlBindVector(query, "@position", NWScript.GetPosition(oChest));
        NWScript.SqlBindFloat(query, "@facing", NWScript.GetFacing(oChest));
        NWScript.SqlStep(query);
      }
    }
    private void UpdateChestTagToLootsDic(uint oChest)
    {
      NwPlaceable chest = oChest.ToNwObject<NwPlaceable>();

      if (chest == null) return;

      var loots = new List<NwItem> { };

      foreach (NwItem item in chest.Items)
      {
        loots.Add(item);
      }
      chestTagToLootsDic[chest.Tag] = loots;
    }
    private static void ThrowException(string message)
    {
      throw new ApplicationException($"LootSystem: {message}");
    }
  }
}
