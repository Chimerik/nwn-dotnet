using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using NLog;
using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;

namespace NWN.Systems
{
  [ServiceBinding(typeof(LootSystem))]
  public partial class LootSystem
  {
    private readonly SpellSystem spellSystem;
    public static readonly Logger Log = LogManager.GetCurrentClassLogger();
    private static readonly Dictionary<string, List<NwItem>> chestTagToLootsDic = new();
    public LootSystem(SpellSystem spellSystem)
    {
      this.spellSystem = spellSystem;
      NwModule.Instance.OnModuleLoad += OnModuleLoad;
    }
    private void OnModuleLoad(ModuleEvents.OnModuleLoad onModuleLoad)
    {
      InitChestArea();
    }
    private async void InitChestArea()
    {
      NwArea area = NwModule.Instance.Areas.FirstOrDefault(a => a.Tag == CHEST_AREA_TAG);

      if (area == null)
      {
        await NwTask.Delay(TimeSpan.FromSeconds(5));
        Utils.LogMessageToDMs("Attention - La zone des loots n'est pas initialisée.");
        return;
      }

      var query = SqLiteUtils.SelectQuery(SQL_TABLE,
        new List<string>() { { "serializedChest" }, { "position" }, { "facing" } },
        new List<string[]>() );

      await NwTask.SwitchToMainThread();

      foreach (var result in query)
      {
        NwPlaceable oChest = SqLiteUtils.PlaceableSerializationFormatProtection(result[0], Utils.GetLocationFromDatabase(CHEST_AREA_TAG, result[1], float.Parse(result[2])));

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

      InitializeLootChestFromArray(NwObject.FindObjectsWithTag<NwPlaceable>("low_blueprints").FirstOrDefault(), Craft.Collect.System.lowWeaponBlueprints);
      InitializeLootChestFromArray(NwObject.FindObjectsWithTag<NwPlaceable>("medium_blueprints").FirstOrDefault(), Craft.Collect.System.mediumWeaponBlueprints);
      InitializeLootChestFromArray(NwObject.FindObjectsWithTag<NwPlaceable>("low_blueprints").FirstOrDefault(), Craft.Collect.System.lowArmorBlueprints);
      InitializeLootChestFromArray(NwObject.FindObjectsWithTag<NwPlaceable>("medium_blueprints").FirstOrDefault(), Craft.Collect.System.mediumArmorBlueprints);

      //InitializeLootChestFromFeatArray(NwObject.FindObjectsWithTag<NwPlaceable>("low_skillbooks").FirstOrDefault(), SkillSystem.lowSkillBooks);
      //InitializeLootChestFromFeatArray(NwObject.FindObjectsWithTag<NwPlaceable>("medium_skillbooks").FirstOrDefault(), SkillSystem.mediumSkillBooks);

      InitializeLootChestFromScrollArray(NwObject.FindObjectsWithTag<NwPlaceable>("low_enchantements").FirstOrDefault(), spellSystem.lowEnchantements);
      InitializeLootChestFromScrollArray(NwObject.FindObjectsWithTag<NwPlaceable>("medium_enchantements").FirstOrDefault(), spellSystem.mediumEnchantements);
      InitializeLootChestFromScrollArray(NwObject.FindObjectsWithTag<NwPlaceable>("high_enchantements").FirstOrDefault(), spellSystem.highEnchantements);
    }

    private async void InitializeLootChestFromArray(NwPlaceable oChest, BaseItemType[] array)
    {
      foreach (BaseItemType baseItemType in array)
      {
        NwItem oBlueprint = await NwItem.Create("blueprintgeneric", oChest, 1, "blueprint");
        ItemUtils.CreateShopWeaponBlueprint(oBlueprint, baseItemType);
      }

      UpdateChestTagToLootsDic(oChest);
    }
    private async void InitializeLootChestFromArray(NwPlaceable oChest, int[] array)
    {
      foreach (int baseACValue in array)
      {
        NwItem oBlueprint = await NwItem.Create("blueprintgeneric", oChest, 1, "blueprint");
        ItemUtils.CreateShopArmorBlueprint(oBlueprint, baseACValue);
      }

      UpdateChestTagToLootsDic(oChest);
    }
    /*private async void InitializeLootChestFromFeatArray(NwPlaceable oChest, Feat[] array)
    {
      foreach (Feat feat in array)
      {
        NwItem skillBook = await NwItem.Create("skillbookgeneriq", oChest, 1, "skillbook");
        ItemUtils.CreateShopSkillBook(skillBook, (int)feat);
        UpdateChestTagToLootsDic(oChest);
      }
    }*/
    private async void InitializeLootChestFromScrollArray(NwPlaceable oChest, int[] array)
    {
      foreach (int itemPropertyId in array)
      {
        NwItem oScroll = await NwItem.Create("spellscroll", oChest, 1, "scroll");
        NwSpell spellEntry = NwSpell.FromSpellId(NwGameTables.ItemPropertyTable.GetRow(15).SubTypeTable.GetInt(itemPropertyId, "SpellIndex").Value); // 15 = ItemProperty CastSpell
        oScroll.Name = $"{spellEntry.Name}";
        oScroll.Description = $"{spellEntry.Description}";

        oScroll.AddItemProperty(ItemProperty.CastSpell((IPCastSpell)itemPropertyId, IPCastSpellNumUses.SingleUse), EffectDuration.Permanent);
        oScroll.GetObjectVariable<LocalVariableString>("ITEM_KEY").Value = Config.itemKey;
      }

      UpdateChestTagToLootsDic(oChest);
    }
    private void OnLootConfigContainerClose(PlaceableEvents.OnClose onClose)
    {
      UpdateChestTagToLootsDic(onClose.Placeable);
      UpdateDB(onClose.Placeable, onClose.ClosedBy);
    }
    public static void HandleLoot(CreatureEvents.OnDeath onDeath)
    {
      NwCreature oContainer = onDeath.KilledCreature;

      if (lootablesDic.TryGetValue(oContainer.Tag, out Lootable.Config lootableConfig))
      {
        ItemUtils.MakeCreatureInventoryUndroppable(oContainer);
        lootableConfig.GenerateLoot(oContainer);
      }
      //else
        //Utils.LogMessageToDMs($"Unregistered container tag=\"{oContainer.Tag}\"");

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
