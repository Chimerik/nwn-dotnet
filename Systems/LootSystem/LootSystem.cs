using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using NLog;
using NWN.API;
using NWN.API.Constants;
using NWN.API.Events;
using NWN.Core;
using NWN.Core.NWNX;
using NWN.Services;
using NWN.Systems.Craft;
using NWNX.API;

namespace NWN.Systems
{
  [ServiceBinding(typeof(LootSystem))]
  public partial class LootSystem
  {
    public static readonly Logger Log = LogManager.GetCurrentClassLogger();
    private static Dictionary<string, List<NwItem>> chestTagToLootsDic = new Dictionary<string, List<NwItem>> { };
    public LootSystem()
    {
      NwModule.Instance.OnModuleLoad += OnModuleLoad;
    }
    private void OnModuleLoad(ModuleEvents.OnModuleLoad onModuleLoad)
    {
      InitChestArea();
    }
    private void InitChestArea()
    {
      NwArea area = NwModule.Instance.Areas.FirstOrDefault(a => a.Tag == CHEST_AREA_TAG);
      if (area == null)
      {
        Task waitForDiscordBot = NwTask.Run(async () =>
        {
          await NwTask.Delay(TimeSpan.FromSeconds(5));
          Utils.LogMessageToDMs("Attention - La zone des loots n'est pas initialisée.");
        });
        
        return;
      }

      var query = SqLiteUtils.SelectQuery(SQL_TABLE,
        new List<string>() { { "serializedChest" }, { "position" }, { "facing" } },
        new List<string[]>() );

      foreach (var result in query.Results)
      {
        NwPlaceable oChest = SqLiteUtils.PlaceableSerializationFormatProtection(result, 0, Utils.GetLocationFromDatabase(CHEST_AREA_TAG, result.GetString(1), result.GetFloat(2)));

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
        oChest.OnClose += OnLootConfigContainerClose;
      }

      InitializeLootChestFromArray(NwObject.FindObjectsWithTag<NwPlaceable>("low_blueprints").FirstOrDefault(), Craft.Collect.System.lowBlueprints);
      InitializeLootChestFromArray(NwObject.FindObjectsWithTag<NwPlaceable>("medium_blueprints").FirstOrDefault(), Craft.Collect.System.mediumBlueprints);

      InitializeLootChestFromFeatArray(NwObject.FindObjectsWithTag<NwPlaceable>("low_skillbooks").FirstOrDefault(), SkillSystem.lowSkillBooks);
      InitializeLootChestFromFeatArray(NwObject.FindObjectsWithTag<NwPlaceable>("medium_skillbooks").FirstOrDefault(), SkillSystem.mediumSkillBooks);

      InitializeLootChestFromScrollArray(NwObject.FindObjectsWithTag<NwPlaceable>("low_enchantements").FirstOrDefault(), SpellSystem.lowEnchantements);
      InitializeLootChestFromScrollArray(NwObject.FindObjectsWithTag<NwPlaceable>("medium_enchantements").FirstOrDefault(), SpellSystem.mediumEnchantements);
      InitializeLootChestFromScrollArray(NwObject.FindObjectsWithTag<NwPlaceable>("high_enchantements").FirstOrDefault(), SpellSystem.highEnchantements);
    }

    private async void InitializeLootChestFromArray(NwPlaceable oChest, int[] array)
    {
      foreach (int baseItemType in array)
      {
        var blueprint = new Blueprint(baseItemType);

        if (!Craft.Collect.System.blueprintDictionnary.ContainsKey(baseItemType))
          Craft.Collect.System.blueprintDictionnary.Add(baseItemType, blueprint);

        NwItem oBlueprint = await NwItem.Create("blueprintgeneric", oChest, 1, "blueprint");
        oBlueprint.Name = $"Patron : {blueprint.name}";
        oBlueprint.GetLocalVariable<int>("_BASE_ITEM_TYPE").Value = baseItemType;
      }

      UpdateChestTagToLootsDic(oChest);
    }
    private async void InitializeLootChestFromFeatArray(NwPlaceable oChest, Feat[] array)
    {
      foreach (Feat feat in array)
      {
        NwItem skillBook = await NwItem.Create("skillbookgeneriq", oChest, 1, "skillbook");
        skillBook.Appearance.SetSimpleModel((byte)Utils.random.Next(0, 50));
        skillBook.GetLocalVariable<int>("_SKILL_ID").Value = (int)feat;

        FeatTable.Entry entry = Feat2da.featTable.GetFeatDataEntry(feat);

        if (SkillSystem.customFeatsDictionnary.ContainsKey(feat))
        {
          skillBook.Name = SkillSystem.customFeatsDictionnary[feat].name;
          skillBook.Description = SkillSystem.customFeatsDictionnary[feat].description;
        }
        else
        {
          skillBook.Name = entry.name;
          skillBook.Description = entry.description;
        }

        skillBook.BaseGoldValue = (uint)(entry.CRValue * 1000);
      }

      UpdateChestTagToLootsDic(oChest);
    }
    private async void InitializeLootChestFromScrollArray(NwPlaceable oChest, int[] array)
    {
      foreach (int itemPropertyId in array)
      {
        NwItem oScroll = await NwItem.Create("spellscroll", oChest, 1, "scroll");
        SpellsTable.Entry spellEntry = Spells2da.spellsTable.GetSpellDataEntry(ItemPropertySpells2da.spellsTable.GetSpellDataEntry(itemPropertyId).spell);

        oScroll.Name = $"{spellEntry.name}";
        oScroll.Description = $"{spellEntry.description}";

        oScroll.AddItemProperty(ItemProperty.CastSpell((IPCastSpell)itemPropertyId, IPCastSpellNumUses.SingleUse), EffectDuration.Permanent);
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

      if(oContainer.Tag.StartsWith("boss_"))
      {
        foreach (NwPlaceable chest in oContainer.Area.FindObjectsOfTypeInArea<NwPlaceable>().Where(c => lootablesDic.ContainsKey(c.Tag)))
        {
          //Log.Info($"Found chest : {chest.Name}");

          Utils.DestroyInventory(chest);

          if (lootablesDic.TryGetValue(chest.Tag, out Lootable.Config lootableChest))
          {
            lootableChest.GenerateLoot(chest);
          }
          else
            Utils.LogMessageToDMs($"AREA - {oContainer.Area.Name} - Unregistered container tag=\"{chest.Tag}\", name : {chest.Name}");
        }
      }
    }
  }
}
