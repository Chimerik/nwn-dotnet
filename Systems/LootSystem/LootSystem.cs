using System;
using System.Collections;
using System.Collections.Generic;
using NWN.Core;
using NWN.Core.NWNX;
using NWN.Systems.Craft;

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

      if (oArea != NWScript.OBJECT_INVALID)
      {
        var query = NWScript.SqlPrepareQueryCampaign(ModuleSystem.database, $"SELECT serializedChest, position, facing from {SQL_TABLE}");

        while (Convert.ToBoolean(NWScript.SqlStep(query)))
          UpdateChestTagToLootsDic(NWScript.SqlGetObject(query, 0, Utils.GetLocationFromDatabase(CHEST_AREA_TAG, NWScript.SqlGetVector(query, 1), NWScript.SqlGetFloat(query, 2))));

        InitializeLootChestFromArray(NWScript.GetObjectByTag("low_blueprints"), Craft.Collect.System.lowBlueprints);
        InitializeLootChestFromArray(NWScript.GetObjectByTag("medium_blueprints"), Craft.Collect.System.mediumBlueprints);

        InitializeLootChestFromFeatArray(NWScript.GetObjectByTag("low_skillbooks"), SkillSystem.lowSkillBooks);
        InitializeLootChestFromFeatArray(NWScript.GetObjectByTag("medium_skillbooks"), SkillSystem.mediumSkillBooks);
      }
      else
        Utils.LogMessageToDMs("Attention - La zone des loots n'est pas initialisée.");
    }
    private static void InitializeLootChestFromArray(uint oChest, int[] array)
    {
      foreach (int baseItemType in array)
      {
        var blueprint = new Blueprint(baseItemType);

        if (!Craft.Collect.System.blueprintDictionnary.ContainsKey(baseItemType))
          Craft.Collect.System.blueprintDictionnary.Add(baseItemType, blueprint);

        uint oBlueprint = NWScript.CreateItemOnObject("blueprintgeneric", oChest, 1, "blueprint");
        NWScript.SetName(oBlueprint, $"Patron : {blueprint.name}");
        NWScript.SetLocalInt(oBlueprint, "_BASE_ITEM_TYPE", baseItemType);
      }
    }
    private static void InitializeLootChestFromFeatArray(uint oChest, Feat[] array)
    {
      foreach (Feat feat in array)
      {
        uint skillBook = NWScript.CreateItemOnObject("skillbookgeneriq", oChest, 10, "skillbook");
        ItemPlugin.SetItemAppearance(skillBook, NWScript.ITEM_APPR_TYPE_SIMPLE_MODEL, 2, Utils.random.Next(0, 50));
        NWScript.SetLocalInt(skillBook, "_SKILL_ID", (int)feat);

        int value;
        if (int.TryParse(NWScript.Get2DAString("feat", "FEAT", (int)feat), out value))
          NWScript.SetName(skillBook, NWScript.GetStringByStrRef(value));

        if (int.TryParse(NWScript.Get2DAString("feat", "DESCRIPTION", (int)feat), out value))
          NWScript.SetDescription(skillBook, NWScript.GetStringByStrRef(value));
      }
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
