using System;
using System.Collections.Generic;
using System.Numerics;
using NWN.Core;

namespace NWN.Systems
{
  public static partial class LootSystem
  {
    public static Dictionary<string, Func<uint, int>> Register = new Dictionary<string, Func<uint, int>>
        {
            { LOOT_CONTAINER_ON_CLOSE_SCRIPT, HandleContainerClose },
            { ON_LOOT_SCRIPT, HandleLoot },
        };

    public static void InitChestArea()
    {
      var oArea = NWScript.GetObjectByTag(CHEST_AREA_TAG);

      if (oArea == NWScript.OBJECT_INVALID)
      {
        ThrowException($"Invalid CHEST_AREA_TAG={CHEST_AREA_TAG}");
      }

      var query = NWScript.SqlPrepareQueryCampaign(ModuleSystem.database, $"SELECT serializedChest, position, facing from {SQL_TABLE}");

      while(Convert.ToBoolean(NWScript.SqlStep(query)))
        UpdateChestTagToLootsDic(NWScript.SqlGetObject(query, 0, Utils.GetLocationFromDatabase(CHEST_AREA_TAG, NWScript.SqlGetVector(query, 1), NWScript.SqlGetFloat(query, 2))));
    }

    private static int HandleContainerClose(uint oidSelf)
    {
      UpdateChestTagToLootsDic(oidSelf);
      UpdateDB(oidSelf);
      return 0;
    }

    private static int HandleLoot(uint oidSelf)
    {
      var oContainer = oidSelf;
      var oArea = NWScript.GetArea(oContainer);

      var containerTag = NWScript.GetTag(oContainer);
      Lootable.Config lootableConfig;

      if (!lootablesDic.TryGetValue(containerTag, out lootableConfig))
      {
        ThrowException($"Unregistered container tag=\"{containerTag}\"");
      }

      Utils.DestroyInventory(oContainer);
      NWScript.AssignCommand(oArea, () => NWScript.DelayCommand(
          0.1f,
          () => lootableConfig.GenerateLoot(oContainer)
      ));

      return 0;
    }
  }
}
