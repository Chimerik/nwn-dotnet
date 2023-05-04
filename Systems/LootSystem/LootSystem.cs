using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NLog;
using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;
using Newtonsoft.Json;
using NWN.Systems.Arena;

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
      LoadLootSystem();
    }
    private static async void LoadLootSystem()
    {
      var result = await SqLiteUtils.SelectQueryAsync("lootSystem",
            new List<string>() { { "loot" } },
            new List<string[]>() { });

      if (result == null || result.Count < 1)
      {
        Dictionary<LootQuality, List<string>> newLootDico = new();

        foreach (LootQuality category in (LootQuality[])Enum.GetValues(typeof(LootQuality)))
        {
          newLootDico.Add(category, new List<string>());
          lootDictionary.Add(category, new List<NwItem>());
        }

        await SqLiteUtils.InsertQueryAsync("lootSystem",
          new List<string[]>() { new string[] { "loot", JsonConvert.SerializeObject(newLootDico) } });

        return;
      }

      string serializedLoots = result.FirstOrDefault()[0];
      Dictionary<LootQuality, List<string>> serializedLootDico = new();

      await Task.Run(() =>
      {
        if (string.IsNullOrEmpty(serializedLoots) || serializedLoots == "null")
          return;

        serializedLootDico = JsonConvert.DeserializeObject<Dictionary<LootQuality, List<string>>>(serializedLoots);
      });

      foreach (var entry in serializedLootDico)
      {
        List<NwItem> category = new();
        lootDictionary.Add(entry.Key, category);

        foreach (string serializedItem in entry.Value)
          category.Add(NwItem.Deserialize(serializedItem.ToByteArray()));
      }
    }
    /*private async void InitChestArea()
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
    /* private async void InitializeLootChestFromScrollArray(NwPlaceable oChest, int[] array)
     {
       foreach (int itemPropertyId in array)
       {
         NwItem oScroll = await NwItem.Create("spellscroll", oChest, 1, "scroll");
         NwSpell spellEntry = NwSpell.FromSpellId(NwGameTables.ItemPropertyTable.GetRow(15).SubTypeTable.GetInt(itemPropertyId, "SpellIndex").Value); // 15 = ItemProperty CastSpell
         oScroll.Name = $"{spellEntry.Name}";
         oScroll.Description = $"{spellEntry.Description}";

         oScroll.AddItemProperty(ItemProperty.CastSpell((IPCastSpell)itemPropertyId, IPCastSpellNumUses.SingleUse), EffectDuration.Permanent);
         //oScroll.GetObjectVariable<LocalVariableString>("ITEM_KEY").Value = Config.itemKey;
       }

       UpdateChestTagToLootsDic(oChest);
     }
     private void OnLootConfigContainerClose(PlaceableEvents.OnClose onClose)
     {
       UpdateChestTagToLootsDic(onClose.Placeable);
       UpdateDB(onClose.Placeable, onClose.ClosedBy);
     }*/
    public static void HandleLoot(CreatureEvents.OnDeath onDeath)
    {
      NwCreature deadCreature = onDeath.KilledCreature;

      if (deadCreature.Tag.StartsWith("boss_")) // Cas spécifique où la créature est un boss et loot de meilleures récompenses
      {

      }
      else // Cas des mobs génériques
      {
        // 5 % de chance de drop de
      }
    }
    /*public static void HandleLoot(CreatureEvents.OnDeath onDeath)
    {
      NwCreature oContainer = onDeath.KilledCreature;

      if(oContainer.Area is null)
      {
        LogUtils.LogMessage($"{onDeath.Killer.Name} tue {oContainer.Name} hors zone - Pas de loot", LogUtils.LogType.LootSystem);
        return;
      }

      int areaLevel = oContainer.Area.GetObjectVariable<LocalVariableInt>("_AREA_LEVEL").Value;

      if(areaLevel < 2)
      {
        LogUtils.LogMessage($"{oContainer.Area.Name} (zone level {areaLevel}) - {onDeath.Killer.Name} tue {oContainer.Name} - Pas de loot", LogUtils.LogType.LootSystem);
        return;
      }

      /*if (lootablesDic.TryGetValue(oContainer.Tag, out Lootable.Config lootableConfig))
      {
        ItemUtils.MakeCreatureInventoryUndroppable(oContainer);
        lootableConfig.GenerateLoot(oContainer);
      }*/
      //else
      //Utils.LogMessageToDMs($"Unregistered container tag=\"{oContainer.Tag}\"");

      /*if (oContainer.Tag.StartsWith("boss_"))
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

      int dropChance = NwRandom.Roll(Utils.random, 100);

      if (oContainer.Tag.StartsWith("boss_"))
      {

      }
      else if (dropChance < (Config.baseCreatureDropChance + 2 * areaLevel) * 2) // Normal Creature Item Drop
      {
        //NwItem lootItem = CreateLootItem(areaLevel);
      }
      else if(dropChance  < Config.baseCreatureDropChance + 2 * areaLevel) // Normal Creature Gold Drop
      {
        int goldDrop = Utils.random.Next(Config.minCreatureGoldDrop + ((areaLevel - 2) * Config.creatureGoldDropAreaMultiplier), Config.maxCreatureGoldDrop + ((areaLevel - 2) * Config.creatureGoldDropAreaMultiplier) + 1);
        // TODO : spawn n'importe quel objet de la catégorie Inutile et lui ajouter "Extractible" dans le nom
        // TODO : Lui affecter la valeur de goldDrop sur une variable locale
        // TODO : Créer un PNJ qui effectue l'extraction d'influx pour une rentabilité dérisoire
        // TODO : Créer des compétences d'extraction d'influx (le job est instant à partir de n'import quel atelier)
      }
    }*/
  }
}
