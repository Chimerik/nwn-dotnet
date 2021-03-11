using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NWN.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class LootSystem
  {
    public class Lootable
    {
      public struct Config
      {
        public Gold? gold { get; set; }
        public List<Item> items { get; set; }

        public Config(List<Item> items, Gold? gold = null)
        {
          this.gold = gold;
          this.items = items;
        }

        public void GenerateLoot(NwGameObject oContainer)
        {
          if (NWScript.GetHasInventory(oContainer) == 0)
          {
            Utils.LogMessageToDMs($"Can't GenerateLoot : Object '{oContainer.Tag}' in area '{oContainer.Area.Name}' has no inventory.");
            return;
          }

          if (gold != null)
            gold.GetValueOrDefault().Generate(oContainer);

          GenerateItems(oContainer);
        }

        private void GenerateItems(NwGameObject oContainer)
        {
          foreach (var item in items)
          {
            item.Generate(oContainer);
          }
        }
      }

      public struct Gold
      {
        public uint min { get; set; }
        public uint max { get; set; }
        public uint chance { get; set; }

        public Gold(uint max, uint min = 0, uint chance = 100)
        {
          this.min = min;
          this.max = max;
          this.chance = chance;
        }

        public void Generate(NwGameObject oContainer)
        {
          Log.Info($"tag : {oContainer.Tag}");
          int rand = Utils.random.Next(1, 101);
          Log.Info($"GOLD CHANCE : {rand}/{chance}");
          if (rand <= chance)
          {
            int goldCount = Utils.random.Next((int)min, (int)max);
            
            NwItem.Create("NW_IT_GOLD001", oContainer, goldCount);
            Log.Info($"GOLD LOOTED : {goldCount}");

            if (ModuleSystem.goldBalanceMonitoring.TryGetValue(oContainer.Tag, out GoldBalance gold))
            {
              gold.nbTimesLooted++;
              gold.cumulatedGold += goldCount;
            }
            else
            {
              ModuleSystem.goldBalanceMonitoring.Add(oContainer.Tag, new GoldBalance(goldCount));
            }
          }
        }
      }

      public struct Item
      {
        public string chestTag { get; set; }
        public uint count { get; set; }
        public uint chance { get; set; }

        public Item(string chestTag, uint count = 1, uint chance = 100)
        {
          this.chestTag = chestTag;
          this.count = count;
          this.chance = chance;
        }

        public void Generate(NwGameObject oContainer)
        {
          Log.Info($"tag : {oContainer.Tag}");
          if (chestTagToLootsDic.TryGetValue(chestTag, out List<NwItem> loots))
          {
            if (loots.Count > 0)
            {
              for (var i = 0; i < count; i++)
              {
                int rand = Utils.random.Next(1, 101);
                Log.Info($"LOOT CHANCE : {rand}/{chance}");
                if (rand <= chance)
                {
                  NwItem oItem = loots[NWN.Utils.random.Next(0, loots.Count - 1)].Clone(oContainer, "", true);
                  Log.Info($"SUCCESS : item created {oItem.Name}");

                  Craft.Collect.System.AddCraftedItemProperties(oItem, "mauvais état");
                }
              }
            }
          }
          else
          {
            ThrowException($"Invalid chest tag '{chestTag}'");
          }
        }
      }
    }
  }
}
