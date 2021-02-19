using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NLog;
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
    public static readonly Logger Log = LogManager.GetCurrentClassLogger();
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
      if (area == null)
      {
        Task waitForDiscordBot = NwTask.Run(async () =>
        {
          await NwTask.Delay(TimeSpan.FromSeconds(5));
          Utils.LogMessageToDMs("Attention - La zone des loots n'est pas initialisée.");
        });
        
        return;
      }

      var query = NWScript.SqlPrepareQueryCampaign(Config.database, $"SELECT serializedChest, position, facing from {SQL_TABLE}");

      while (Convert.ToBoolean(NWScript.SqlStep(query)))
      {
        NwPlaceable oChest = NWScript.SqlGetObject(query, 0, Utils.GetLocationFromDatabase(CHEST_AREA_TAG, NWScript.SqlGetVector(query, 1), NWScript.SqlGetFloat(query, 2))).ToNwObject<NwPlaceable>();

        if (oChest == null)
        {
          Task waitForDiscordBot = NwTask.Run(async () =>
          {
            await NwTask.Delay(TimeSpan.FromSeconds(5));
            Utils.LogMessageToDMs("Attention - Un coffre initialisé de la base de données est invalide !");
          });
          
          continue;
        }

        UpdateChestTagToLootsDic(oChest);
        nativeEventService.Subscribe<NwPlaceable, PlaceableEvents.OnClose>(oChest, OnLootConfigContainerClose);
      }

      InitializeLootChestFromArray(NwModule.FindObjectsWithTag<NwPlaceable>("low_blueprints").FirstOrDefault(), Craft.Collect.System.lowBlueprints);
      InitializeLootChestFromArray(NwModule.FindObjectsWithTag<NwPlaceable>("medium_blueprints").FirstOrDefault(), Craft.Collect.System.mediumBlueprints);

      InitializeLootChestFromFeatArray(NwModule.FindObjectsWithTag<NwPlaceable>("low_skillbooks").FirstOrDefault(), SkillSystem.lowSkillBooks);
      InitializeLootChestFromFeatArray(NwModule.FindObjectsWithTag<NwPlaceable>("medium_skillbooks").FirstOrDefault(), SkillSystem.mediumSkillBooks);
    }

    private void InitializeLootChestFromArray(NwPlaceable oChest, int[] array)
    {
      foreach (int baseItemType in array)
      {
        var blueprint = new Blueprint(baseItemType);

        if (!Craft.Collect.System.blueprintDictionnary.ContainsKey(baseItemType))
          Craft.Collect.System.blueprintDictionnary.Add(baseItemType, blueprint);

        NwItem oBlueprint = NwItem.Create("blueprintgeneric", oChest, 1, "blueprint");
        oBlueprint.Name = $"Patron : {blueprint.name}";
        oBlueprint.GetLocalVariable<int>("_BASE_ITEM_TYPE").Value = baseItemType;
      }

      UpdateChestTagToLootsDic(oChest);
    }
    private void InitializeLootChestFromFeatArray(NwPlaceable oChest, Feat[] array)
    {
      foreach (Feat feat in array)
      {
        NwItem skillBook = NwItem.Create("skillbookgeneriq", oChest, 10, "skillbook");
        ItemPlugin.SetItemAppearance(skillBook, NWScript.ITEM_APPR_TYPE_SIMPLE_MODEL, 2, NWN.Utils.random.Next(0, 50));
        skillBook.GetLocalVariable<int>("_SKILL_ID").Value = (int)feat;

        int value;
        if (int.TryParse(NWScript.Get2DAString("feat", "FEAT", (int)feat), out value))
          skillBook.Name = NWScript.GetStringByStrRef(value);

        if (int.TryParse(NWScript.Get2DAString("feat", "DESCRIPTION", (int)feat), out value))
          skillBook.Description = NWScript.GetStringByStrRef(value);
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
      NwCreature oContainer = onDeath.KilledCreature;

      if (lootablesDic.TryGetValue(oContainer.Tag, out Lootable.Config lootableConfig))
      {
        Utils.DestroyInventory(oContainer);
        lootableConfig.GenerateLoot(oContainer);
      }
      else
        ThrowException($"Unregistered container tag=\"{oContainer.Tag}\"");
    }
  }
}
