using System;
using System.Collections.Generic;
using System.Linq;
using NWN.API;
using NWN.API.Events;
using NWN.Core;
using NWN.Core.NWNX;
using NWN.Services;
using NWN.Systems.Craft;

namespace NWN.Systems
{
  [ServiceBinding(typeof(LootSystem))]
  public partial class LootSystem
  {
    private readonly NativeEventService nativeEventService;
    private static Dictionary<string, List<NwItem>> chestTagToLootsDic = new Dictionary<string, List<NwItem>> { };
    public LootSystem(NativeEventService eventService)
    {
      this.nativeEventService = eventService;
      eventService.Subscribe<NwModule, ModuleEvents.OnModuleLoad>(NwModule.Instance, OnModuleLoad);
    }
    private void OnModuleLoad(ModuleEvents.OnModuleLoad onModuleLoad)
    {
      InitChestArea();
    }
    private void InitChestArea()
    {
      NwArea area = NwModule.Instance.Areas.Where(a => a.Tag == CHEST_AREA_TAG).FirstOrDefault();
      if (!area.IsValid)
      {
        NWN.Utils.LogMessageToDMs("Attention - La zone des loots n'est pas initialisée.");
        return;
      }

      var query = NWScript.SqlPrepareQueryCampaign(Config.database, $"SELECT serializedChest, position, facing from {SQL_TABLE}");

      while (Convert.ToBoolean(NWScript.SqlStep(query)))
      {
        uint oChest = NWScript.SqlGetObject(query, 0, NWN.Utils.GetLocationFromDatabase(CHEST_AREA_TAG, NWScript.SqlGetVector(query, 1), NWScript.SqlGetFloat(query, 2)));
        if (!Convert.ToBoolean(NWScript.GetIsObjectValid(oChest)))
          continue;
        UpdateChestTagToLootsDic(oChest);
        nativeEventService.Subscribe<NwPlaceable, PlaceableEvents.OnClose>(oChest.ToNwObject<NwPlaceable>(), OnLootConfigContainerClose);
      }

      InitializeLootChestFromArray(NWScript.GetObjectByTag("low_blueprints"), Craft.Collect.System.lowBlueprints);
      InitializeLootChestFromArray(NWScript.GetObjectByTag("medium_blueprints"), Craft.Collect.System.mediumBlueprints);

      InitializeLootChestFromFeatArray(NWScript.GetObjectByTag("low_skillbooks"), SkillSystem.lowSkillBooks);
      InitializeLootChestFromFeatArray(NWScript.GetObjectByTag("medium_skillbooks"), SkillSystem.mediumSkillBooks);
    }

    private void InitializeLootChestFromArray(uint oChest, int[] array)
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

      UpdateChestTagToLootsDic(oChest);
    }
    private void InitializeLootChestFromFeatArray(uint oChest, Feat[] array)
    {
      foreach (Feat feat in array)
      {
        uint skillBook = NWScript.CreateItemOnObject("skillbookgeneriq", oChest, 10, "skillbook");
        ItemPlugin.SetItemAppearance(skillBook, NWScript.ITEM_APPR_TYPE_SIMPLE_MODEL, 2, NWN.Utils.random.Next(0, 50));
        NWScript.SetLocalInt(skillBook, "_SKILL_ID", (int)feat);

        int value;
        if (int.TryParse(NWScript.Get2DAString("feat", "FEAT", (int)feat), out value))
          NWScript.SetName(skillBook, NWScript.GetStringByStrRef(value));

        if (int.TryParse(NWScript.Get2DAString("feat", "DESCRIPTION", (int)feat), out value))
          NWScript.SetDescription(skillBook, NWScript.GetStringByStrRef(value));
      }

      UpdateChestTagToLootsDic(oChest);
    }
    private void OnLootConfigContainerClose(PlaceableEvents.OnClose onClose)
    {
      UpdateChestTagToLootsDic(onClose.Placeable);
      UpdateDB(onClose.Placeable);
    }
    public static void HandleLoot(CreatureEvents.OnDeath onDeath)
    {
      var oContainer = onDeath.KilledCreature;

      if (lootablesDic.TryGetValue(oContainer.Tag, out Lootable.Config lootableConfig))
      {
        NWN.Utils.DestroyInventory(oContainer);
        NWScript.AssignCommand(oContainer.Area, () => NWScript.DelayCommand(
            0.1f,
            () => lootableConfig.GenerateLoot(oContainer)
        ));
      }
      else
        ThrowException($"Unregistered container tag=\"{oContainer.Tag}\"");
    }
  }
}
